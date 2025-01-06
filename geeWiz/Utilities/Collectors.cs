// Revit API
using View = Autodesk.Revit.DB.View;

// The class belongs to the utility namespace
// using gCol = geeWiz.Utilities.FileUtils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to collecting objects.
    /// </summary>
    public static class Collectors
    {
        /// <summary>
        /// Creates a new collector object, with an optional view input.
        /// </summary>
        /// <param name="doc">A Revit document.</param>
        /// <param name="view">An optional view.</param>
        /// <returns>A FilteredElementCollector object.</returns>
        public static FilteredElementCollector NewElementCollector(Document doc, View view = null)
        {
            if (view is View)
            {
                return new FilteredElementCollector(doc, view.Id);
            }
            else
            {
                return new FilteredElementCollector(doc);
            }
        }

        /// <summary>
        /// Gets all sheets in the given document.
        /// </summary>
        /// <param name="doc">A Revit document.</param>
        /// <param name="sorted">Sort the sheets by number.</param>
        /// <param name="includePlaceholders">Include placeholder sheets.</param>
        /// <returns>A list of ViewSheets.</returns>
        public static List<ViewSheet> Sheets(Document doc, bool sorted = false, bool includePlaceholders = false)
        {
            // Collect all viewsheets in document
            List<ViewSheet> sheets = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .ToElements()
                .Cast<ViewSheet>()
                .ToList();

            // Filter our placeholders if not desired
            if (!includePlaceholders)
            {
                sheets = sheets.Where(s => !s.IsPlaceholder).ToList();
            }

            // Return the elements sorted or unsorted
            if (sorted)
            {
                return sheets.OrderBy(s => s.SheetNumber).ToList();
            }
            else
            {
                return sheets;
            }
        }
    }
}
