// Revit API
using View = Autodesk.Revit.DB.View;
// geeWiz
using gFrm = geeWiz.Forms;
using gSpa = geeWiz.Utilities.Spatial_Utils;
using gView = geeWiz.Utilities.View_Utils;
using Autodesk.Revit.UI;

// The class belongs to the extensions namespace
// Document doc.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to collecting and selecting objects.
    /// </summary>
    public static class Document_Ext
    {
        #region Document properties

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

        #endregion

        #region Delete element(s)

        // DeleteElement has 2 overloads (Element / ElementId)

        /// <summary>
        /// Attempts to delete an element from the document.
        /// </summary>
        /// <param name="doc">The Document to delete from (extended).</param>
        /// <param name="element">A Revit Element.</param>
        /// <returns>A Result object.</returns>
        public static Result Ext_DeleteElement(this Document doc, Element element)
        {
            // Try to delete the element
            try
            {
                doc.Delete(element.Id);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        /// <summary>
        /// Attempts to delete an element from the document by Id.
        /// </summary>
        /// <param name="doc">The Document to delete from (extended).</param>
        /// <param name="elementId">A Revit ElementId.</param>
        /// <returns>A Result object.</returns>
        public static Result Ext_DeleteElement(this Document doc, ElementId elementId)
        {
            // Try to delete the element
            try
            {
                doc.Delete(elementId);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        // DeleteElements has 2 overloads (Elements / ElementIds)

        /// <summary>
        /// Attempts to delete elements from the document.
        /// </summary>
        /// <param name="doc">The Document to delete from (extended).</param>
        /// <param name="elements">Revit Elements.</param>
        /// <returns>A list of Results.</returns>
        public static List<Result> Ext_DeleteElements(this Document doc, List<Element> elements)
        {
            // Result list to return
            var results = new List<Result>();
            
            // For each element
            foreach (Element element in elements)
            {
                // Try to delete, add result
                results.Add(doc.Ext_DeleteElement(element));
            }

            // Return the results
            return results;
        }

        /// <summary>
        /// Attempts to delete elements by Id from the document.
        /// </summary>
        /// <param name="doc">The Document to delete from (extended).</param>
        /// <param name="elementIds">Revit ElementIds.</param>
        /// <returns>A list of Results.</returns>
        public static List<Result> Ext_DeleteElements(this Document doc, List<ElementId> elementIds)
        {
            // Result list to return
            var results = new List<Result>();

            // For each element
            foreach (ElementId elementId in elementIds)
            {
                // Try to delete, add result
                results.Add(doc.Ext_DeleteElement(elementId));
            }

            // Return the results
            return results;
        }

        /// <summary>
        /// Attempts to delete elements from the document with a transaction and progress bar.
        /// </summary>
        /// <param name="doc">The Document to delete from (extended).</param>
        /// <param name="elements">Revit ElementIds.</param>
        /// <param name="typeName">Name of element type to delete.</param>
        /// <param name="showMessage">Show messages to the user.</param>
        /// <returns>A result.</returns>
        public static Result Ext_DeleteElementsRoutine(this Document doc, List<Element> elements, string typeName = "Element", bool showMessage = true)
        {
            // If no elements, we are finished
            if (elements.Count == 0)
            {
                // Optional message
                if (showMessage)
                {
                    gFrm.Custom.Completed($"No {typeName}s available for deletion.");
                }

                // Return success
                return Result.Succeeded;
            }

            // Progress bar properties
            int pbTotal = elements.Count;
            int pbStep = gFrm.Custom.ProgressDelay(pbTotal);
            int deleteCount = 0;

            // Using a progress bar
            using (var pb = new gFrm.ProgressBar($"Deleting {typeName}(s)...", pbTotal: pbTotal))
            {
                // Using a transaction
                using (var t = new Transaction(doc, $"geeWiz: Delete {typeName}(s)"))
                {
                    // Start the transaction
                    t.Start();

                    // For each element
                    foreach (var element in elements)
                    {
                        // Check for cancellation
                        if (pb.CancelCheck(t))
                        {
                            return Result.Cancelled;
                        }

                        // Try to delete the element, uptick deletCount if we do
                        if (doc.Ext_DeleteElement(element) == Result.Succeeded)
                        {
                            deleteCount++;
                        }

                        // Increase progress
                        Thread.Sleep(pbStep);
                        pb.Increment();
                    }

                    // Commit the transaction
                    pb.Commit(t);
                }
            }

            // Optional message
            if (showMessage)
            {
                gFrm.Custom.Completed($"{deleteCount}/{pbTotal} {typeName}s deleted.");
            }

            // Return the result
            return Result.Succeeded;
        }

        #endregion

        #region Generic collectors

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

        #endregion

        #region Sheet collector/selection

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
        /// Select sheet(s) from the document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="sheets">An optional list of sheets to show.</param>
        /// <param name="title">The form title (optional).</param>
        /// <param name="multiSelect">Select more than one item.</param>
        /// <param name="sorted">Sort the levels by elevation.</param>
        /// <param name="includePlaceholders">Include placeholder sheets.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A FormResult object.</returns>
        public static gFrm.FormResult Ext_SelectSheets(this Document doc, List<ViewSheet> sheets = null, string title = null,
            bool multiSelect = true, bool sorted = false, bool includePlaceholders = false, bool includeId = false)
        {
            // Set the default form title if not provided
            if (title is null)
            {
                // Process based on multiSelect
                if (multiSelect)
                {
                    title = "Select Sheets(s):";
                }
                else
                {
                    title = "Select a Sheet:";
                }
            }

            // Get all Sheets in document if none provided
            sheets ??= doc.Ext_GetSheets(sorted: sorted);

            // Process into keys (to return)
            var keys = sheets
                .Select(s => s.Ext_ToSheetKey(includeId))
                .ToList();

            // Process into values (to display)
            var values = sheets
                .Cast<object>()
                .ToList();

            // Run the selection from list
            return gFrm.Custom.SelectFromList(keys: keys,
                values: values,
                title: title,
                multiSelect: multiSelect);
        }

        #endregion

        #region View collector/selection

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
            viewTypes ??= gView.VIEWTYPES_GRAPHICAL;
            
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
        /// Select sheet(s) from the document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="views">An optional list of views to show.</param>
        /// <param name="viewTypes">View types to include.</param>
        /// <param name="title">The form title (optional).</param>
        /// <param name="multiSelect">Select more than one item.</param>
        /// <param name="sorted">Sort the levels by elevation.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A FormResult object.</returns>
        public static gFrm.FormResult Ext_SelectViews(this Document doc, List<View> views = null, List<ViewType> viewTypes = null,
            string title = null, bool multiSelect = true, bool sorted = false, bool includeId = false)
        {
            // Set the default form title if not provided
            title ??= multiSelect ? "Select View(s):" : "Select a View:";

            // Get all Views in document if none provided
            views ??= doc.Ext_GetViews(viewTypes: viewTypes, sorted: sorted);

            // Process into keys (to return)
            var keys = views
                .Select(v => v.Ext_ToViewKey(includeId))
                .ToList();

            // Process into values (to display)
            var values = views
                .Cast<object>()
                .ToList();

            // Run the selection from list
            return gFrm.Custom.SelectFromList(keys: keys,
                values: values,
                title: title,
                multiSelect: multiSelect);
        }

        #endregion

        #region View Template collector/selection

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
        /// Select sheet(s) from the document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="views">An optional list of views to show.</param>
        /// <param name="viewTypes">View types to include.</param>
        /// <param name="title">The form title (optional).</param>
        /// <param name="multiSelect">Select more than one item.</param>
        /// <param name="sorted">Sort the levels by elevation.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A FormResult object.</returns>
        public static gFrm.FormResult Ext_SelectViewTemplates(this Document doc, List<View> views = null, List<ViewType> viewTypes = null,
            string title = null, bool multiSelect = true, bool sorted = false, bool includeId = false)
        {
            // Filter to just view templates
            views = views.Where(v => v.IsTemplate).ToList();
            
            // Same routine as Views, key handles view template identification
            return doc.Ext_SelectViews(views, viewTypes, title, multiSelect, sorted, includeId);
        }

        #endregion

        #region VewFamilyType collector/selection

        /// <summary>
        /// Gets all ViewFamilyTypes in the given document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="viewFamilies">ViewFamilies to include.</param>
        /// <param name="sorted">Sort the ViewFamilyTypes by name.</param>
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
        /// Select sheet(s) from the document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="viewFamilies">ViewFamilies to include.</param>
        /// <param name="title">The form title (optional).</param>
        /// <param name="multiSelect">Select more than one item.</param>
        /// <param name="sorted">Sort the levels by elevation.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A FormResult object.</returns>
        public static gFrm.FormResult Ext_SelectViewFamilyTypes(this Document doc, List<ViewFamily> viewFamilies= null,
            string title = null, bool multiSelect = true, bool sorted = false, bool includeId = false)
        {
            // Set the default form title if not provided
            title ??= multiSelect ? "Select ViewFamilyType(s):" : "Select a ViewFamilyType:";

            // Get all ViewFamilyTypes in document
            var viewFamilyTypes = doc.Ext_GetViewFamilyTypes(viewFamilies: viewFamilies, sorted: sorted);

            // Process into keys (to return)
            var keys = viewFamilyTypes
                .Select(v => v.Ext_ToViewFamilyTypeKey(includeId))
                .ToList();

            // Process into values (to display)
            var values = viewFamilyTypes
                .Cast<object>()
                .ToList();

            // Run the selection from list
            return gFrm.Custom.SelectFromList(keys: keys,
                values: values,
                title: title,
                multiSelect: multiSelect);
        }

        #endregion

        #region Level collector/selection

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
            title ??= multiSelect ? "Select Level(s):" : "Select a Level:";

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

        #endregion

        #region Titleblock type collector/selection

        /// <summary>
        /// Gets all titleblock types in the given document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="sorted">Sort the types by name.</param>
        /// <returns>A list of Family types.</returns>
        public static List<FamilySymbol> Ext_GetTitleblockTypes(this Document doc, bool sorted = false)
        {
            // Collect all titleblock types in document
            List<FamilySymbol> titleblockTypes = doc.Ext_Collector()
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .ToList();

            // Return the types sorted or unsorted
            if (sorted)
            {
                return titleblockTypes
                    .OrderBy(t => t.Ext_ToFamilySymbolKey())
                    .ToList();
            }
            else
            {
                return titleblockTypes;
            }
        }

        /// <summary>
        /// Select titleblock type(s) from the document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="title">The form title (optional).</param>
        /// <param name="multiSelect">Select more than one item.</param>
        /// <param name="sorted">Sort the types by name.</param>
        /// <returns>A FormResult object.</returns>
        public static gFrm.FormResult Ext_SelectTitleblockTypes(this Document doc, string title = null, bool multiSelect = true, bool sorted = false)
        {
            // Set the default form title if not provided
            title ??= multiSelect ? "Select Titleblock type(s):" : "Select a Titleblock type:";

            // Get all Titleblock types in document
            var titleblockTypes = doc.Ext_GetTitleblockTypes(sorted: sorted);

            // Process into keys (to return)
            var keys = titleblockTypes
                .Select(t => t.Ext_ToFamilySymbolKey())
                .ToList();

            // Process into values (to display)
            var values = titleblockTypes
                .Cast<object>()
                .ToList();

            // Run the selection from list
            return gFrm.Custom.SelectFromList(keys: keys,
                values: values,
                title: title,
                multiSelect: multiSelect);
        }

        #endregion

        #region Revision collector/selection

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
        /// Select revision(s) from the document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="title">The form title (optional).</param>
        /// <param name="multiSelect">Select more than one item.</param>
        /// <param name="sorted">Sort the revisions by sequence.</param>
        /// <returns>A FormResult object.</returns>
        public static gFrm.FormResult Ext_SelectRevisions(this Document doc, string title = null, bool multiSelect = true, bool sorted = false)
        {
            // Set the default form title if not provided
            title ??= multiSelect ? "Select Revision(s):" : "Select a Revision:";

            // Get all revisions in document
            var revisions = doc.Ext_GetRevisions(sorted: sorted);

            // Process into keys (to return)
            var keys = revisions
                .Select(r => r.Ext_ToRevisionKey())
                .ToList();

            // Process into values (to display)
            var values = revisions
                .Cast<object>()
                .ToList();

            // Run the selection from list
            return gFrm.Custom.SelectFromList(keys: keys,
                values: values,
                title: title,
                multiSelect: multiSelect);
        }

        /// <summary>
        /// Select a revision from the document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="title">The form title (optional).</param>
        /// <param name="message">The form message (optional).</param>
        /// <param name="sorted">Sort the Revisions by sequence.</param>
        /// <returns>A FormResult object.</returns>
        public static gFrm.FormResult Ext_SelectRevision(Document doc, string title = null, string message = null, bool sorted = false)
        {
            // Set the default form title/message if not provided
            title ??= "Select Revision";
            message ??= "Select a revision from below:";

            // Get all Revisions in document
            var revisions = doc.Ext_GetRevisions(sorted: sorted);

            // Process into keys (to return)
            var keys = revisions
                .Select(r => r.Ext_ToRevisionKey())
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

        #endregion

        #region Room collector/selection

        /// <summary>
        /// Gets all rooms in the given document.
        /// </summary>
        /// <param name="doc">A Revit document (extended).</param>
        /// <param name="view">An optional View to collect visible rooms from.</param>
        /// <param name="sorted">Sort the rooms by name.</param>
        /// <param name="includePlaced">Include valid rooms.</param>
        /// <param name="includeRedundant">Include redundant rooms.</param>
        /// <param name="includeUnenclosed">Include unenclosed rooms.</param>
        /// <param name="includeUnplaced">Include unplaced rooms.</param>
        /// <returns>A list of Rooms.</returns>
        public static List<SpatialElement> Ext_GetRooms(this Document doc, View view = null, bool sorted = false,
            bool includePlaced = true, bool includeRedundant = false, bool includeUnenclosed = false, bool includeUnplaced = false)
        {
            // Collect all rooms in document
            List<SpatialElement> rooms = doc.Ext_Collector(view)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .Cast<SpatialElement>()
                .ToList();

            // New list to construct by placement
            var roomsFinal = new List<SpatialElement>();

            // Filter the rooms, then rebuild the list by placement types
            var roomMatrixByPlacement = gSpa.RoomsMatrixByPlacement(rooms, doc);
            if (includePlaced) { roomsFinal.AddRange(roomMatrixByPlacement[0]); }
            if (includeRedundant) { roomsFinal.AddRange(roomMatrixByPlacement[1]); }
            if (includeUnenclosed) { roomsFinal.AddRange(roomMatrixByPlacement[2]); }
            if (includeUnplaced) { roomsFinal.AddRange(roomMatrixByPlacement[3]); }

            // Return the rooms sorted or unsorted
            if (sorted)
            {
                return roomsFinal
                    .OrderBy(r => r.Ext_ToRoomKey())
                    .ToList();
            }
            else
            {
                return roomsFinal;
            }
        }

        /// <summary>
        /// Select room(s) from the document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="rooms">Rooms to select from (optional).</param>
        /// <param name="title">The form title (optional).</param>
        /// <param name="multiSelect">Select more than one item.</param>
        /// <param name="sorted">Sort the revisions by sequence.</param>
        /// <param name="includePlaced">Include valid rooms.</param>
        /// <param name="includeRedundant">Include redundant rooms.</param>
        /// <param name="includeUnenclosed">Include unenclosed rooms.</param>
        /// <param name="includeUnplaced">Include unplaced rooms.</param>
        /// <returns>A FormResult object.</returns>
        public static gFrm.FormResult Ext_SelectRooms(this Document doc, List<SpatialElement> rooms = null, string title = null, bool multiSelect = true,
            bool sorted = false, bool includePlaced = true, bool includeRedundant = false, bool includeUnenclosed = false, bool includeUnplaced = false)
        {
            // Set the default form title if not provided
            title ??= multiSelect ? "Select Room(s):" : "Select a Room:";

            // Get all rooms in document if not provided
            if (rooms is null)
            {
                rooms = doc.Ext_GetRooms(sorted: sorted,
                includePlaced: includePlaced,
                includeRedundant: includeRedundant,
                includeUnenclosed: includeUnenclosed,
                includeUnplaced: includeUnplaced);
            }

            // Process into keys (to return)
            var keys = rooms
                .Select(r => r.Ext_ToRoomKey())
                .ToList();

            // Process into values (to display)
            var values = rooms
                .Cast<object>()
                .ToList();

            // Run the selection from list
            return gFrm.Custom.SelectFromList(keys: keys,
                values: values,
                title: title,
                multiSelect: multiSelect);
        }

        #endregion

        #region Workset collector/selection

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

        /// <summary>
        /// Select Workset(s) from the document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="title">The form title (optional).</param>
        /// <param name="multiSelect">Select more than one item.</param>
        /// <param name="sorted">Sort the revisions by sequence.</param>
        /// <returns>A FormResult object.</returns>
        public static gFrm.FormResult Ext_SelectWorksets(this Document doc, string title = null, bool multiSelect = true, bool sorted = false)
        {
            // Set the default form title if not provided
            title ??= multiSelect ? "Select Workset(s):" : "Select a Workset:";

            // Get all worksets in document
            var worksets = doc.Ext_GetWorksets(sorted: sorted);

            // Process into keys (to return)
            var keys = worksets
                .Select(w => $"{w.Name}")
                .ToList();

            // Process into values (to display)
            var values = worksets
                .Cast<object>()
                .ToList();

            // Run the selection from list
            return gFrm.Custom.SelectFromList(keys: keys,
                values: values,
                title: title,
                multiSelect: multiSelect);
        }

        #endregion
    }
}
