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
using gScr = geeWiz.Utilities.Script_Utils;
using gWsh = geeWiz.Utilities.Workshare_Utils;
using gXcl = geeWiz.Utilities.Excel_Utils;

// The class belongs to the Commands namespace
namespace geeWiz.Cmds_Revision
{
    #region Cmd_BulkRev

    /// <summary>
    /// Adds or removes revisions from sheets.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_BulkRev : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Detect alt fire
            var altFired = gScr.KeyHeldShift();

            // Select a revision
            var formResultRevision = doc.Ext_SelectRevisions(
                title: altFired ? "Revision to remove" : "Revision to add",
                multiSelect: false,
                sorted: true);
            if (formResultRevision.Cancelled) { return Result.Cancelled; }
            var selectedRevision = formResultRevision.Object;

            // Select sheets
            var formResultSheets = doc.Ext_SelectSheets(sorted: true);
            if (formResultSheets.Cancelled) { return Result.Cancelled; }
            var sheets = formResultSheets.Objects;

            //Filter out workshared sheets
            if (doc.IsWorkshared)
            {
                var worksharingResult = gWsh.ProcessElements<ViewSheet>(sheets, doc);
                sheets = worksharingResult.Editable;
            }

            // Progress bar properties
            int pbTotal = sheets.Count;
            int pbStep = gFrm.Utilities.ProgressDelay(pbTotal);
            int updated = 0;

