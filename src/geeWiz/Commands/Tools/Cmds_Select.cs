// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
// geeWiz
using geeWiz.Extensions;
using gFrm = geeWiz.Forms;
using gSel = geeWiz.Utilities.Select_Utils;

// The class belongs to the Commands namespace
namespace geeWiz.Cmds_Select
{
    #region Cmd_PickRooms

    /// <summary>
    /// Provides a filtered selection for rooms.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_PickRooms : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;

            // Make the category filter
            var selectionFilter = new gSel.ISF_ByBuiltInCategory(BuiltInCategory.OST_Rooms);

            // Select with filter applied
            var selectedElements = uiDoc.Ext_SelectWithFilter(
                selectionFilter: selectionFilter,
                selectionPrompt: "Select rooms, then press \'Finish\'");

            // If elements were selected, select them
            return uiDoc.Ext_SelectElements(selectedElements);
        }
    }

    #endregion

    #region Cmd_PickWalls

    /// <summary>
    /// Provides a filtered selection for walls.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_PickWalls : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;

            // Make the category filter
            var selectionFilter = new gSel.ISF_ByBuiltInCategory(BuiltInCategory.OST_Walls);

            // Select with filter applied
            var selectedElements = uiDoc.Ext_SelectWithFilter(
                selectionFilter: selectionFilter,
                selectionPrompt: "Select walls, then press \'Finish\'");

            // If elements were selected, select them
            return uiDoc.Ext_SelectElements(selectedElements);
        }
    }

    #endregion

    #region Cmd_GetHidden

    /// <summary>
    /// Gets all hidden elements in view.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_GetHidden : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Active view and hidden elements
            var activeView = uiDoc.ActiveGraphicalView;
            var hiddenElements = new List<Element>();

            // Ensure active view is editable
            if (!(activeView as Element).Ext_IsEditable(doc))
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

    #endregion

    #region Cmd_GetTtbs

    /// <summary>
    /// Gets all title blocks on selected sheets.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_GetTtbs : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Get selected sheet Ids
            var selectedSheetIds = uiDoc.Ext_SelectedElements<ViewSheet>()
                .Select(s => s.Id)
                .ToList();

            // Collect all title blocks who have owner sheet Ids
            var titleBlocks = doc.Ext_GetElementsOfCategory(BuiltInCategory.OST_TitleBlocks)
                .Where(t => selectedSheetIds.Contains(t.OwnerViewId))
                .ToList();

            // If elements were found, select them
            return uiDoc.Ext_SelectElements(titleBlocks);
        }
    }

    #endregion

    #region Cmd_RemoveGrouped

    /// <summary>
    /// Removes grouped elements from selection.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_RemoveGrouped : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;

            // Get selected elements which are not grouped
            var ungroupedElements = uiDoc.Ext_SelectedElements()
                .Where(e => e is not Group && e.GroupId == ElementId.InvalidElementId)
                .ToList();

            // Select ungrouped elements
            return uiDoc.Ext_SelectElements(ungroupedElements);
        }
    }

    #endregion
}