﻿// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
// geeWiz
using geeWiz.Extensions;
using gFrm = geeWiz.Forms;

// The class belongs to the Commands namespace
namespace geeWiz.Cmds_Workset
{
    #region Cmd_Create

    /// <summary>
    /// Creates worksets from a predefined list.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_Create : IExternalCommand
    {
        private static readonly List<string> names = new List<string>()
        {
            "AR Interior",
            "AR Facade",
            "AR Structure",
            "AR FFE",
            "AR Site",
            "Z_Link DWG General",
            "Z_Link DWG Survey",
            "Z_Link RVT Services",
            "Z_Link RVT Structure"
        };
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Collect worksets and names
            var worksets = doc.Ext_GetWorksets();
            var worksetNames = worksets.Select(w => w.Name).ToList();

            // Get unused workset names
            var unusedNames = names.Where(n => !worksetNames.Contains(n)).ToList();

            // Cancel if no names
            if (unusedNames.Count == 0)
            {
                return gFrm.Custom.Completed("All standard worksets are already present.");
            }

            // Choose which worksets to create
            var formResult = gFrm.Custom.SelectFromList<string>(keys: names,
                values: names,
                title: "Select worksets to create");
            if (formResult.Cancelled) { return Result.Cancelled; }
            var chosenNames = formResult.Objects;

            // If default grids name exists, offer to rename
            if (worksetNames.Contains("Shared Levels and Grids"))
            {
                // Confirm if we want to
                var confirmResult = gFrm.Custom.Message(
                    title: "Confirm rename",
                    message: "Rename Shared Levels and Grids?",
                    yesNo: true);

                // If we do, rename it
                if (confirmResult.Affirmative)
                {
                    // Get the workset
                    int ind = worksetNames.IndexOf("Shared Levels and Grids");
                    var gridsWorkset = worksets[ind];

                    // If editable
                    if (gridsWorkset.IsEditable)
                    {
                        // Using a transaction...
                        using (var t = new Transaction(doc, "geeWiz: Rename workset"))
                        {
                            // Start the transaction
                            t.Start();

                            // Rename the workset
                            WorksetTable.RenameWorkset(doc, gridsWorkset.Id, "AR Grids and Levels");

                            // Commit the transaction
                            t.Commit();
                        }
                    }
                    else
                    {
                        // Otherwise notify the user
                        gFrm.Custom.Message(title: "Renaming skipped",
                            message: "Workset is not editable (creation process will continue).");
                    }
                }
            }

            // Progress bar properties
            int pbTotal = chosenNames.Count;
            int pbStep = gFrm.Utilities.ProgressDelay(pbTotal);

            // Using a progress bar
            using (var pb = new gFrm.Bases.ProgressBar(taskName: "Creating worksets...", pbTotal: pbTotal))
            {
                // Using a transaction
                using (var t = new Transaction(doc, "geeWiz: Create worksets"))
                {
                    // Start the transaction
                    t.Start();

                    // For each workset name
                    foreach (var name in chosenNames)
                    {
                        // Check for cancellation
                        if (pb.CancelCheck(t))
                        {
                            return Result.Cancelled;
                        }

                        // Create the workset
                        Workset.Create(doc, name);

                        // Increase progress
                        Thread.Sleep(pbStep);
                        pb.Increment();
                    }

                    // Commit the transaction
                    pb.Commit(t);
                }
            }

            // Final message to user
            return gFrm.Custom.BubbleMessage(title: "Task completed",
                message: $"{chosenNames.Count} worksets created.");
        }
    }

    #endregion
}