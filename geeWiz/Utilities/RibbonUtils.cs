// System
using System.Diagnostics;
// Revit API
using Autodesk.Revit.UI;
// geeWiz
using gFil = geeWiz.Utilities.FileUtils;

// The class belongs to the geeWiz namespace
// using gRib = geeWiz.Utilities.RibbonUtils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to ribbon setup.
    /// (Most ribbon construction is under extension methods).
    /// </summary>
    public static class RibbonUtils
    {
        /// <summary>
        /// Converts a command class to a base name for tooltip/icon finding.
        /// </summary>
        /// <param name="commandClass">The name of the command class.</param>
        /// <returns>A string.</returns>
        public static string CommandClassToBaseName(string commandClass)
        {
            return commandClass.Replace("geeWiz.", "").Replace(".Cmd", "");
        }

        /// <summary>
        /// Creates PushButtonData (to stack, generally).
        /// </summary>
        /// <param name="buttonName">The name for the button.</param>
        /// <param name="commandClass">The command class path.</param>
        /// <returns>A PushButtonData object.</returns>
        public static PushButtonData NewPushButtonData(string buttonName, string commandClass)
        {
            // Strip the command class name to basics
            string baseName = CommandClassToBaseName(commandClass);

            // Make pushbuttondata
            PushButtonData pushButtonData = new PushButtonData(baseName, buttonName, Globals.AssemblyPath, commandClass);

            // Set tooltip and icons
            pushButtonData.ToolTip = gFil.GetTooltip(baseName);
            pushButtonData.LargeImage = gFil.GetImageSource(baseName, resolution: 32);
            pushButtonData.Image = gFil.GetImageSource(baseName, resolution: 16);

            // Return the data
            return pushButtonData;
        }

        /// <summary>
        /// Creates PulldownButtonData (to stack, generally).
        /// </summary>
        /// <param name="buttonName">The name for the button.</param>
        /// <param name="commandClass">The command class path.</param>
        /// <returns>A PulldownButtonData object.</returns>
        public static PulldownButtonData NewPulldownButtonData(string buttonName, string commandClass)
        {
            // Strip the command class name to basics
            string baseName = CommandClassToBaseName(commandClass);

            // Make pushbuttondata
            PulldownButtonData pulldownButtonData = new PulldownButtonData(baseName, buttonName);

            // Set tooltip and icons
            pulldownButtonData.ToolTip = gFil.GetTooltip(baseName);
            pulldownButtonData.LargeImage = gFil.GetImageSource(baseName, resolution: 32);
            pulldownButtonData.Image = gFil.GetImageSource(baseName, resolution: 16);

            // Return the data
            return pulldownButtonData;
        }
    }
}