            // Using a progress bar
            using (var pb = new gFrm.Bases.ProgressBar(
                taskName: altFired ? "Removing revision from sheet(s)..." : "Adding revision to sheet(s)...",
                pbTotal: pbTotal))
            {
                // Using a transaction
                using (var t = new Transaction(doc, "geeWiz: BulkRev"))
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

                        // Add or remove result if altfired
                        if (altFired)
                        {
                            updated += sheet.Ext_RemoveRevision(selectedRevision).Ext_ToInteger();
                        }
                        else
                        {
                            updated += sheet.Ext_AddRevision(selectedRevision).Ext_ToInteger();
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
                $"{updated}/{pbTotal} sheets updated.\n\n" +
                $"Skipped sheets required no changes.");
        }
    }

    #endregion

    #region Cmd_RevSet

    /// <summary>
    /// Creates a sheet set with revised sheets.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_RevSet : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Select a revision
            var formResultRevision = doc.Ext_SelectRevisions(multiSelect: false, sorted: true);
            if (formResultRevision.Cancelled) { return Result.Cancelled; }
            var selectedRevision = formResultRevision.Object as Revision;

            // Get all revised sheets
            var sheets = doc.Ext_GetSheets()
                .Where(s => s.GetAllRevisionIds().Contains(selectedRevision.Id))
                .ToList();

            // Cancel if no sheets
            if (sheets.Count == 0)
            {
                return gFrm.Custom.Cancelled("No sheets found with that revision.");
            }

            // Collect sheet sets and names
            var sheetSets = doc.Ext_Collector()
                .OfClass(typeof(ViewSheetSet))
                .Cast<ViewSheetSet>()
                .ToList();
            var sheetSetNames = sheetSets
                .Select(s => s.Name)
                .ToList();

            // New set name
            var sheetSetName = selectedRevision.Ext_ToRevisionKey();

            // Using a transaction...
            using (var t = new Transaction(doc, "geeWiz: Delete sheetset"))
            {
                // Start the transaction
                t.Start();

                if (sheetSetNames.Contains(sheetSetName))
                {
                    // Get the sheet set
                    int ind = sheetSetNames.IndexOf(sheetSetName);
                    var sheetSetExisting = sheetSets[ind];

                    // If existing sheet set is editable
                    if ((sheetSetExisting as Element).Ext_IsEditable(doc))
                    {
                        // Delete the sheetset
                        doc.Ext_DeleteElement<ViewSheetSet>(sheetSetExisting);
                    }
                    else
                    {
                        // Otherwise, cancel the task
                        return gFrm.Custom.Cancelled("Sheet set exists, but is not editable.");
                    }
                }

                // Create a viewset, add the sheets to it
                var viewSet = new ViewSet();
                foreach (var sheet in sheets) { viewSet.Insert(sheet as View); }

                // Get current sheet setting
                var printManager = doc.PrintManager;
                printManager.PrintRange = PrintRange.Select;
                var viewSheetSetting = printManager.ViewSheetSetting;

                /// Save the sheet set
                viewSheetSetting.CurrentViewSheetSet.Views = viewSet;
                viewSheetSetting.SaveAs(sheetSetName);

                // Commit the transaction
                t.Commit();
            }

            // Final message to user
            return gFrm.Custom.BubbleMessage(title: "Task completed",
                message: "ViewSheetSet created.");
        }
    }

    #endregion

    #region Cmd_DocTrans

    /// <summary>
    /// Creates a document transmittal.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_DocTrans : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Select a revision
            var formResultRevision = doc.Ext_SelectRevisions(sorted: true);
            if (formResultRevision.Cancelled) { return Result.Cancelled; }
            var revisions = formResultRevision.Objects;

            // Select sheets
            var formResultSheets = doc.Ext_SelectSheets(sorted: true);
            if (formResultSheets.Cancelled) { return Result.Cancelled; }
            var sheets = formResultSheets.Objects;

            // Construct doctans, header row
            var matrix = new List<List<string>>();
            var header = new List<string>() { "Number", "Name", "Current"};
            var revisionIds = new List<ElementId>();

            foreach (var revision in revisions)
            {
                header.Add(revision.Ext_ToRevisionKey());
                revisionIds.Add(revision.Id);
            }

            // Add header row to matrix
            matrix.Add(header);

            // For each sheet
            foreach (var sheet in sheets)
            {
                // New row with sheet number and name
                var row = new List<string>()
                {
                    sheet.SheetNumber,
                    sheet.Name
                };

                // Get current revision
                if (sheet.GetCurrentRevision() != ElementId.InvalidElementId)
                {
                    row.Add(sheet.GetRevisionNumberOnSheet(sheet.GetCurrentRevision()));
                }
                else
                {
                    row.Add("-");
                }

                // For each revision
                foreach (var id in revisionIds)
                {
                    // Add its number to the row for the sheet
                    var revisionNumber = sheet.GetRevisionNumberOnSheet(id);
                    revisionNumber ??= "";
                    row.Add(revisionNumber);
                }

                // Add row to matrix
                matrix.Add(row);
            }

            // Select directory
            var directoryResult = gFrm.Custom.SelectDirectoryPath("Select where to save the transmittal");
            if (directoryResult.Cancelled) { return Result.Cancelled; }
            var directoryPath = directoryResult.Object;
            var filePath = Path.Combine(directoryPath, "Doctrans.xlsx");

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
                        worksheetName: "Doctrans", getFirstOtherwise: true);
                    worksheet.Clear();
                }
                else
                {
                    // Otherwise, add the worksheet
                    worksheet = workbook.AddWorksheet("Doctrans");
                }

                // Write the matrix to the workbook
                gXcl.WriteToWorksheet(worksheet, matrix);

                // Make the header row taller
                worksheet.Row(1).Height = 150;

                // For each column...
                for (int i = 1; i <= revisionIds.Count + 3; i++)
                {
                    // First 3 columns set to 30 wide
                    if (i < 3)
                    {
                        worksheet.Column(i).Width = 30;
                    }
                    // Remainder set to 5, roated 90 degrees (revision columns)
                    else
                    {
                        worksheet.Cell(1, i).Style.Alignment.TextRotation = 90;
                        worksheet.Column(i).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                        worksheet.Column(i).Width = 5;
                    }
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
            return gFrm.Custom.BubbleMessage(title: "Doctrans written", filePath: filePath);
        }
    }

    #endregion
}