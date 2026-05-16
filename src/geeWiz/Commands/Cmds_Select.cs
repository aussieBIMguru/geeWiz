// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using View = Autodesk.Revit.DB.View;
// geeWiz
using geeWiz.Extensions;
using gFrm = geeWiz.Forms;

// The class belongs to the Commands namespace
namespace geeWiz.Commands.Cmds_Select
{
    /// <summary>
    /// Provides a filtered selection for rooms.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_PickRooms : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;

            // Make the category filter
            var selectionFilter = new ISF.ByBuiltInCategory(BuiltInCategory.OST_Rooms);

            // Select with filter applied
            List<Element> selectedElements = uiDoc.Ext_SelectWithFilter(
                selectionFilter: selectionFilter,
                selectionPrompt: "Select rooms, then press \'Finish\'");

            // If elements were selected, select them
            return uiDoc.Ext_SelectElements(selectedElements);
        }
    }

    /// <summary>
    /// Provides a filtered selection for walls.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_PickWalls : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;

            // Make the category filter
            var selectionFilter = new ISF.ByBuiltInCategory(BuiltInCategory.OST_Walls);

            // Select with filter applied
            List<Element> selectedElements = uiDoc.Ext_SelectWithFilter(
                selectionFilter: selectionFilter,
                selectionPrompt: "Select walls, then press \'Finish\'");

            // If elements were selected, select them
            return uiDoc.Ext_SelectElements(selectedElements);
        }
    }

    /// <summary>
    /// Gets all hidden elements in view.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_GetHidden : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Active view and hidden elements
            View activeView = uiDoc.ActiveGraphicalView;
            var hiddenElements = new List<Element>();

            // Ensure active view is editable
            if (!activeView.Ext_IsEditable(doc))
            {
                return gFrm.Custom.Cancelled("Active view is not editable.");
            }

            // Using a transaction
            using (var t = new Transaction(doc, "geeWiz: Reveal hidden"))
            {
                // Start the transaction
                t.Start();

                // Reveal hidden
                activeView.EnableRevealHiddenMode();
                activeView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);

                // Collect hidden elements in view
                hiddenElements = doc.Ext_Collector(activeView)
                    .Where(e => e.IsHidden(activeView))
                    .ToList();

                // Commit the transaction
                t.Commit();
            }

            // Select the elements
            return uiDoc.Ext_SelectElements(hiddenElements);
        }
    }

    /// <summary>
    /// Gets all title blocks on selected sheets.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_GetTtbs : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Get selected sheet Ids
            HashSet<ElementId> selectedSheetIds = uiDoc.Ext_SelectedElements<ViewSheet>()
                .Select(s => s.Id)
                .ToHashSet();

            // Collect all title blocks who have owner sheet Ids
            List<Element> titleBlocks = doc.Ext_GetElementsOfCategory(BuiltInCategory.OST_TitleBlocks)
                .Where(t => selectedSheetIds.Contains(t.OwnerViewId))
                .ToList();

            // If elements were found, select them
            return uiDoc.Ext_SelectElements(titleBlocks);
        }
    }

    /// <summary>
    /// Removes grouped elements from selection.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_RemoveGrouped : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Get selected elements which are not grouped
            List<Element> ungroupedElements = uiDoc.Ext_SelectedElements()
                .Where(e => e is not Group && e.GroupId == ElementId.InvalidElementId)
                .ToList();

            // Select ungrouped elements
            return uiDoc.Ext_SelectElements(ungroupedElements);
        }
    }
}