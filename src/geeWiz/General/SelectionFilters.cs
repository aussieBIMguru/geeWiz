// Revit API
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

// The class belongs to the root namespace
namespace geeWiz.ISF
{
    /// <summary>
    /// ISelectionFilter that filters by provided builtincategory.
    /// </summary>
    public class ByBuiltInCategory : ISelectionFilter
    {
        // Private variable to store and give access to Id
        private ElementId _builtinCategoryId;

        // Default constructor, internalize the builtin category
        public ByBuiltInCategory(BuiltInCategory builtInCategory)
        {
            this._builtinCategoryId = new ElementId(builtInCategory);
        }

        // Condition for allowing elements by category
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

        // Do not allow reference selection
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
        // Private variable to store and give access to Id
        private ElementId _ownerViewId;

        // Default constructor, internalize the builtin category
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

        // Condition for allowing elements by owner view Id
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

        // Do not allow reference selection
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
        // Default constructor
        public AnyElement()
        {
            // Any element
        }

        // Condition for allowing elements by owner view Id
        public bool AllowElement(Element element)
        {
            // Always true
            return true;
        }

        // Do not allow reference selection
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}