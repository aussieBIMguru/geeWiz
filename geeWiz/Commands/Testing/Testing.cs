// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
// geeWiz
using geeWiz.Extensions;
using gFrm = geeWiz.Forms;
using gFil = geeWiz.Utilities.File_Utils;
using gScr = geeWiz.Utilities.Script_Utils;
using gWsh = geeWiz.Utilities.Workshare_Utils;

// The class belongs to the Commands namespace
namespace geeWiz.Testing
{
    #region Command - Github

    /// <summary>
    /// Opens the project Github page.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_Github : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Open the Url
            string linkPath = @"https://github.com/aussieBIMguru/geeWiz";
            return gFil.OpenLinkPath(linkPath);
        }
    }

    #endregion

    #region Command - Testing

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

    #region Command - UiToggle

    /// <summary>
    /// Toggles light and dark mode (Revit 2024+).
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_UiToggle : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #if REVIT2024_OR_GREATER
            // Get panel 1 on the ribbon
            var ribbonPanel1 = Globals.UiCtlApp.Ext_GetRibbonPanelByName("Testing");

            // Get the button name and icon suffix
            string oldButtonName = Globals.IsDarkMode ? "Light mode" : "Dark mode";
            string newButtonName = Globals.IsDarkMode ? "Dark mode" : "Light mode";
            string iconSuffix = Globals.IsDarkMode ? "_Dark" : "";

            // For each panel in the tab
            var pushButton = ribbonPanel1.Ext_GetPushButtonByText(oldButtonName);
            pushButton.ItemText = newButtonName;
            pushButton.Image = gFil.GetImageSource("Testing_UiToggle", resolution: 16, suffix: iconSuffix);
            pushButton.LargeImage = gFil.GetImageSource("Testing_UiToggle", resolution: 32, suffix: iconSuffix);

            // Switch the UITheme and canvas theme (always light)
            UIThemeManager.CurrentTheme = Globals.IsDarkMode ? UITheme.Light : UITheme.Dark;
            UIThemeManager.CurrentCanvasTheme = gScr.KeyHeldShift() ? UITheme.Dark : UITheme.Light;
            Globals.IsDarkMode = !Globals.IsDarkMode;
            #endif
            // Return success
            return Result.Succeeded;
        }
    }

#endregion

    #region Command - Colour tabs

    /// <summary>
    /// Toggles tab colouring (off by default).
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_ColourTabs : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get panel 1 on the ribbon
            var ribbonPanel1 = Globals.UiCtlApp.Ext_GetRibbonPanelByName("Testing");

            // Get the colour tabs pushbutton
            var pushButton = ribbonPanel1.Ext_GetPushButtonByText("Coloured Tabs");

            // Toggle off pathway
            if (Globals.ColouringTabs)
            {
                // Message to user
                var formResult = gFrm.Custom.Message(title: "Tab colouring",
                    message: "Disable tab colouring?\n\n" +
                    "Tabs will stop being coloured, but will remain neutrally coloured until Revit restarts, " +
                    "or tab colouring is turned back on.",
                    yesNo: true);

                // If affirmative
                if (formResult.Affirmative)
                {
                    // Deregister coloring of tabs
                    ColouredTabs.DeRegister();
                    Globals.ColouringTabs = false;

                    // Set the icons
                    pushButton.Image = gFil.GetImageSource("Testing_ColourTabs", resolution: 16, suffix: "");
                    pushButton.LargeImage = gFil.GetImageSource("Testing_ColourTabs", resolution: 32, suffix: "");
                }
            }
            // Toggle on pathway
            else
            {
                // Message to user
                var formResult = gFrm.Custom.Message(title: "Tab colouring",
                    message: "Enable tab colouring?\n\n" +
                    "It is not recommended to use this feature if you are going to be opening " +
                    "a lot of different documents. Note that whilst this setting can be disabled in " +
                    "session, normal tab colouring behavior can only be reactivated by restarting Revit.",
                    yesNo: true);

                // If affirmative
                if (formResult.Affirmative)
                {
                    // Register colouring of tabs
                    ColouredTabs.Register();
                    Globals.ColouringTabs = true;

                    // Set the icons
                    pushButton.Image = gFil.GetImageSource("Testing_ColourTabs", resolution: 16, suffix: "_On");
                    pushButton.LargeImage = gFil.GetImageSource("Testing_ColourTabs", resolution: 32, suffix: "_On");

                    // Run the tab colouring routine
                    ColouredTabs.ColorTabs();
                }
            }

            // Return success
            return Result.Succeeded;
        }
    }

    #endregion
}