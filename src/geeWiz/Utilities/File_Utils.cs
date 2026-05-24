// System
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using Form = System.Windows.Forms.Form;
// Revit API
using Autodesk.Revit.UI;
// geeWiz
using gStr = geeWiz.Utilities.String_Utils;
using geeWiz.Extensions;

// The class belongs to the utility namespace
// using gFil = geeWiz.Utilities.File_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Static methods container related to Files.
    /// </summary>
    public static class File_Utils
    {
        #region Access links

        /// <summary>
        /// Used to verify if a URL is valid (will open).
        /// </summary>
        /// <param name="linkPath">The path, typically a URL.</param>
        /// <returns>A Boolean.</returns>
        public static bool LinkIsAccessible(string linkPath)
        {
            return Uri.TryCreate(linkPath, UriKind.Absolute, out Uri uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp
                   || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Attempts to open a link in the default browser.
        /// </summary>
        /// <param name="linkPath">The path, typically a URL.</param>
        /// <returns>A Result.</returns>
        public static Result OpenLinkPath(string linkPath)
        {
            if (LinkIsAccessible(linkPath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo { FileName = linkPath, UseShellExecute = true });
                    return Result.Succeeded;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while trying to open the URL: {ex.Message} ({linkPath})");
                    return Result.Cancelled;
                }
            }
            else
            {
                Console.WriteLine($"ERROR: Link path could not be opened ({linkPath})");
                return Result.Cancelled;
            }
        }

        #endregion

        #region Access files

        /// <summary>
        /// Runs an accessibility check on a file path.
        /// </summary>
        /// <param name="filePath">The path.</param>
        /// <returns>A Boolean.</returns>
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
                using (var stream = new FileStream(filePath,
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
        /// <param name="filePath">The file path.</param>
        /// <returns>A Result.</returns>
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
                return Result.Cancelled;
            }
        }

        #endregion

        #region Read files

        /// <summary>
        /// Returns the contents of a file, by row.
        /// </summary>
        /// <param name="filePath">The file path to read.</param>
        /// <param name="skipEmpty">Do not write empty rows.</param>
        /// <returns>A list of strings.</returns>
        public static List<string> ReadFileAsRows(string filePath, bool skipEmpty = false)
        {
            // List of strings to return
            var rows = new List<string>();

            // Try to read the file...
            try
            {
                // Using a stream reader
                using (var reader = new StreamReader(filePath))
                {
                    // While we have more rows to read
                    while (!reader.EndOfStream)
                    {
                        // Read the line
                        string line = reader.ReadLine();
                        
                        // If the row is not empty, add it
                        if (line.Ext_HasChars())
                        {
                            rows.Add(line);
                        }

                        // If it isn't, and we don't skip, add empty
                        else if (!skipEmpty)
                        {
                            rows.Add(string.Empty);
                        }
                    }
                }
            }
            // Report exception
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to read the file: {ex.Message}");
            }

            // Return the list of strings (rows)
            return rows;
        }

        /// <summary>
        /// Returns the contents of a file, as a matrix.
        /// </summary>
        /// <param name="filePath">The file path to read.</param>
        /// <param name="separator">The separator string.</param>
        /// <param name="skipEmpty">Do not write empty rows.</param>
        /// <returns>A matrix of strings.</returns>
        public static List<List<string>> ReadFileAsMatrix(string filePath, string separator = ",", bool skipEmpty = false)
        {
            // List of list of strings to return
            var dataList = ReadFileAsRows(filePath, skipEmpty: skipEmpty);

            // Return as matrix
            return gStr.ListToMatrix(dataList, separator);
        }

        #endregion

        # region Read Resources

        /// <summary>
        /// Returns the contents of a resource, by row.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="resourceFolderName">The folder it is in.</param>
        /// <param name="skipEmpty">Do not write empty rows.</param>
        /// <returns>A list of strings.</returns>
        public static List<string> ReadResourceAsRows(string resourceName, string resourceFolderName = "Files", bool skipEmpty = false)
        {
            // List of strings to return
            var rows = new List<string>();

            // Construct the full resource name
            string fullResourceName = $"geeWiz.Resources.{resourceFolderName}.{resourceName}";

            // Try to read the resource...
            try
            {
                // Using a stream reader...
                using (Stream stream = Globals.Assembly.GetManifestResourceStream(fullResourceName))
                {
                    // If there is a stream
                    if (stream is not null)
                    {
                        // Using that stream
                        using (var reader = new StreamReader(stream))
                        {
                            // While we have more rows to read
                            while (!reader.EndOfStream)
                            {
                                // Read the line
                                string line = reader.ReadLine();

                                // If the row is not empty, add it
                                if (line.Ext_HasChars())
                                {
                                    rows.Add(line);
                                }

                                // If it isn't, and we don't skip, add empty
                                else if (!skipEmpty)
                                {
                                    rows.Add(string.Empty);
                                }
                            }
                        }
                    }
                }
            }
            // Report exception
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to read the file: {ex.Message}");
            }

            // Return the list of strings (rows)
            return rows;
        }

        /// <summary>
        /// Returns the contents of a resource, as a matrix.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="resourceFolderName">The folder it is in.</param>
        /// <param name="separator">The separator string.</param>
        /// <param name="skipEmpty">Do not write empty rows.</param>
        /// <returns>A matrix of strings.</returns>
        public static List<List<string>> ReadResourceAsMatrix(string resourceName, string resourceFolderName = "Files",
            string separator = ",", bool skipEmpty = false)
        {
            // List of list of strings to return
            var dataList = ReadResourceAsRows(resourceName, resourceFolderName, skipEmpty: skipEmpty);

            // Return as matrix
            return gStr.ListToMatrix(dataList, separator);
        }

        #endregion

        #region Write to files

        /// <summary>
        /// Writes a list of strings to a file.
        /// </summary>
        /// <param name="filePath">The file path to read.</param>
        /// <param name="dataRows">A list of strings to write.</param>
        /// <returns>A Result.</returns>
        public static Result WriteListToFile(string filePath, List<string> dataRows)
        {
            // Make sure file path is valid
            if (filePath is null || !FileIsAccessible(filePath))
            {
                return Result.Cancelled;
            }

            // Write to the file as list
            try
            {
                using (var writer = new StreamWriter(filePath, false))
                {
                    foreach (string row in dataRows)
                    {
                        writer.WriteLine(row);
                    }
                }
            }
            // Report exception
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to write file: {ex.Message}");
                return Result.Cancelled;
            }

            // Return success
            return Result.Succeeded;
        }

        /// <summary>
        /// Applies the standard form logo
        /// </summary>
        /// <param name="form">The form to set the icon for.</param>
        public static void SetFormIcon(Form form)
        {
            string iconPath = "geeWiz.Resources.Icons16.IconList16.ico";

            using (Stream stream = Globals.Assembly.GetManifestResourceStream(iconPath))
            {
                if (stream != null)
                {
                    form.Icon = new Icon(stream);
                }
            }
        }

        #endregion

        #region Open directory

        /// <summary>
        /// Attempts to open a directory.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns>A Result.</returns>
        public static Result OpenDirectory(string directoryPath)
        {
            // Fail if it does not exist
            if (!Directory.Exists(directoryPath))
            {
                return Result.Cancelled;
            }

            // Try to open the directory with Explorer.exe
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", directoryPath);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Cancelled;
            }
        }

        #endregion
    }
}