// System
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
// Revit API
using Autodesk.Revit.UI;

// The class belongs to the utility namespace
// using gFil = geeWiz.Utilities.FileUtils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to file based operations.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Used to verify if a URL is valid (will open).
        /// </summary>
        /// <param name="linkPath"">The path, typically a URL.</param>
        /// <returns>A boolean.</returns>
        public static bool CheckLinkPath(string linkPath)
        {
            return Uri.TryCreate(linkPath, UriKind.Absolute, out Uri uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp
                   || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Attempts to open a link in the default browser.
        /// </summary>
        /// <param name="linkPath"">The path, typically a URL.</param>
        /// <returns>A result.</returns>
        public static Result OpenLinkPath(string linkPath)
        {
            if (CheckLinkPath(linkPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo { FileName = linkPath, UseShellExecute = true });
                    return Result.Succeeded;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while trying to open the URL: {ex.Message} ({linkPath})");
                    return Result.Failed;
                }
            }
            else
            {
                Console.WriteLine($"ERROR: Link path could not be opened ({linkPath})");
                return Result.Failed;
            }
        }

        /// <summary>
        /// Runs an accessibility check on a file path.
        /// </summary>
        /// <param name="filePath"">The path.</param>
        /// <returns>A boolean.</returns>
        public static bool FileIsAccessible(string filePath)
        {
            // If the file doesn't exist, we return true (to allow creation)
            if (!File.Exists(filePath))
            {
                return true;
            }

            // Try to open the file with exclusive access
            try
            {
                using (FileStream stream = new FileStream(filePath,
                    FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    // If we managed to run a stream, we can just return true
                    return true;
                }
            }
            // Otherwise the file was not accessible
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to open a file path.
        /// </summary>
        /// <param name="filePath"">The file path.</param>
        /// <returns>A result.</returns>
        public static Result OpenFilePath(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = filePath, UseShellExecute = true });
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: File path could not be opened {ex.Message} ({filePath})");
                return Result.Failed;
            }
        }

        /// <summary>
        /// Applies the standard form logo
        /// </summary>
        /// <param name="form"">The form to set the icon for.</param>
        /// <returns>Void (nothing).</returns>
        public static void SetFormIcon(System.Windows.Forms.Form form)
        {
            string iconPath = "gSharp.Resources.Icons.IconList16";

            using (Stream stream = Globals.Assembly.GetManifestResourceStream(iconPath))
            {
                if (stream != null)
                {
                    form.Icon = new Icon(stream);
                }
            }
        }

        /// <summary>
        /// Prepares an image source from a Png resource.
        /// </summary>
        /// <param name="iconName"">The name of the icon (without format, resolution).</param>
        /// <param name="resolution"">The resolution suffix (16 or 32, typically).</param>
        /// <param name="suffix"">An additional suffix (optional).</param>
        /// <returns>An ImageSource object.</returns>
        public static System.Windows.Media.ImageSource GetImageSource(string iconName, int resolution = 32, string suffix = "")
        {
            // Construct the resource path
            var resourcePath = $"geeWiz.Resources.Icons{resolution}.{iconName}{resolution}{suffix}.png";

            // Read the resource from its full path
            using (Stream stream = Globals.Assembly.GetManifestResourceStream(resourcePath))
            {
                // Throw exception if stream not made
                if (stream == null)
                {
                    throw new ArgumentException($"Png resource not found: {resourcePath}");
                }

                // Decode the png resource
                var decoder = new System.Windows.Media.Imaging.PngBitmapDecoder(stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);

                // Decode to image source
                return decoder.Frames[0];
            }
        }

        /// <summary>
        /// Retrieve a tooltip value by key.
        /// </summary>
        /// <param name="key">The key value for the tooltip resource.</param>
        /// <returns>A string.</returns>
        public static string GetTooltip(string key)
        {
            if (Globals.Tooltips.TryGetValue(key, out string value))
            {
                return value;
            }
            return "Tooltip missing!";
        }
    }
}
