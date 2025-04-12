// Revit API
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;
// geeWiz
using gFrm = geeWiz.Forms;
using Autodesk.Revit.UI.Selection;

// The class belongs to the extensions namespace
// UIDocument uiDoc.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Extension methods to the UIDocument class.
    /// </summary>
    public static class UIDocument_Ext
    {
        #region Collect or open views

        /// <summary>
        /// Activate and switch to the given Revit view.
        /// </summary>
        /// <param name="uiDoc">The UIDocument (extended).</param>
        /// <param name="view">The view to open.</param>
        /// <param name="showMessage">If you want to show error/completion messages.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_OpenView(this UIDocument uiDoc, View view, bool showMessage = true)
        {
            // Ensure both are valid
            if (uiDoc is null || view is null)
            {
                return Result.Failed;
            }

            // Catch if view is already active and opened
            if (uiDoc.ActiveGraphicalView.Id == view.Id)
            {
                if (showMessage)
                {
                    gFrm.Custom.Completed("View is already active and opened.");
                }
                return Result.Succeeded;
            }

            // Try to open the view and switch to it (even if opened)
            try
            {
                uiDoc.RequestViewChange(view);
                return Result.Succeeded;
            }
            catch
            {
                if (showMessage)
                {
                    gFrm.Custom.Cancelled("View could not be opened.");
                }
                return Result.Cancelled;
            }
        }

        /// <summary>
        /// Returns all opened views of the active document.
        /// </summary>
        /// <param name="uiDoc">The UIDocument (extended).</param>
        /// <returns>A list of Views.</returns>
        public static List<View> Ext_OpenedViews(this UIDocument uiDoc)
        {
            // Return empty list if no uiDoc
            if (uiDoc is null) { return new List<View>(); }

            // Get document
            var doc = uiDoc.Document;

            // Return opened UIViews as Views
            return uiDoc.GetOpenUIViews()
                .Select(u => doc.GetElement(u.ViewId))
                .Cast<View>()
                .ToList();
        }

        #endregion

        #region Select element(s)

        // Method overload - Select Elements / ElementIds

        /// <summary>
        /// Set the current selection to given elements.
        /// </summary>
        /// <param name="uiDoc">The UIDocument (extended).</param>
        /// <param name="elements">The elements to select.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_SelectElements(this UIDocument uiDoc, List<Element> elements)
        {
            // Ensure both are valid
            if (uiDoc is null) { return Result.Failed; }

            // Get element Ids as ICollection
            var idsToSelect = elements
                .Select(e => e.Id)
                .Where(i => i is not null)
                .Distinct()
                .ToList();

            // Select elements
            if (idsToSelect.Count > 0)
            {
                uiDoc.Selection.SetElementIds(idsToSelect);
            }

            // Return success
            return Result.Succeeded;
        }

        /// <summary>
        /// Set the current selection to given ElementIds.
        /// </summary>
        /// <param name="uiDoc">The UIDocument (extended).</param>
        /// <param name="elementIds">The element Ids to select.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_SelectElements(this UIDocument uiDoc, List<ElementId> elementIds)
        {
            // Ensure both are valid
            if (uiDoc is null)
            {
                return Result.Failed;
            }

            // Select elements
            if (elementIds.Count > 0)
            {
                uiDoc.Selection.SetElementIds(elementIds);
            }

            // Return success
            return Result.Succeeded;
        }

        // Method overload - Select Element
        /// <summary>
        /// Set the current selection to given element.
        /// </summary>
        /// <param name="uiDoc">The UIDocument (extended).</param>
        /// <param name="element">The element to select.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_SelectElement(this UIDocument uiDoc, Element element)
        {
            // Run the elements method using a new list
            return Ext_SelectElements(uiDoc, new List<Element>() { element });
        }

        // Method overload - Select Element
        /// <summary>
        /// Set the current selection to given ElementId.
        /// </summary>
        /// <param name="uiDoc">The UIDocument (extended).</param>
        /// <param name="elementId">The ElementId to select.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_SelectElement(this UIDocument uiDoc, ElementId elementId)
        {
            // Run the elementIds method using a new list
            return Ext_SelectElements(uiDoc, new List<ElementId>() { elementId });
        }

        #endregion

        #region Get selected elements

        /// <summary>
        /// Gets currently selected elements.
        /// </summary>
        /// <param name="uiDoc">The active UIDocument (extended).</param>
        /// <returns>A list of elements.</returns>
        public static List<Element> Ext_SelectedElements(this UIDocument uiDoc)
        {
            // Get selected elements
            return uiDoc.Selection.GetElementIds()
                .Select(i => uiDoc.Document.GetElement(i))
                .Where(e => e is not null)
                .ToList();
        }

        /// <summary>
        /// Gets currently selected sheets.
        /// </summary>
        /// <param name="uiDoc">The active UIDocument (extended).</param>
        /// <returns>A list of sheets.</returns>
        public static List<ViewSheet> Ext_SelectedSheets(this UIDocument uiDoc)
        {
            // Get selected sheets
            return uiDoc.Ext_SelectedElements()
                .Where(e => e is ViewSheet)
                .Cast<ViewSheet>()
                .ToList();
        }

        /// <summary>
        /// Gets currently selected views
        /// </summary>
        /// <param name="uiDoc">The active UIDocument (extended).</param>
        /// <returns>A list of views.</returns>
        public static List<View> Ext_SelectedViews(this UIDocument uiDoc)
        {
            // Get selected views
            return uiDoc.Ext_SelectedElements()
                .Where(e => e is View)
                .Cast<View>()
                .ToList();
        }

        /// <summary>
        /// Gets currently selected rooms.
        /// </summary>
        /// <param name="uiDoc">The active UIDocument (extended).</param>
        /// <returns>A list of rooms.</returns>
        public static List<SpatialElement> Ext_SelectedRooms(this UIDocument uiDoc)
        {
            // Get selected views
            return uiDoc.Ext_SelectedElements()
                .Where(e => e is Autodesk.Revit.DB.Architecture.Room)
                .Cast<SpatialElement>()
                .ToList();
        }

        #endregion

        #region Prompted selections

        /// <summary>
        /// Run a selection using an ISelectionFilter.
        /// </summary>
        /// <param name="uiDoc">The active UIDocument (extended).</param>
        /// <param name="selectionFilter">The ISelectionFilter to apply.</param>
        /// <param name="selectionPrompt">Selection prompt in sub-ribbon.</param>
        /// <param name="showMessage">Show error message if encountered.</param>
        /// <returns>A list of Elements.</returns>
        public static List<Element> Ext_SelectWithFilter(this UIDocument uiDoc, ISelectionFilter selectionFilter,
            string selectionPrompt, bool showMessage = true)
        {
            // Try to form a selection
            try
            {
                // Return the selected elements if it worked
                return uiDoc.Selection.PickObjects(ObjectType.Element, selectionFilter, selectionPrompt)
                    .Select(i => uiDoc.Document.GetElement(i))
                    .Where (e => e is Element)
                    .Cast<Element>()
                    .ToList();
            }
            // Return an empty list if it did not work
            catch 
            {
                if (showMessage)
                {
                    gFrm.Custom.Cancelled("Could not create filtered selection.");
                }
                return new List<Element>();
            }
        }

        /// <summary>
        /// Run a single element selection (pick) using an ISelectionFilter.
        /// </summary>
        /// <param name="uiDoc">The active UIDocument (extended).</param>
        /// <param name="selectionFilter">The ISelectionFilter to apply.</param>
        /// <param name="selectionPrompt">Selection prompt in sub-ribbon.</param>
        /// <param name="showMessage">Show error message if encountered.</param>
        /// <returns>An Element.</returns>
        public static Element Ext_PickWithFilter(this UIDocument uiDoc, ISelectionFilter selectionFilter,
            string selectionPrompt, bool showMessage = true)
        {
            // Set a reference variable
            Reference reference = null;
            
            // Try to pick the object and get its reference
            try
            {
                reference = uiDoc.Selection.PickObject(ObjectType.Element, selectionFilter, selectionPrompt);
            }
            // Optional error message, reference remains as null
            catch
            {
                if (showMessage)
                {
                    gFrm.Custom.Cancelled("Could not create filtered selection.");
                }
            }

            // Catch the scenario that we have no reference, or no valid Id
            if (reference is null) { return null; }
            if (reference.ElementId == ElementId.InvalidElementId) { return null; }

            // Return the element if not
            return uiDoc.Document.GetElement(reference.ElementId);
        }

        #endregion
    }
}