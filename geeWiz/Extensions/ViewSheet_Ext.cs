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
    }
}