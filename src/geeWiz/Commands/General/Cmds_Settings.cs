// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
// geeWiz
using geeWiz.Extensions;
using gFrm = geeWiz.Forms;
using gFil = geeWiz.Utilities.File_Utils;
using gScr = geeWiz.Utilities.Script_Utils;

// The class belongs to the Commands namespace
namespace geeWiz.Cmds_Settings
{
    #region Cmd_Warden

    /// <summary>
    /// Toggles light and dark mode (Revit 2024+).
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_Warden : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Action to report
            string action = null;
            
            // Path 1 - Warden is active
            if (Globals.WardenActive)
            {
                // Message to user
                var formResult = gFrm.Custom.Message(title: "Warden",
                    message: "Disable warden?\n\n" +
                    "Commands will become unmonitored:\n" +
                    "- Model in place\n" +
                    "- Import CAD\n" +
                    "- Paint tools",
                    yesNo: true);

                // Check for confirmation
                if (formResult.Affirmative)
                {
                    // Deregister Warden
                    Warden.DeRegister(Globals.UiCtlApp);
                    Globals.WardenActive = false;
                    action = "disabled";
                }
            }
            // Path 2 - Warden is inactive
            else
            {
                // Message to user
                var formResult = gFrm.Custom.Message(title: "Warden",
                    message: "Enable warden?\n\n" +
                    "Commands will become monitored:\n" +
                    "- Model in place\n" +
                    "- Import CAD\n" +
                    "- Paint tools",
                    yesNo: true);

                // Check for confirmation
                if (formResult.Affirmative)
                {
                    // Register Warden
                    Warden.Register(Globals.UiCtlApp);
                    Globals.WardenActive = true;
                    action = "enabled";
                }
            }

            // Return message to user
            if (action is null)
            {
                return Result.Cancelled;
            }
            else
            {
                return gFrm.Custom.BubbleMessage(
                    title: "Task completed",
                    message: $"Warden {action}.\n\nClick me to see how this works!",
                    linkPath: @"https://github.com/aussieBIMguru/geeWiz/blob/main/geeWiz/General/Warden.cs");
            }
        }
    }

    #endregion

    #region Cmd_ColourTabs

    /// <summary>
    /// Toggles tab colouring (off by default).
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_ColourTabs : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the PushButton on the PulldownButton
            var ribbonPanel1 = Globals.UiCtlApp.Ext_GetRibbonPanelByName(geeWiz.Application.PANEL1_NAME);
            var pulldownSettings = ribbonPanel1.Ext_GetPulldownButtonByName("Settings");
            var pushButton = pulldownSettings.Ext_GetPushButtonByText("Coloured Tabs");
            string action = null;

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
                    action = "disabled";

                    // Set the icons
                    pushButton.Image = gFil.GetImageSource("Settings_ColourTabs", resolution: 16, suffix: "");
                    pushButton.LargeImage = gFil.GetImageSource("Settings_ColourTabs", resolution: 32, suffix: "");
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
                    action = "enabled";

                    // Set the icons
                    pushButton.Image = gFil.GetImageSource("Settings_ColourTabs", resolution: 16, suffix: "_On");
                    pushButton.LargeImage = gFil.GetImageSource("Settings_ColourTabs", resolution: 32, suffix: "_On");

                    // Run the tab colouring routine
                    ColouredTabs.ColorTabs();
                }
            }

            // Return message to user
            if (action is null)
            {
                return Result.Cancelled;
            }
            else
            {
                // Return message to user
                return gFrm.Custom.BubbleMessage(
                    title: "Task completed",
                    message: $"Tab colouring {action}.\n\nClick me to see how this works!",
                    linkPath: @"https://github.com/aussieBIMguru/geeWiz/blob/main/geeWiz/General/ColouredTabs.cs");
            }
        }
    }

    #endregion

    #region Cmd_UiToggle

    /// <summary>
    /// Toggles light and dark mode (Revit 2024+).
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_UiToggle : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            #if REVIT2024_OR_GREATER

            // Get the button name and icon suffix
            string oldButtonName = Globals.IsDarkMode ? "Light mode" : "Dark mode";
            string newButtonName = Globals.IsDarkMode ? "Dark mode" : "Light mode";
            string iconSuffix = Globals.IsDarkMode ? "_Dark" : "";

            // Get the PushButton on the PulldownButton
            var ribbonPanel1 = Globals.UiCtlApp.Ext_GetRibbonPanelByName("General");
            var pulldownSettings = ribbonPanel1.Ext_GetPulldownButtonByName("Settings");
            var pushButton = pulldownSettings.Ext_GetPushButtonByText(oldButtonName);

            // Set the new button properties
            pushButton.ItemText = newButtonName;
            pushButton.Image = gFil.GetImageSource("Settings_UiToggle", resolution: 16, suffix: iconSuffix);
            pushButton.LargeImage = gFil.GetImageSource("Settings_UiToggle", resolution: 32, suffix: iconSuffix);

            // Switch the UITheme and canvas theme (always light)
            UIThemeManager.CurrentTheme = Globals.IsDarkMode ? UITheme.Light : UITheme.Dark;
            UIThemeManager.CurrentCanvasTheme = gScr.KeyHeldShift() ? UITheme.Dark : UITheme.Light;
            Globals.IsDarkMode = !Globals.IsDarkMode;

            // Return message to user
            return gFrm.Custom.BubbleMessage(title: "Task completed",
                message: $"Revit theme set to {oldButtonName}.\n\nClick me to see how this works!",
                linkPath: @"https://github.com/aussieBIMguru/geeWiz/blob/main/geeWiz/Commands/General/Cmds_Settings.cs#L171");

            #else

            return Result.Failed;

            #endif
        }
    }

    #endregion
}