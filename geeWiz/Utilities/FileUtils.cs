using Autodesk.Revit.UI;
using System.Diagnostics;

namespace geeWiz.Utilities
{
    internal class FileUtils
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
    }
}
