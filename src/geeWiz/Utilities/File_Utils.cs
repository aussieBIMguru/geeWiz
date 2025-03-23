// System
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using Form = System.Windows.Forms.Form;
// Revit API
using Autodesk.Revit.UI;

// The class belongs to the utility namespace
// using gFil = geeWiz.Utilities.File_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to file based operations.
    /// </summary>
    public static class File_Utils
    {
        #region Access links

        /// <summary>
        /// Used to verify if a URL is valid (will open).
        /// </summary>
        /// <param name="linkPath"">The path, typically a URL.</param>
        /// <returns>A boolean.</returns>
        public static bool LinkIsAccessible(string linkPath)
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
                    return Result.Failed;
                }
            }
            else
            {
                Console.WriteLine($"ERROR: Link path could not be opened ({linkPath})");
                return Result.Failed;
            }
        }

        #endregion

        #region Access files

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
                        // If the row is a string, add it
                        if (reader.ReadLine() is string rowString)
                        {
                            rows.Add(rowString);
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
        /// Returns the contents of a file, by row.
        /// </summary>
        /// <param name="filePath">The file path to read.</param>
        /// <param name="separator">The separator string.</param>
        /// <param name="skipEmpty">Do not write empty rows.</param>
        /// <returns>A list of strings.</returns>
        public static List<List<string>> ReadFileAsMatrix(string filePath, string separator = ",", bool skipEmpty = false)
        {
            // List of list of strings to return
            var matrix = new List<List<string>>();

            // Separator to array of strings
            var separators = new[] { separator };

            // Try to read the file...
            try
            {
                // Using a stream reader
                using (var reader = new StreamReader(filePath))
                {
                    // While we have more rows to read
                    while (!reader.EndOfStream)
                    {
                        // If the row is a string, split and add to matrix
                        if (reader.ReadLine() is string rowString)
                        {
                            var values = rowString.Split(separators, StringSplitOptions.None);
                            matrix.Add(new List<string>(values));
                        }
                        // If it isn't, and we don't skip, add empty list
                        else if (!skipEmpty)
                        {
                            matrix.Add(new List<string>());
                        }
                    }
                }
            }
            // Report exception
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to read file: {ex.Message}");
            }

            // Return the matrix
            return matrix;
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
                return Result.Failed;
            }

            // Write to the file as list
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
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
                return Result.Failed;
            }

            // Return success
            return Result.Succeeded;
        }

        /// <summary>
        /// Applies the standard form logo
        /// </summary>
        /// <param name="form"">The form to set the icon for.</param>
        /// <returns>Void (nothing).</returns>
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
        /// <param name="directoryPath"">The directory path.</param>
        /// <returns>A Result.</returns>
        public static Result OpenDirectory(string directoryPath)
        {
            // Fail if it does not exist
            if (!Directory.Exists(directoryPath))
            {
                return Result.Failed;
            }

            // Try to open the directory with Explorer.exe
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", directoryPath);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        #endregion

        #region Ribbon management

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
            var resourcePath = $"{Globals.AddinName}.Resources.Icons{resolution}.{iconName}{resolution}{suffix}.png";

            // Read the resource from its full path
            using (Stream stream = Globals.Assembly.GetManifestResourceStream(resourcePath))
            {
                // Throw exception if stream not made
                if (stream == null)
                {
                    return null;
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
        /// Returns a value based on a key from a dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary of keys/values to search.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="defaultValue">The value to return if no key is found.</param>
        /// <returns>The related tooltip, if found.</returns>
        public static string GetDictValue(Dictionary<string, string> dictionary, string key, string defaultValue = "Value not found.")
        {
            if (dictionary.TryGetValue(key, out string value))
            {
                return value;
            }
            return defaultValue;
        }

        #endregion
    }
}