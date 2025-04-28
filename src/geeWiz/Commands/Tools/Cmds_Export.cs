// System
using System.IO;
// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;
// geeWiz
using geeWiz.Extensions;
using gFrm = geeWiz.Forms;
using gFil = geeWiz.Utilities.File_Utils;
using gXcl = geeWiz.Utilities.Excel_Utils;
using gView = geeWiz.Utilities.View_Utils;
using gScr = geeWiz.Utilities.Script_Utils;

// The class belongs to the Commands namespace
namespace geeWiz.Cmds_Export
{
    #region Cmd_Schedule

    /// <summary>
    /// Exports active schedule to Excel.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_Schedule : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;
            var activeView = uiDoc.ActiveView;

            // Ensure we have a schedule
            if (activeView is not ViewSchedule)
            {
                return gFrm.Custom.Cancelled("Active view is not a schedule");
            }

            // Matrix to write to
            var matrix = new List<List<string>>();

            // Get table data and section data
            var viewSchedule = activeView as ViewSchedule;
            var tableData = viewSchedule.GetTableData();
            var tableSectionData = tableData.GetSectionData(SectionType.Body);

            // Count rows and columns
            var rowCount = tableSectionData.NumberOfRows;
            var colCount = tableSectionData.NumberOfColumns;

            // For each row...
            for (int r = 0; r < rowCount; r++)
            {
                // New row to make
                var row = new List<string>();

                // For each column...
                for (int c = 0; c < colCount; c++)
                {
                    // Add text to row
                    var cellText = viewSchedule.GetCellText(SectionType.Body, r, c);
                    row.Add(cellText);
                }

                // If the row is not empty
                if (!row.All(string.IsNullOrWhiteSpace))
                {
                    // Add the row to the matrix
                    matrix.Add(row);
                }
            }

            // Select a directory, make file path
            var directoryResult = gFrm.Custom.SelectDirectoryPath("Choose where to save the file");
            if (directoryResult.Cancelled) { return Result.Cancelled; }
            var directoryPath = directoryResult.Object;
            var filePath = Path.Combine(directoryPath, "Export schedule.xlsx");

            // Accessibility check if it exists
            if (File.Exists(filePath))
            {
                if (!gFil.FileIsAccessible(filePath))
                {
                    return gFrm.Custom.Cancelled(
                        "File exists and is not editable.\n\n" +
                        "Ensure it is closed and try again.");
                }
            }

            // Using a workbook object
            using (var workbook = gXcl.CreateWorkbook(filePath))
            {
                // Establish workbook variable
                ClosedXML.Excel.IXLWorksheet worksheet = null;

                // If the file exists, clear its contents
                if (File.Exists(filePath))
                {
                    worksheet = gXcl.GetWorkSheet(workbook: workbook,
                        worksheetName: "Schedule", getFirstOtherwise: true);
                    worksheet.Clear();
                }
                else
                {
                    // Otherwise, add the worksheet
                    worksheet = workbook.AddWorksheet("Schedule");
                }

                // Write the matrix to the workbook
                gXcl.WriteToWorksheet(worksheet, matrix);

                // Make each column wider
                for (int i = 1; i <= worksheet.ColumnCount(); i++)
                {
                    worksheet.Column(i).Width = 30;
                }

                // If the workbook exists...
                if (File.Exists(filePath))
                {
                    // Save it
                    workbook.Save();
                }
                else
                {
                    // Otherwise save it to the file path
                    workbook.SaveAs(filePath);
                }
            }

            // Final message to user, click bubble to open file
            return gFrm.Custom.BubbleMessage(title: "Schedule exported", filePath: filePath);
        }
    }

    #endregion

    #region Cmd_SheetsPdf

    /// <summary>
    /// Exports sheets to Pdf.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_SheetsPdf : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Check for alt fire
            var altFire = gScr.KeyHeldShift();

            // Select sheets to export
            var formResults = doc.Ext_SelectSheets(title: "Select sheets to export", sorted: true);
            if (formResults.Cancelled) { return Result.Cancelled; }
            var sheets = formResults.Objects;

            // Select directory to export to
            var directoryResult = gFrm.Custom.SelectDirectoryPath("Select where to export to");
            if (directoryResult.Cancelled) {  return Result.Cancelled; }
            var directoryPath = directoryResult.Object;

            // Pdf export options
            var options = gView.DefaultPdfExportOptions(hideCrop: !altFire);

            // Progress bar properties
            int pbTotal = sheets.Count;
            int pbStep = gFrm.Utilities.ProgressDelay(pbTotal);

            // Using a progress bar
            using (var pb = new gFrm.Bases.ProgressBar(taskName: "Exporting sheets", pbTotal: pbTotal))
            {
                // Using a transaction
                using (var t = new Transaction(doc, "geeWiz: Export sheets"))
                {
                    // Start the transaction
                    t.Start();

                    // For each sheet
                    foreach (var sheet in sheets)
                    {
                        // Check for cancellation
                        if (pb.CancelCheck(t))
                        {
                            return Result.Cancelled;
                        }

                        // Export the sheet to Pdf
                        sheet.Ext_ExportToPdf(
                            fileName: sheet.Ext_ToExportKey(),
                            directoryPath: directoryPath,
                            doc: doc,
                            options: options);

                        // Increase progress
                        Thread.Sleep(pbStep);
                        pb.Increment();
                    }

                    // Commit the transaction
                    pb.Commit(t);
                }
            }

            // Finish by opening the directory path
            return gFil.OpenDirectory(directoryPath);
        }
    }

    #endregion

    #region Cmd_SheetsDwg

    /// <summary>
    /// Exports sheets to Dwg.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_SheetsDwg : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Check for alt fire
            var altFire = gScr.KeyHeldShift();

            // Select sheets to export
            var formResults = doc.Ext_SelectSheets(title: "Select sheets to export", sorted: true);
            if (formResults.Cancelled) { return Result.Cancelled; }
            var sheets = formResults.Objects;

            // Select directory to export to
            var directoryResult = gFrm.Custom.SelectDirectoryPath("Select where to export to");
            if (directoryResult.Cancelled) { return Result.Cancelled; }
            var directoryPath = directoryResult.Object;

            // Dwg export options
            var options = gView.DefaultDwgExportOptions(shared: altFire);

            // Progress bar properties
            int pbTotal = sheets.Count;
            int pbStep = gFrm.Utilities.ProgressDelay(pbTotal);

            // Using a progress bar
            using (var pb = new gFrm.Bases.ProgressBar(taskName: "Exporting sheets...", pbTotal: pbTotal))
            {
                // Using a transaction
                using (var t = new Transaction(doc, "geeWiz: Export sheets"))
                {
                    // Start the transaction
                    t.Start();

                    // For each sheet
                    foreach (var sheet in sheets)
                    {
                        // Check for cancellation
                        if (pb.CancelCheck(t))
                        {
                            return Result.Cancelled;
                        }

                        // Export the sheet to Dwg
                        sheet.Ext_ExportToDwg(
                            fileName: sheet.Ext_ToExportKey(),
                            directoryPath: directoryPath,
                            doc: doc,
                            options: options);

                        // Increase progress
                        Thread.Sleep(pbStep);
                        pb.Increment();
                    }

                    // Commit the transaction
                    pb.Commit(t);
                }
            }

            // Finish by opening the directory path
            return gFil.OpenDirectory(directoryPath);
        }
    }

    #endregion
}