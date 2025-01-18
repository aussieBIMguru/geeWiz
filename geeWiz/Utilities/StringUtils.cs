// System
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
// Revit API
using Autodesk.Revit.UI;

// The class belongs to the utility namespace
// using gStr = geeWiz.Utilities.StringUtils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to string based operations.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Joins each row of a matrix of strings, returning a list.
        /// </summary>
        /// <param name="matrix"">A list of list of strings.</param>
        /// <param name="separator"">A string to put between elements.</param>
        /// <returns>A list of strings.</returns>
        public static List<string> MatrixToList(List<List<string>> matrix, string separator = ",")
        {
            // New list to add each row to
            var rows = new List<string>();

            // For each row, join and add to the list
            foreach (var row in matrix)
            {
                rows.Add(string.Join(separator, row.ToArray()));
            }

            // Return the list of strings
            return rows;
        }

        /// <summary>
        /// Splits each string into a list, and constructs a matrix from that.
        /// </summary>
        /// <param name="dataRows"">A list of strings.</param>
        /// <param name="separator"">A string to split each row by.</param>
        /// <returns>A list of list of strings.</returns>
        public static List<List<string>> ListToMatrix(List<string> dataRows, string separator = ",")
        {
            // New matrix to add each split row to
            var matrix = new List<List<string>>();

            // Separator to array of strings
            var separators = new[] { separator };

            // For each row, split and add the list to the matrix
            foreach (var row in dataRows)
            {
                var values = row.Split(separators, StringSplitOptions.None);
                matrix.Add(new List<string>(values));
            }

            // Return the list of list of strings
            return matrix;
        }

        /// <summary>
        /// Returns a matrix with the provided list as one row.
        /// </summary>
        /// <param name="dataRows"">A list of strings.</param>
        /// <returns>A list of list of strings.</returns>
        public static List<List<string>> ListAsMatrix(List<string> dataRows)
        {
            return new List<List<string>>() { dataRows };
        }

        /// <summary>
        /// Fills the end of each list in a matrix to the longest list.
        /// </summary>
        /// <param name="matrix"">A list of list of strings.</param>
        /// <param name="padString"">Pad the lists with this string. If null, will pad the last string.</param>
        /// <returns>A list of list of strings.</returns>
        public static List<List<string>> PadMatrix(List<List<string>> matrix, string padString = "")
        {
            // New matrix to add each padded row to
            var newMatrix = new List<List<string>>();

            // Find max row length
            int maxLength = matrix.Select(l => l.Count).Max();

            // For each row in the old matrix
            foreach (var oldRow in matrix)
            {
                // New row, old row length
                var newRow = new List<string>();
                var rowLength = oldRow.Count;

                // We store the pad string, and make sure it isn't null
                var safePadString = padString;
                if (safePadString is null) { safePadString = ""; }

                // Check if we should pad with last string in list
                if (rowLength > 0 && padString is null)
                {
                    safePadString = oldRow.Last();
                }

                // For each object to max length
                for (int i = 0; i < maxLength; i++)
                {
                    // If we have items, add to new row
                    if (i < rowLength)
                    {
                        newRow.Add(oldRow[i]);
                    }
                    // If we don't, we pad
                    else
                    {
                        newRow.Add(safePadString);
                    }
                }

                // Add row to matrix
                matrix.Add(newRow);
            }

            // Return the list of list of strings
            return matrix;
        }

        /// <summary>
        /// Flips the columns and rows of a matrix of strings, padding if needed.
        /// </summary>
        /// <param name="matrix"">A list of list of strings.</param>
        /// <param name="padString"">Pad the lists with this string. If null, will pad the last string.</param>
        /// <returns>A list of list of strings.</returns>
        public static List<List<string>> FlipMatrix(List<List<string>> matrix, string padString = "")
        {
            // Pad matrix and get the row length
            matrix = PadMatrix(matrix, padString: padString);
            int columnCount = matrix.First().Count;
            int rowCount = matrix.First().Count;

            // New matrix to construct
            var newMatrix = new List<List<string>>();

            // For each column in the matrix...
            for (int c = 0; c < columnCount; c++)
            {
                // New row to add strings to
                var newRow = new List<string>();

                // For each row in the matrix...
                for (int r = 0; r < rowCount; r++)
                {
                    // Add the string on that row, in that column
                    newRow.Add(matrix[r][c]);
                }

                // Add row to matrix
                newMatrix.Add(newRow);
            }

            // Return the list of list of strings
            return newMatrix;
        }

        /// <summary>
        /// Replaces all nulls in a list of strings.
        /// </summary>
        /// <param name="dataRow"">A list of strings.</param>
        /// <param name="replaceString"">The string to replace nulls with.</param>
        /// <returns>A list of strings.</returns>
        public static List<string> ReplaceListNulls(List<string> dataRow, string replaceString = "")
        {
            // Return the list if no nulls
            if (!dataRow.Contains(null)) { return dataRow; }
            
            // New row to construct
            var newRow = new List<string>();

            // For each cell in row...
            foreach (var cell in dataRow)
            {
                // Add either the cell or replacement if null
                if (cell is null)
                {
                    newRow.Add(replaceString);
                }
                else
                {
                    newRow.Add(cell);
                }
            }

            // Return the new row
            return dataRow;
        }

        /// <summary>
        /// Replaces all nulls in a list of list of strings.
        /// </summary>
        /// <param name="matrix"">A list of list of strings.</param>
        /// <param name="replaceString"">The string to replace nulls with.</param>
        /// <returns>A list of list of strings.</returns>
        public static List<List<string>> ReplaceMatrixNulls(List<List<string>> matrix, string replaceString = "")
        {
            return matrix
                .Select(r => ReplaceListNulls(r, replaceString: replaceString))
                .ToList();
        }
    }

    /// <summary>
    /// A class to assess matrix properties for use.
    /// </summary>
    public class MatrixProperties
    {
        // Properties of the matrix
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public int ObjectCount { get; set; }
        public bool IsUniform { get; set; }
        public bool ContainsNulls { get; set; }

        // Matrix properties constructor from matrix
        public MatrixProperties(List<List<string>> matrix)
        {
            // Counts and uniformity verification
            this.RowCount = matrix.Count;
            this.ColumnCount = matrix.Select(x => x.Count).Max();
            this.ObjectCount = matrix.Select(x => x.Count).Sum();
            this.IsUniform = ObjectCount == RowCount * ColumnCount;

            // Set nulls as false
            this.ContainsNulls = false;

            // Check each row for nulls
            foreach (var row in matrix)
            {
                if (row.Contains(null))
                {
                    this.ContainsNulls = true;
                    break;
                }
            }
        }
    }
}