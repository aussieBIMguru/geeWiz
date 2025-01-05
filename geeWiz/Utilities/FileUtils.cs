using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geeWiz.Utilities
{
    internal class FileUtils
    {
        /// <summary>
        /// Checks if a web link is valid.
        /// </summary>
        /// <param name="linkpath"">The path.</param>
        /// <returns>If the link was valid.</returns>
        public static bool UrlIsValid(string linkpath)
        {
            // Check if the URL is valid
            return Uri.TryCreate(linkpath, UriKind.Absolute, out Uri uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp
                   || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Tries to open a web link.
        /// </summary>
        /// <param name="linkpath"">The path.</param>
        /// <returns>If the link was valid.</returns>
        public static Result OpenUrl(string linkpath)
        {
            if (UrlIsValid(linkpath)) // Check url is valid
            {
                try // to open the link
                {
                    Process.Start(new ProcessStartInfo { FileName = linkpath, UseShellExecute = true });
                    return Result.Succeeded;
                }
                catch (Exception ex) // Link could not be opened
                {
                    Console.WriteLine($"An error occurred while trying to open the URL: {ex.Message}");
                    return Result.Failed;
                }
            }
            else // Link not valid
            {
                Console.WriteLine($"Invalid URL: {linkpath}");
                return Result.Failed;
            }
        }

        /// <summary>
        /// Tries to open a file.
        /// </summary>
        /// <param name="filePath"">The path to the file.</param>
        /// <returns>If the file was opened.</returns>
        public static Result OpenFile(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true // This is important for opening files
                });
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error launching Excel file: {ex.Message}");
                return Result.Failed;
            }
        }
    }
}
