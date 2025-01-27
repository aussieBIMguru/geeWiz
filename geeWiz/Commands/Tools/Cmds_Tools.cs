// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
// geeWiz
using geeWiz.Extensions;
using gFrm = geeWiz.Forms;
using gWsh = geeWiz.Utilities.Workshare_Utils;

// The class belongs to the Commands namespace
namespace geeWiz.Cmds_Tools
{
    #region Cmd_Testing

    /// <summary>
    /// Testing command. Makes all sheet names in model uppercase.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_Testing : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Testing code
            var sheets = doc.Ext_GetSheets();

            //Filter out workshared sheets
            if (doc.IsWorkshared)
            {
                var worksharingResult = gWsh.ProcessElements(sheets.Cast<Element>().ToList(), doc);
                sheets = worksharingResult.Editable.Cast<ViewSheet>().ToList();
            }

            // Ensure we have sheets
            if (sheets.Count == 0)
            {
                return gFrm.Custom.Completed("No editable sheets in model to modify.");
            }

            // Progress bar properties
            int pbTotal = sheets.Count;
            int pbCount = 1;
            int pbStep = gFrm.Custom.ProgressDelay(pbTotal);

            // Using a progress bar
            using (var pb = new gFrm.ProgressView("Running a process...", total: pbTotal))
            {
                // Using a transaction
                using (var t = new Transaction(doc, "geeWiz example"))
                {
                    // Start the transaction
                    t.Start();

                    // For each sheet
                    foreach (var sheet in sheets)
                    {
                        // Check for cancellation
                        if (pb.UpdateProgress(pbCount, pbTotal))
                        {
                            t.RollBack();
                            return Result.Cancelled;
                        }

                        // If sheet name is not uppercase
                        if (sheet.Name != sheet.Name.ToUpper())
                        {
                            // Update sheet name to uppercase
                            sheet.Name = sheet.Name.ToUpper();
                        }

                        // Increase progress
                        Thread.Sleep(pbStep);
                        pbCount++;
                    }

                    // Commit the transaction
                    t.Commit();
                }
            }

            // Return the result
            return gFrm.Custom.Completed("Test script completed.\n\n" +
                "(Sheet names are now upper case)");
        }
    }

    #endregion
}