// System
using System.IO;
// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using View = Autodesk.Revit.DB.View;
// geeWiz
using geeWiz.Extensions;
using gFrm = geeWiz.Forms;
using gFil = geeWiz.Utilities.File_Utils;
using gXcl = geeWiz.Utilities.Excel_Utils;
using gView = geeWiz.Utilities.View_Utils;
using gScr = geeWiz.Utilities.Script_Utils;
// ClosedXML
using ClosedXML.Excel;

// The class belongs to the Commands namespace
namespace geeWiz.Commands.Cmds_Export
{
    /// <summary>
    /// A Revit command ran from the ribbon.
    /// Exports a Revit schedule to Excel.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_Schedule : IExternalCommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="commandData">Command related data.</param>
        /// <param name="message">Command related message.</param>
        /// <param name="elements">Command related elements.</param>
        /// <returns>A Result.</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            View activeView = uiDoc.ActiveView;

            // Ensure we have a schedule
            if (activeView is not ViewSchedule)
            {
                return gFrm.Custom.Cancelled("Active view is not a schedule");
            }

            // Matrix to write to
            var matrix = new List<List<string>>();

            // Get table data and section data
            ViewSchedule viewSchedule = activeView as ViewSchedule;
            TableData tableData = viewSchedule.GetTableData();
            TableSectionData tableSectionData = tableData.GetSectionData(SectionType.Body);

            // For each row...
            for (int r = 0; r < tableSectionData.NumberOfRows; r++)
            {
                // New row to make
                var row = new List<string>();

                // For each column...
                for (int c = 0; c < tableSectionData.NumberOfColumns; c++)
                {
                    // Add text to row
                    string cellText = viewSchedule.GetCellText(SectionType.Body, r, c);
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
            var directoryResult = gFrm.Custom.SelectFolder("Choose where to save the file");
            if (directoryResult.Cancelled) { return Result.Cancelled; }
            string directoryPath = directoryResult.Object;
            var filePath = Path.Combine(directoryPath, "Export schedule.xlsx");

            // Accessibility check if it exists
            if (File.Exists(filePath) && !gFil.FileIsAccessible(filePath))
            {
                return gFrm.Custom.Cancelled("File exists and is not editable.\n\n" +
                        "Ensure it is closed and try again.");
            }

            // Using a workbook object
            using (var workbook = gXcl.CreateWorkbook(filePath))
            {
                // Establish workbook variable
                IXLWorksheet worksheet = null;

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

    /// <summary>
    /// A Revit command ran from the ribbon.
    /// Exports selected Sheets to PDF.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_SheetsPdf : IExternalCommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="commandData">Command related data.</param>
        /// <param name="message">Command related message.</param>
        /// <param name="elements">Command related elements.</param>
        /// <returns>A Result.</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Check for alt fire
            bool altFire = gScr.KeyHeldShift();

            // Select sheets to export
            var formResults = doc.Ext_SelectSheets(title: "Select sheets to export", sorted: true);
            if (formResults.Cancelled) { return Result.Cancelled; }
            List<ViewSheet> sheets = formResults.Objects;

            // Select directory to export to
            var directoryResult = gFrm.Custom.SelectFolder("Select where to export to");
            if (directoryResult.Cancelled) {  return Result.Cancelled; }
            string directoryPath = directoryResult.Object;

            // Pdf export options
            PDFExportOptions options = gView.DefaultPdfExportOptions(hideCrop: !altFire);

            // Progress bar properties
            var pb = new gFrm.ProgressCoordinator(total: sheets.Count, taskName: "Exporting sheets");

            // Using a transaction
            using (var t = new Transaction(doc, "geeWiz: Export sheets"))
            {
                // Start the transaction
                t.Start();

                // For each sheet
                foreach (var sheet in sheets)
                {
                    // Check for cancellation
                    if (pb.CancelCheckOrUpdate(t: t))
                    {
                        return Result.Cancelled;
                    }

                    // Export the sheet to Pdf
                    sheet.Ext_ExportToPdf(
                        fileName: sheet.Ext_ToExportKey(),
                        directoryPath: directoryPath,
                        doc: doc,
                        options: options);
                }

                // Commit the transaction
                pb.Commit(t: t);
            }

            // Finish by opening the directory path
            return gFil.OpenDirectory(directoryPath);
        }
    }

    /// <summary>
    /// A Revit command ran from the ribbon.
    /// Exports selected Sheets to DWG.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_SheetsDwg : IExternalCommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="commandData">Command related data.</param>
        /// <param name="message">Command related message.</param>
        /// <param name="elements">Command related elements.</param>
        /// <returns>A Result.</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Check for alt fire
            bool altFire = gScr.KeyHeldShift();

            // Select sheets to export
            var formResults = doc.Ext_SelectSheets(title: "Select sheets to export", sorted: true);
            if (formResults.Cancelled) { return Result.Cancelled; }
            List<ViewSheet> sheets = formResults.Objects;

            // Select directory to export to
            var directoryResult = gFrm.Custom.SelectFolder("Select where to export to");
            if (directoryResult.Cancelled) { return Result.Cancelled; }
            string directoryPath = directoryResult.Object;

            // Dwg export options
            var options = gView.DefaultDwgExportOptions(shared: altFire);

            // Progress bar properties
            var pb = new gFrm.ProgressCoordinator(total: sheets.Count, taskName: "Exporting sheets");

            // Using a transaction
            using (var t = new Transaction(doc, "geeWiz: Export sheets"))
            {
                // Start the transaction
                t.Start();

                // For each sheet
                foreach (var sheet in sheets)
                {
                    // Check for cancellation
                    if (pb.CancelCheckOrUpdate(t: t))
                    {
                        return Result.Cancelled;
                    }

                    // Export the sheet to Dwg
                    sheet.Ext_ExportToDwg(
                        fileName: sheet.Ext_ToExportKey(),
                        directoryPath: directoryPath,
                        doc: doc,
                        options: options);
                }

                // Commit the transaction
                pb.Commit(t: t);
            }

            // Finish by opening the directory path
            return gFil.OpenDirectory(directoryPath);
        }
    }
}