// System
using System.IO;
using System.Windows.Media.Imaging;
// Revit API
using Autodesk.Revit.UI;
// geeWiz libraries
using gScr = geeWiz.Utilities.ScriptUtils;

// The class belongs to the geeWiz namespace
// using gRib = geeWiz.Utilities.RibbonUtils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to ribbon setup.
    /// </summary>
    public static class RibbonUtils
    {
        /// <summary>
        /// Creates a ribbon.
        /// </summary>
        // TBA

        /// <summary>
        /// Creates a panel.
        /// </summary>
        // TBA

        /// <summary>
        /// Creates data for a pushdownbutton.
        /// </summary>
        /// <param name="commandName">The command name for the button.</param>
        /// <param name="buttonName">The name for the button.</param>
        /// <param name="iconName">The icon resopurce name.</param>
        /// <param name="commandClass">The icon resopurce name.</param>
        /// <param name="assemblypath">The icon resopurce name.</param>
        /// <returns>A pushdownbuttondata object.</returns>
        public static PushButtonData PushButtonDataToStack(string commandName, string buttonName, string iconName, string commandClass, string assemblypath)
        {
            // Construct pushbuttondata object
            var pushButtonData = new PushButtonData(commandName, buttonName, assemblypath, commandClass);

            // Set the tooltip and large/small image icons
            pushButtonData.ToolTip = gScr.GetTooltip(commandName);
            pushButtonData.LargeImage = PngImageSource($"geeWiz.Resources.Icons.{iconName}32.png");
            pushButtonData.Image = PngImageSource($"geeWiz.Resources.Icons.{iconName}16.png");

            // Return the pushbuttondata object
            return pushButtonData;
        }

        /// <summary>
        /// Creates a pushbutton.
        /// </summary>
        // TBA

        /// <summary>
        /// Converts a PNG resource into an imagesource.
        /// </summary>
        /// <param name="resourcePath">The full resource path.</param>
        /// <returns>An ImageSource object.</returns>
        private static System.Windows.Media.ImageSource PngImageSource(string resourcePath)
        {
            // Read the resource from its full path
            using (Stream stream = Globals.Assembly.GetManifestResourceStream(resourcePath))
            {
                // If no stream, we didn't find it - throw exception
                if (stream == null)
                {
                    throw new ArgumentException($"Resource not found: {resourcePath}");
                }

                // Decode the png resource
                var decoder = new System.Windows.Media.Imaging.PngBitmapDecoder(stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);

                // Get the imagesource from the decoder
                return decoder.Frames[0];
            }
        }
    }
}