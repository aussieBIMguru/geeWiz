// No dependencies yet

// The class belongs to the extensions namespace
// ViewSheet viewSheet.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to sheets.
    /// </summary>
    public static class ViewSheet_Ext
    {
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
        /// Adds or removes a revision from a sheet.
        /// </summary>
        /// <param name="sheet">A Revit Sheet (extended).</param>
        /// <param name="revision">The Revision to add/remove.</param>
        /// <param name="upRev">Add or remove the revision.</param>
        /// <returns>1 if actioned, 0 if passed.</returns>
        public static int Ext_RevSheet(this ViewSheet sheet, Revision revision, bool upRev = true)
        {
            // Get revisions on sheet
            var revisionIds = sheet.GetAdditionalRevisionIds();

            // If the revision is in the list
            if (revisionIds.Contains(revision.Id))
            {
                // Down revision routine
                if (!upRev)
                {
                    revisionIds.Remove(revision.Id);
                    sheet.SetAdditionalRevisionIds(revisionIds);
                    return 1;
                }
            }
            else
            {
                // Uprevision routine
                if (upRev)
                {
                    revisionIds.Add(revision.Id);
                    sheet.SetAdditionalRevisionIds(revisionIds);
                    return 1;
                }
            }

            // Do nothing
            return 0;
        }
    }
}