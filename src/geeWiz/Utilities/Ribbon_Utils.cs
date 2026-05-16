// Revit API
using Autodesk.Revit.UI;
// geeWiz
using geeWiz.Extensions;
using System.Xaml;
using gFil = geeWiz.Utilities.File_Utils;

// The class belongs to the geeWiz namespace
// using gRib = geeWiz.Utilities.Ribbon_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to ribbon setup.
    /// (Most ribbon construction is under extension methods).
    /// </summary>
    public static class Ribbon_Utils
    {
        public static bool DARKMODE = false;
        
        #region Command class to base name

        /// <summary>
        /// Converts a command class to a base name for tooltip/icon finding.
        /// </summary>
        /// <param name="commandClass">The name of the command class.</param>
        /// <returns>A string.</returns>
        public static string CommandClassToBaseName(string commandClass)
        {
            // Example: geeWiz.Commands.Cmds_Settings.Cmd_UiToggle
            // Step 1: Settings.Cmd_UiToggle
            // Step 2: Settings_UiToggle
            return commandClass.Replace($"{Globals.AddinName}.Commands.Cmds_", "").Replace(".Cmd", "");
        }

        #endregion

        #region Button data

        /// <summary>
        /// Creates PushButtonData (to stack, generally).
        /// </summary>
        /// <typeparam name="CommandClass">The related Command class.</typeparam>
        /// <param name="buttonName">The name for the button.</param>
        /// <returns>A PushButtonData object</returns>
        public static PushButtonData NewPushButtonData<CommandClass>(string buttonName)
        {
            // Strip the command class name to basics
            string commandClass = typeof(CommandClass).FullName;
            string baseName = CommandClassToBaseName(commandClass);

            // Make pushbuttondata
            var pushButtonData = new PushButtonData(baseName, buttonName, Globals.AssemblyPath, commandClass)
            {
                ToolTip = gFil.GetDictValue(Globals.Tooltips, baseName),
                LargeImage = gFil.GetImageSource(baseName, resolution: 32),
                Image = gFil.GetImageSource(baseName, resolution: 16)
            };

            // Return the data
            return pushButtonData;
        }

        /// <summary>
        /// Creates PulldownButtonData (to stack, generally).
        /// </summary>
        /// <param name="buttonName">The name for the button.</param>
        /// <param name="nameSpace">The namespace the commands relate to.</param>
        /// <returns>A PulldownButtonData object.</returns>
        public static PulldownButtonData NewPulldownButtonData(string buttonName, string nameSpace)
        {
            // Strip the command class name to basics
            string baseName = CommandClassToBaseName(nameSpace);

            // Make pushbuttondata
            var pulldownButtonData = new PulldownButtonData(baseName, buttonName)
            {
                ToolTip = gFil.GetDictValue(Globals.Tooltips, baseName),
                LargeImage = gFil.GetImageSource(baseName, resolution: 32),
                Image = gFil.GetImageSource(baseName, resolution: 16)
            };

            // Return the data
            return pulldownButtonData;
        }

        #endregion

        #region Special buttons

        /// <summary>
        /// Creates the UiToggle button (Revit 2024+).
        /// </summary>
        /// <param name="pulldownButton">The PulldownButton to add it to.</param>
        /// <param name="commandClass">The command class path.</param>
        /// <param name="availability">The availability string.</param>
        /// <returns>Void (nothing).</returns>
        public static void AddButton_UiToggle<T>(PulldownButton pulldownButton, string availability)
        {
            // Add Dark/Light mode if in 2024 or higher
#if REVIT2020 || REVIT2021 || REVIT2022 || REVIT2023
#else
            // Set dark mode global variable
            DARKMODE = UIThemeManager.CurrentTheme == UITheme.Dark;

            // Add UiToggle button
            pulldownButton.Ext_AddPushButton<T>(
                buttonName: DARKMODE ? "Light mode" : "Dark mode",
                availability: availability,
                suffix: DARKMODE ? "" : "_Dark");
#endif

            // Return either way
            return;
        }

        #endregion
    }
}