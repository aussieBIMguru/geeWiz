// Autodesk
using Autodesk.Revit.UI;
// geeWiz
using gView = geeWiz.Utilities.View_Utils;
using gStr = geeWiz.Utilities.String_Utils;

// The class belongs to the extensions namespace
// ViewSheet viewSheet.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to sheets.
    /// </summary>
    public static class ViewSheet_Ext
    {
        #region Name keys

        /// <summary>
        /// Constructs a name key based on a Revit sheet.
        /// </summary>
        /// <param name="sheet">A Revit Sheet.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToSheetKey(this ViewSheet sheet, bool includeId = false)
        {
            // Null catch
            if (sheet is null) { return "???"; }
            
            // Return key with Id
            if (includeId)
            {
                return $"{sheet.SheetNumber}: {sheet.Name} [{sheet.Id.ToString()}]";
            }
            // Return key without Id
            else
            {
                return $"{sheet.SheetNumber}: {sheet.Name}";
            }
        }

        /// <summary>
        /// Constructs a name key for exporting.
        /// </summary>
        /// <param name="sheet">A Revit Sheet (extended).</param>
        /// <returns>A string.</returns>
        public static string Ext_ToExportKey(this ViewSheet sheet)
        {
            // Null catch
            if (sheet is null) { return "ERROR (-) - ERROR"; }

            // Get current revision
            string revisionNumber;

            if (sheet.GetCurrentRevision() != ElementId.InvalidElementId)
            {
                revisionNumber = sheet.GetRevisionNumberOnSheet(sheet.GetCurrentRevision());
            }
            else
            {
                revisionNumber = "-";
            }

            // Return sheet key
            var sheetKey = $"{sheet.SheetNumber} ({revisionNumber}) - {sheet.Name}";
            return gStr.MakeStringValid(sheetKey);
        }

        #endregion

        #region Add/remove revision

        /// <summary>
        /// Adds a revision to a sheet.
        /// </summary>
        /// <param name="sheet">A Sheet (extended).</param>
        /// <param name="revision">The Revision to add.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_AddRevision(this ViewSheet sheet, Revision revision)
        {
            // Get revisions on sheet
            var revisionIds = sheet.GetAdditionalRevisionIds();

            // If the revision is in the list
            if (revisionIds.Contains(revision.Id))
            {
                // Do not add it
                return Result.Failed;
            }
            else
            {
                // Otherwise, add it
                revisionIds.Add(revision.Id);
                sheet.SetAdditionalRevisionIds(revisionIds);
                return Result.Succeeded;
            }
        }

        /// <summary>
        /// Removes a revision to a sheet.
        /// </summary>
        /// <param name="sheet">A Sheet (extended).</param>
        /// <param name="revision">The Revision to remove.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_RemoveRevision(this ViewSheet sheet, Revision revision)
        {
            // Get revisions on sheet
            var revisionIds = sheet.GetAdditionalRevisionIds();

            // If the revision is in the list
            if (revisionIds.Contains(revision.Id))
            {
                // Remove it
                revisionIds.Remove(revision.Id);
                sheet.SetAdditionalRevisionIds(revisionIds);
                return Result.Succeeded;
            }
            else
            {
                // Otherwise, do not remove it
                return Result.Failed;
            }
        }

        #endregion

        #region Export to Pdf/Dwg

        /// <summary>
        /// Exports a sheet to PDF.
        /// </summary>
        /// <param name="sheet">A revit sheet (extended).</param>
        /// <param name="fileName">The file name to use (do not include extension).</param>
        /// <param name="directoryPath">The directory to export to.</param>
        /// <param name="doc">The document (optional).</param>
        /// <param name="options">The export options (optional).</param>
        /// <returns>A Result.</returns>
        public static Result Ext_ExportToPdf(this ViewSheet sheet, string fileName, string directoryPath,
            Document doc = null, PDFExportOptions options = null)
        {
            // Ensure we have a sheet
            if (sheet is null) { return Result.Failed; }
            
            // Set document and/or options if not provided
            doc ??= sheet.Document;
            options ??= gView.DefaultPdfExportOptions();

            // Set the file name
            options.FileName = fileName;

            // Create the sheet list
            var sheetIds = new List<ElementId>() { sheet.Id };

            // Try to export to Pdf
            try
            {
                doc.Export(directoryPath, sheetIds, options);
                return Result.Succeeded;
            }
            catch
            { 
                return Result.Failed;
            }
        }

        /// <summary>
        /// Exports a sheet to DWG.
        /// </summary>
        /// <param name="sheet">A revit sheet (extended).</param>
        /// <param name="fileName">The file name to use (do not include extension).</param>
        /// <param name="directoryPath">The directory to export to.</param>
        /// <param name="doc">The document (optional).</param>
        /// <param name="options">The export options (optional).</param>
        /// <returns>A Result.</returns>
        public static Result Ext_ExportToDwg(this ViewSheet sheet, string fileName, string directoryPath,
            Document doc = null, DWGExportOptions options = null)
        {
            // Ensure we have a sheet
            if (sheet is null) { return Result.Failed; }

            // Set document and/or options if not provided
            doc ??= sheet.Document;
            options ??= gView.DefaultDwgExportOptions();

            // Create the sheet list
            var sheetIds = new List<ElementId>() { sheet.Id };

            // Try to export to Dwg
            try
            {
                doc.Export(directoryPath, fileName, sheetIds, options);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        #endregion
    }
}