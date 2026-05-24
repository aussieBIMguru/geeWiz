// Revit API
using Autodesk.Revit.UI;
// geeWiz
using geeWiz.Extensions;
using System.IO;
using System.Windows.Media.Imaging;
using gDat = geeWiz.Utilities.Data_Utils;
using gFil = geeWiz.Utilities.File_Utils;

// The class belongs to the geeWiz namespace
// using gRib = geeWiz.Utilities.Ribbon_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Static methods container related to the Ribbon.
    /// </summary>
    public static class Ribbon_Utils
    {
        /// <summary>
        /// If we are in dark mode currently.
        /// </summary>
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
                ToolTip = gDat.GetDictValue(Globals.Tooltips, baseName),
                LargeImage = GetImageSource(baseName, resolution: 32),
                Image = GetImageSource(baseName, resolution: 16)
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
                ToolTip = gDat.GetDictValue(Globals.Tooltips, baseName),
                LargeImage = GetImageSource(baseName, resolution: 32),
                Image = GetImageSource(baseName, resolution: 16)
            };

            // Return the data
            return pulldownButtonData;
        }

        #endregion

        #region Special buttons

        /// <summary>
        /// Creates the UiToggle button.
        /// </summary>
        /// <typeparam name="CommandClass">The related Command class.</typeparam>
        /// <param name="pulldownButton">The PulldownButton to add it to.</param>
        /// <param name="availability">The availability string.</param>
        /// <returns>Void (nothing).</returns>
        public static void AddButton_UiToggle<CommandClass>(PulldownButton pulldownButton, string availability)
        {
            // Add Dark/Light mode if in 2024 or higher
#if REVIT2020 || REVIT2021 || REVIT2022 || REVIT2023
#else
            // Set dark mode global variable
            DARKMODE = UIThemeManager.CurrentTheme == UITheme.Dark;

            // Add UiToggle button
            pulldownButton.Ext_AddPushButton<CommandClass>(
                buttonName: DARKMODE ? "Light mode" : "Dark mode",
                availability: availability,
                suffix: DARKMODE ? "" : "_Dark");
#endif

            // Return either way
            return;
        }

        #endregion

        #region Get image resources

        /// <summary>
        /// Prepares an image source from a Png resource.
        /// </summary>
        /// <param name="iconName">The name of the icon (without format, resolution).</param>
        /// <param name="resolution">The resolution suffix (16 or 32, typically).</param>
        /// <param name="suffix">An additional suffix (optional).</param>
        /// <returns>An ImageSource object.</returns>
        public static System.Windows.Media.ImageSource GetImageSource(string iconName, int resolution = 32, string suffix = "")
        {
            // Construct the resource path
            string resourcePath = $"{Globals.AddinName}.Resources.Icons{resolution}.{iconName}{resolution}{suffix}.png";

            // Read the resource from its full path
            using (Stream stream = Globals.Assembly.GetManifestResourceStream(resourcePath))
            {
                // Throw exception if stream not made
                if (stream == null)
                {
                    return null;
                }

                // Decode the png resource
                PngBitmapDecoder decoder = new System.Windows.Media.Imaging.PngBitmapDecoder(stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);

                // Decode to image source
                return decoder.Frames[0];
            }
        }

        #endregion
    }
}