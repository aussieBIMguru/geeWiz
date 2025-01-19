// System
using System.IO;
// Revit API
using Autodesk.Revit.UI;
// geeWiz
using gFil = geeWiz.Utilities.FileUtils;
using gFrm = geeWiz.Forms;
// ClosedXML
using ClosedXML.Excel;

// The class belongs to the utility namespace
// using gXcl = geeWiz.Utilities.ExcelUtils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to Excel based operations.
    /// </summary>
    public static class ExcelUtils
    {
        /// <summary>
        /// Checks that an Excel file can be accessed.
        /// </summary>
        /// <param name="filePath">The filepath of the Excel file.</param>
        /// <param name="worksheetName">A worksheet to check for.</param>
        /// <returns>A Result.</returns>
        public static Result FileIsAccessible(string filePath, string worksheetName = null)
        {
            // Make sure the file exists
            if (!File.Exists(filePath))
            {
                return gFrm.Custom.Cancelled("File could not be found.");
            }

            // Make sure the file can be read
            if (!gFil.FileIsAccessible(filePath))
            {
                return gFrm.Custom.Cancelled("File was found, but could not be accessed.\n\n" +
                    "Ensure the file is not opened by any users, then try again.");
            }

            // Check for worksheet if provided
            if (worksheetName is not null)
            {
                // Get workbook
                var xclWorkbook = GetWorkbook(filePath);

                // Get the worksheet, report an error if we could not
                if (GetWorkSheet(xclWorkbook, worksheetName) is not null)
                {
                    return gFrm.Custom.Cancelled($"Worksheet '{worksheetName}' not be found in workbook.");
                }
            }
            
            // We are able to proceed otherwise
            return Result.Succeeded;
        }

        /// <summary>
        /// Gets a Workbook object from a filepath.
        /// </summary>
        /// <param name="filePath">The filepath of the Excel file.</param>
        /// <returns>A ClosedXML workbook object (null if not found).</returns>
        public static XLWorkbook GetWorkbook(string filePath)
        {
            // Try to open the workbook
            try
            {
                return new XLWorkbook(filePath);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a Workbook object from a filepath. If it exists, returns the existing one.
        /// </summary>
        /// <param name="filePath">The filepath of the Excel file.</param>
        /// <returns>A ClosedXML workbook object.</returns>
        public static XLWorkbook CreateWorkbook(string filePath)
        {
            // Check if the file exists
            if (File.Exists(filePath))
            {
                // If it does, return the workbook
                return new XLWorkbook(filePath);
            }
            else
            {
                // Otherwise, create a new workbok object (unsaved)
                return new XLWorkbook();
            }
        }

        /// <summary>
        /// Gets a worksheet by name from a workbook.
        /// </summary>
        /// <param name="workbook">A closedXML workbook object.</param>
        /// <param name="worksheetName">A worksheet name to find.</param>
        /// <param name="getFirstOtherwise">Return first worksheet if not found.</param>
        /// <returns>An IXLWorksheet object.</returns>
        public static IXLWorksheet GetWorkSheet(XLWorkbook workbook, string worksheetName, bool getFirstOtherwise = false)
        {
            // Null check for workbook
            if (workbook is null) { return null; }

            // Check each worksheet
            foreach (var worksheet in workbook.Worksheets)
            {
                // If a name matches, return the worksheet
                if (worksheet.Name == worksheetName)
                {
                    return worksheet;
                }
            }

            // Option to get first worksheet otherwise
            if (getFirstOtherwise)
            {
                return workbook.Worksheets.First();
            }

            // Return null if we did not get a worksheet
            return null;
        }

        /// <summary>
        /// Creates a Worksheet in a workbook. If it exists, returns the existing one.
        /// </summary>
        /// <param name="workbook">A closedXML workbook object.</param>
        /// <param name="worksheetName">A worksheet name to find.</param>
        /// <returns>An IXLWorksheet object.</returns>
        public static IXLWorksheet AddWorksheet(XLWorkbook workbook, string worksheetName)
        {
            // Catch if workbook is null
            if (workbook is null) { return null; }
            
            // Try to get the worksheet
            if (GetWorkSheet(workbook, worksheetName) is IXLWorksheet worksheet)
            {
                return worksheet;
            }
            else
            {
                // Try to add the worksheet if not found
                try
                {
                    return workbook.Worksheets.Add(worksheetName);
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets all worksheet names from a workbook.
        /// </summary>
        /// <param name="workbook">A closedXML workbook object.</param>
        /// <returns>A list of Strings.</returns>
        public static List<string> GetWorksheetNames(XLWorkbook workbook)
        {
            // Default list to add names to
            var names = new List<string>();

            // Early null workbook check
            if (workbook is null) { return names; }

            // Return worksheet names
            return workbook.Worksheets
                .Select(w => w.Name)
                .ToList();
        }

        /// <summary>
        /// Reads a matrix of strings from a worksheet.
        /// </summary>
        /// <param name="worksheet">A worksheet object to read from.</param>
        /// <param name="readRows">Number of rows to read (entire range if left as 0).</param>
        /// <param name="readCols">Number of columns to read (entire range if left as 0).</param>
        /// <returns>A list of list of strings.</returns>
        public static List<List<string>> ReadFromWorksheet(IXLWorksheet worksheet, int readRows = 0, int readCols = 0)
        {
            // Matrix to construct
            var matrix = new List<List<string>>();

            // Catch null worksheet
            if (worksheet is null) { return matrix; }

            // Get range used, ensure we aren't reading more than what is available
            var rangeUsed = worksheet.RangeUsed();
            if (rangeUsed.RowCount() <= readRows) { readRows = 0; }
            if (rangeUsed.ColumnCount() <= readCols) { readCols = 0; }

            // Track rows read
            int rowsRead = 0;

            // For each row
            foreach (var row in rangeUsed.RowsUsed())
            {
                var dataList = new List<string>();
                
                // If we can still read, or if read all
                if (rowsRead < readRows || readRows == 0)
                {
                    // Track columns read
                    int colsRead = 0;

                    // For each cell in the row
                    foreach (var cell in row.Cells())
                    {
                        // If we can still read, or if read all
                        if (colsRead < readCols || colsRead == 0)
                        {
                            // Add the cell text to the list
                            dataList.Add(cell.GetString().Trim());
                        }
                        colsRead++; // Next column
                    }
                }
                // Add the row to the matrix
                matrix.Add(dataList);

                rowsRead++; // Next row
            }

            // Return the matrix
            return matrix;
        }

        /// <summary>
        /// Writes matrix of strings to a worksheet.
        /// </summary>
        /// <param name="worksheet">A worksheet object to write to.</param>
        /// <param name="matrix">A matrix of strings.</param>
        /// <param name="startRow">Begin at row (starts from 1).</param>
        /// <param name="startCol">Begin at column (starts from 1).</param>
        /// <returns>Void (nothing).</returns>
        public static void WriteToWorksheet(IXLWorksheet worksheet, List<List<string>> matrix, int startRow = 1, int startCol = 1)
        {
            // Catch null worksheet
            if (worksheet is null) { return; }
            
            // For each row (r), offset by startRow
            for (int r = startRow; r < matrix.Count + startRow; r++)
            {
                // For each column in that row (c), offset by startCol
                for (int c = startCol; c < matrix[r].Count + startCol; c++)
                {
                    // Get the matrix value
                    var cellValue = matrix[r - 1][c - 1];

                    // Write to the cell if its value is not null
                    if (cellValue is not null)
                    {
                        worksheet.Cell(r, c).Value = cellValue;
                    }
                }
            }
        }
    }
}
