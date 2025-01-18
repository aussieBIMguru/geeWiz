// Revit API
using View = Autodesk.Revit.DB.View;
using Room = Autodesk.Revit.DB.Architecture.Room;
// geeWiz
using gView = geeWiz.Utilities.ViewUtils;

// The class belongs to the extensions namespace
// Document doc.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to collecting objects.
    /// </summary>
    public static class Document_Ext
    {
        /// <summary>
        /// Creates a new collector object, with an optional view input.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="view">An optional view.</param>
        /// <returns>A FilteredElementCollector object.</returns>
        public static FilteredElementCollector Ext_Collector(this Document doc, View view = null)
        {
            if (view != null)
            {
                return new FilteredElementCollector(doc, view.Id);
            }
            else
            {
                return new FilteredElementCollector(doc);
            }
        }

        /// <summary>
        /// Collects all elements (not types) of the provided category.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="builtInCategory">A Revit BuiltInCategory.</param>
        /// <param name="view">An optional view.</param>
        /// <returns>A list of Elements.</returns>
        public static List<Element> Ext_GetElementsOfCategory(this Document doc, BuiltInCategory builtInCategory, View view = null)
        {
            return doc.Ext_Collector(view)
                .OfCategory(builtInCategory)
                .WhereElementIsNotElementType()
                .ToList();
        }

        /// <summary>
        /// Gets all sheets in the given document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="sorted">Sort the sheets by number.</param>
        /// <param name="includePlaceholders">Include placeholder sheets.</param>
        /// <returns>A list of ViewSheets.</returns>
        public static List<ViewSheet> Ext_GetSheets(this Document doc, bool sorted = false, bool includePlaceholders = false)
        {
            // Collect all viewsheets in document
            List<ViewSheet> sheets = doc.Ext_Collector()
                .OfClass(typeof(ViewSheet))
                .ToElements()
                .Cast<ViewSheet>()
                .ToList();

            // Filter our placeholders if not desired
            if (!includePlaceholders)
            {
                sheets = sheets
                    .Where(s => !s.IsPlaceholder)
                    .ToList();
            }

            // Return the elements sorted or unsorted
            if (sorted)
            {
                return sheets
                    .OrderBy(s => s.SheetNumber)
                    .ToList();
            }
            else
            {
                return sheets;
            }
        }

        /// <summary>
        /// Gets all views in the given document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="sorted">Sort the views by name.</param>
        /// <param name="viewTypes">View types to include.</param>
        /// <returns>A list of Views.</returns>
        public static List<View> Ext_GetViews(this Document doc, List<ViewType> viewTypes = null, bool sorted = false)
        {
            // Set default types if not provided
            if (viewTypes == null) { viewTypes = gView.VIEWTYPES_GRAPHICAL; }
            
            // Collect all views in document
            List<View> views = doc.Ext_Collector()
                .OfClass(typeof(View))
                .WhereElementIsNotElementType()
                .Cast<View>()
                .Where(v => !v.IsTemplate && viewTypes.Contains(v.ViewType))
                .ToList();

            // Return the views sorted or unsorted
            if (sorted)
            {
                return views
                    .OrderBy(v => $"{v.ViewType}{v.Name}")
                    .ToList();
            }
            else
            {
                return views;
            }
        }

        /// <summary>
        /// Gets all views templates in the given document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="sorted">Sort the views by name.</param>
        /// <param name="viewTypes">View types to include.</param>
        /// <returns>A list of View templates.</returns>
        public static List<View> Ext_GetViewTemplates(this Document doc, List<ViewType> viewTypes = null, bool sorted = false)
        {
            // Set default types if not provided
            if (viewTypes == null) { viewTypes = gView.VIEWTYPES_GRAPHICAL; }

            // Collect all view templates in document
            List<View> viewTemplates = doc.Ext_Collector()
                .OfClass(typeof(View))
                .WhereElementIsNotElementType()
                .Cast<View>()
                .Where(v => v.IsTemplate)
                .ToList();

            // Return the view templates sorted or unsorted
            if (sorted)
            {
                return viewTemplates
                    .OrderBy(v => $"{v.ViewType}{v.Name}")
                    .ToList();
            }
            else
            {
                return viewTemplates;
            }
        }

        /// <summary>
        /// Gets all ViewFamilyTypes in the given document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="sorted">Sort the ViewFamilyTypes by name.</param>
        /// <param name="viewFamilies">ViewFamilies to include.</param>
        /// <returns>A list of ViewFamilyTypes.</returns>
        public static List<ViewFamilyType> Ext_GetViewFamilyTypes(this Document doc, List<ViewFamily> viewFamilies = null, bool sorted = false)
        {
            // Set default types if not provided
            if (viewFamilies == null) { viewFamilies = gView.VIEWFAMILIES_GRAPHICAL; }

            // Collect all viewsfamilytypes in document
            List<ViewFamilyType> viewFamilyTypes = doc.Ext_Collector()
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .Where(vft => viewFamilies.Contains(vft.ViewFamily))
                .ToList();

            // Return the viewfamilytypes sorted or unsorted
            if (sorted)
            {
                return viewFamilyTypes
                    .OrderBy(v => $"{v.ViewFamily}{v.Name}")
                    .ToList();
            }
            else
            {
                return viewFamilyTypes;
            }
        }

        /// <summary>
        /// Gets all levels in the given document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="sorted">Sort the levels by elevation.</param>
        /// <returns>A list of Levels.</returns>
        public static List<Level> Ext_GetLevels(this Document doc, bool sorted = false)
        {
            // Collect all levels in document
            List<Level> levels = doc.Ext_Collector()
                .OfClass(typeof(Level))
                .Cast<Level>()
                .ToList();

            // Return the levels sorted or unsorted
            if (sorted)
            {
                return levels
                    .OrderBy(l => l.Elevation)
                    .ToList();
            }
            else
            {
                return levels;
            }
        }

        /// <summary>
        /// Gets all revisions in the given document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="sorted">Sort the revisions by sequence number.</param>
        /// <returns>A list of Revisions.</returns>
        public static List<Revision> Ext_GetRevisions(this Document doc, bool sorted = false)
        {
            // Collect all revisions in document
            List<Revision> revisions = doc.Ext_Collector()
                .OfClass(typeof(Revision))
                .Cast<Revision>()
                .ToList();

            // Return the revisions sorted or unsorted
            if (sorted)
            {
                return revisions
                    .OrderBy(r => r.SequenceNumber)
                    .ToList();
            }
            else
            {
                return revisions;
            }
        }

        /// <summary>
        /// Gets all revisions in the given document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="sorted">Sort the revisions by sequence number.</param>
        /// <returns>A list of Revisions.</returns>
        public static List<Room> Ext_GetRooms(this Document doc, View view = null, bool includeUnplaced = false, bool sorted = false)
        {
            // Collect all rooms in document
            List<Room> rooms = doc.Ext_Collector(view)
                .OfClass(typeof(Room))
                .Cast<Room>()
                .ToList();

            // Handle unplaced rooms if needed
            if (!includeUnplaced)
            {
                rooms = rooms
                    .Where(r => r.Area > 0)
                    .ToList();
            }

            // Return the rooms sorted or unsorted
            if (sorted)
            {
                return rooms
                    .OrderBy(r => r.Name)
                    .ToList();
            }
            else
            {
                return rooms;
            }
        }
    }
}
