// Revit API
using View = Autodesk.Revit.DB.View;
using Room = Autodesk.Revit.DB.Architecture.Room;
// geeWiz
using gView = geeWiz.Utilities.ViewUtils;
using gFrm = geeWiz.Forms;
using geeWiz.Forms;

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
        /// Returns the start view of the document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <returns>A View.</returns>
        public static View Ext_GetStartView(this Document doc)
        {
            // Get start view settings
            var startingViewSettings = StartingViewSettings.GetStartingViewSettings(doc);
            
            // Return the starting view if there is one
            if (startingViewSettings.ViewId is ElementId elementId)
            {
                return doc.GetElement(elementId) as View;
            }
            else
            {
                return null;
            }
        }

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
        /// Select level(s) from the document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="title">The form title (optional).</param>
        /// <param name="multiSelect">Select more than one item.</param>
        /// <param name="sorted">Sort the levels by elevation.</param>
        /// <returns>A FormResult object.</returns>
        public static gFrm.FormResult Ext_SelectLevels(this Document doc, string title = null, bool multiSelect = true, bool sorted = false)
        {
            // Set the default form title if not provided
            if (title is null)
            {
                // Process based on multiSelect
                if (multiSelect)
                {
                    title = "Select Level(s):";
                }
                else
                {
                    title = "Select a Level:";
                }
            }

            // Get all Levels in document
            var levels = doc.Ext_GetLevels(sorted: sorted);

            // Process into keys (to return)
            var keys = levels
                .Select(l => l.Name)
                .ToList();

            // Process into values (to display)
            var values = levels
                .Cast<object>()
                .ToList();

            // Run the selection from list
            return gFrm.Custom.SelectFromList(keys: keys,
                values: values,
                title: title,
                multiSelect: multiSelect);
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
        /// Processes a form for showing revisions.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="title">The form title (optional).</param>
        /// <param name="message">The form message (optional).</param>
        /// <param name="sorted">Sort the Revisions by sequence.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult Ext_SelectRevision(Document doc, string title = null, string message = null, bool sorted = false)
        {
            // Set the default form title if not provided
            if (title is null)
            {
                title = "Select Revision";
            }

            // Set the default form message if not provided
            if (message is null)
            {
                message = "Select a revision from below:";
            }

            // Get all Revisions in document
            var revisions = doc.Ext_GetRevisions(sorted: sorted);

            // Process into keys (to return)
            var keys = revisions
                .Select(r => $"{r.SequenceNumber} - {r.RevisionDate}: {r.Description}")
                .ToList();

            // Process into values (to display)
            var values = revisions
                .Cast<object>()
                .ToList();

            // Run the selection from list
            return gFrm.Custom.SelectFromDropdown(keys: keys,
                values: values,
                title: title,
                message: message);
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

        /// <summary>
        /// Gets all Worksets in the given document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="sorted">Sort the Worksets by name.</param>
        /// <returns>A list of Worksets.</returns>
        public static List<Workset> Ext_GetWorksets(this Document doc, bool sorted = false)
        {
            // If the document is not workshared, return an empty list
            if (!doc.IsWorkshared) { return new List<Workset>(); }

            // Collect all Worksets in the document
            List<Workset> worksets = new FilteredWorksetCollector(doc)
                .OfKind(WorksetKind.UserWorkset)
                .ToWorksets()
                .ToList();

            // Return the Worksets sorted or unsorted
            if (sorted)
            {
                return worksets
                    .OrderBy(w => w.Name)
                    .ToList();
            }
            else
            {
                return worksets;
            }
        }
    }
}
