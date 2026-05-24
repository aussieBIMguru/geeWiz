// Revit API
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

// The class belongs to the ISelectionFilters (ISF) namespace
namespace geeWiz.ISF
{
    /// <summary>
    /// ISelectionFilter that filters by provided builtincategory.
    /// </summary>
    public class ByBuiltInCategory : ISelectionFilter
    {
        /// <summary>
        /// The BuiltInCategory Id to filter by.
        /// </summary>
        private ElementId _builtinCategoryId;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="builtInCategory">The BuiltInCategory to filter by.</param>
        public ByBuiltInCategory(BuiltInCategory builtInCategory)
        {
            this._builtinCategoryId = new ElementId(builtInCategory);
        }

        /// <summary>
        /// Checks if an Element passes the filter.
        /// </summary>
        /// <param name="element">The Element to check.</param>
        /// <returns>If the Element is selectable.</returns>
        public bool AllowElement(Element element)
        {
            // Check if the element has a category
            if (element.Category is Category category)
            {
                return category.Id == this._builtinCategoryId;
            }
            // False if category is null
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a Reference passes the filter.
        /// </summary>
        /// <param name="reference">Reference to check.</param>
        /// <param name="position">Point to check.</param>
        /// <returns>If the Reference is selectable.</returns>
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    /// <summary>
    /// ISelectionFilter that filters by provided view.
    /// Provide view as null for no owner view.
    /// </summary>
    public class ByViewOwnership : ISelectionFilter
    {
        /// <summary>
        /// The owner View Id to filter by.
        /// </summary>
        private ElementId _ownerViewId;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="view">The owner View to filter by.</param>
        public ByViewOwnership(View view)
        {
            if (view is null)
            {
                this._ownerViewId = null;
            }
            else
            {
                this._ownerViewId = view.Id;
            }
        }

        /// <summary>
        /// Checks if an Element passes the filter.
        /// </summary>
        /// <param name="element">The Element to check.</param>
        /// <returns>If the Element is selectable.</returns>
        public bool AllowElement(Element element)
        {
            // If element has an owner view id
            if (element.OwnerViewId is ElementId viewId)
            {
                // Return if it matches the filter's Id
                return viewId == this._ownerViewId;
            }
            else
            {
                // Otherwise it is not, check if we're filtering as such
                return this._ownerViewId == null;
            }
        }

        /// <summary>
        /// Checks if a Reference passes the filter.
        /// </summary>
        /// <param name="reference">Reference to check.</param>
        /// <param name="position">Point to check.</param>
        /// <returns>If the Reference is selectable.</returns>
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    /// <summary>
    /// ISelectionFilter that filters by provided view.
    /// Provide view as null for no owner view.
    /// </summary>
    public class AnyElement : ISelectionFilter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AnyElement()
        {
            // Any element
        }

        /// <summary>
        /// Checks if an Element passes the filter.
        /// </summary>
        /// <param name="element">The Element to check.</param>
        /// <returns>If the Element is selectable.</returns>
        public bool AllowElement(Element element)
        {
            // Always true
            return true;
        }

        /// <summary>
        /// Checks if a Reference passes the filter.
        /// </summary>
        /// <param name="reference">Reference to check.</param>
        /// <param name="position">Point to check.</param>
        /// <returns>If the Reference is selectable.</returns>
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}