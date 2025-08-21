// System
using System.IO;
// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
// geeWiz
using geeWiz.Extensions;
using geeWiz.Utilities;
using gFrm = geeWiz.Forms;
using gFil = geeWiz.Utilities.File_Utils;
using gXcl = geeWiz.Utilities.Excel_Utils;
using gScr = geeWiz.Utilities.Script_Utils;

// The class belongs to the Commands namespace
namespace geeWiz.Cmds_Import
{
    #region Cmd_SheetsExcel

    /// <summary>
    /// Creates an Excel template.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_SheetsExcel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Check for alt firing
            var altFired = gScr.KeyHeldShift();

            // Select a directory, make file path
            var directoryResult = gFrm.Custom.SelectDirectoryPath("Choose where to save template");
            if (directoryResult.Cancelled) { return Result.Cancelled; }
            var directoryPath = directoryResult.Object;
            var filePath = Path.Combine(directoryPath, "Import sheets.xlsx");

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

            // Default matrix to write
            var matrix = new List<List<string>>()
                {
                    new List<string>() { "Number", "Name" },
                };

            // If alt fired...
            if (altFired)
            {
                // For each sheet
                foreach (var sheet in doc.Ext_GetSheets(sorted: true))
                {
                    // Add it as a row to the matrix
                    matrix.Add(new List<string>() { sheet.SheetNumber, sheet.Name });
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
                        worksheetName: "Sheets", getFirstOtherwise: true);
                    worksheet.Clear();
                }
                else
                {
                    // Otherwise, add the worksheet
                    worksheet = workbook.AddWorksheet("Sheets");
                }

                // Write the matrix to the workbook
                gXcl.WriteToWorksheet(worksheet, matrix);

                // Set column widths and first row to bold
                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 45;
                worksheet.Row(1).Style.Font.Bold = true;

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
            return gFrm.Custom.BubbleMessage(title: "Template created", filePath: filePath);
        }
    }

    #endregion

    #region Cmd_CreateSheets

    /// <summary>
    /// Creates/updates sheets from Excel.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_CreateSheets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Select file path
            var formResult = gFrm.Custom.SelectFilePaths(
                title: "Select sheet list",
                filter: gFrm.Custom.FILTER_EXCEL,
                multiSelect: false);
            if (formResult.Cancelled) { return Result.Cancelled; }
            var filePath = formResult.Object;

            // Accessible check
            if (!gFil.FileIsAccessible(filePath))
            {
                return gFrm.Custom.Cancelled(
                    "File is not editable.\n\n" +
                    "Ensure it is closed and try again.");
            }

            // Get Excel data
            var workbook = gXcl.GetWorkbook(filePath);
            var worksheet = gXcl.GetWorkSheet(workbook, "Sheets", getFirstOtherwise: true);
            var matrix = gXcl.ReadFromWorksheet(worksheet);

            // Validate file structure
            if (matrix.Count < 2 || matrix[0][0] != "Number" || matrix[0][1] != "Name")
            {
                return gFrm.Custom.Cancelled(
                    "Template appears to be incorrect.\n\n" +
                    "Ensure it is generated from the tool above this one.");
            }

            // Remove header row
            matrix.RemoveAt(0);

            // Select titleblock type
            var formResultTtb = doc.Ext_SelectTitleblockTypes(multiSelect: false, sorted: true);
            if (formResultTtb.Cancelled) { return Result.Cancelled; }
            var titleblockTypeId = formResultTtb.Object.Id;

            // Collect sheets and numbers
            var sheets = doc.Ext_GetSheets(includePlaceholders: true);
            // var sheetDictionar = sheets.ToDictionary(s => s.SheetNumber); easier, but not assured in 2026+
            var sheetDictionary = sheets.QuickDictionary(s => s.SheetNumber.ToLower(), s => s);

            // Progress bar properties
            int pbTotal = matrix.Count;
            int pbStep = gFrm.Utilities.ProgressDelay(pbTotal);

            // Tracker variables
            int updated = 0;
            int created = 0;
            int skipped = 0;

            // Using a progress bar
            using (var pb = new gFrm.Bases.ProgressBar(taskName: "Creating/updating sheets...", pbTotal: pbTotal))
            {
                // Using a transaction
                using (var t = new Transaction(doc, "geeWiz: Import sheets"))
                {
                    // Start the transaction
                    t.Start();

                    // For each sheet
                    foreach (var row in matrix)
                    {
                        // Check for cancellation
                        if (pb.CancelCheck(t))
                        {
                            return Result.Cancelled;
                        }

                        // Get the data
                        var newNumber = row[0];
                        var newName = row[1];

                        // If the sheet number exists
                        if (sheetDictionary.TryGetValue(newNumber.ToLower(), out ViewSheet exSheet))
                        {
                            if (exSheet.Ext_IsEditable(doc) && exSheet.Name != newName)
                            {
                                exSheet.Name = newName;
                                created++;
                            }
                            else
                            {
                                skipped++;
                            }
                        }
                        else
                        {
                            var newSheet = ViewSheet.Create(doc, titleblockTypeId);
                            sheetDictionary[newNumber.ToLower()] = newSheet;
                            newSheet.SheetNumber = newNumber;
                            newSheet.Name = newName;
                            created++;
                        }

                        // Increase progress
                        Thread.Sleep(pbStep);
                        pb.Increment();
                    }

                    // Commit the transaction
                    pb.Commit(t);
                }
            }

            // Return the result
            return gFrm.Custom.Completed(
                $"{created} sheets created.\n" +
                $"{updated} sheet names updated.\n" +
                $"{skipped} sheet names not editable.");
        }
    }

    #endregion
}