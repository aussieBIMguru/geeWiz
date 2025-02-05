// Revit API
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

// The class belongs to the utility namespace
// using gSel = geeWiz.Utilities.Select_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to selection based operations.
    /// </summary>
    public static class Select_Utils
    {
        #region ISelectionFilters

        /// <summary>
        /// ISelectionFilter that filters by provided builtincategory.
        /// </summary>
        public class ISF_ByBuiltInCategory : ISelectionFilter
        {
            // Private variable to store and give access to Id
            private ElementId _builtinCategoryId;

            // Default constructor, internalize the builtin category
            public ISF_ByBuiltInCategory(BuiltInCategory builtInCategory)
            {
                _builtinCategoryId  = new ElementId(builtInCategory);
            }
            
            // Condition for allowing elements by category
            public bool AllowElement(Element element)
            {
                // Check if the element has a category
                if (element.Category is Category category)
                {
                    return category.Id == _builtinCategoryId;
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
        public class ISF_ByViewOwnership : ISelectionFilter
        {
            // Private variable to store and give access to Id
            private ElementId _ownerViewId;

            // Default constructor, internalize the builtin category
            public ISF_ByViewOwnership(View view)
            {
                if (view is null)
                {
                    _ownerViewId = null;
                }
                else
                {
                    _ownerViewId = view.Id;
                }
            }

            // Condition for allowing elements by owner view Id
            public bool AllowElement(Element element)
            {
                // If element has an owner view id
                if (element.OwnerViewId is ElementId viewId)
                {
                    // Return if it matches the filter's Id
                    return viewId == _ownerViewId;
                }
                else
                {
                    // Otherwise it is not, check if we're filtering as such
                    return _ownerViewId == null;
                }
            }

            // Do not allow reference selection
            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }

        #endregion
    }
}