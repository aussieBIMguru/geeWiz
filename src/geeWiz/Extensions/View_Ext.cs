// Autodesk
using View = Autodesk.Revit.DB.View;
// geeWiz
using gFrm = geeWiz.Forms;
using Autodesk.Revit.UI;

// The class belongs to the extensions namespace
// View view.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to views.
    /// </summary>
    public static class View_Ext
    {
        #region Namekey

        /// <summary>
        /// Constructs a name key based on a Revit View.
        /// </summary>
        /// <param name="view">A Revit View (extended).</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToViewKey(this View view, bool includeId = false)
        {
            // Null catch
            if (view is null) { return "???"; }

            // Construct view key
            var viewPrefix = view.ViewType.ToString();

            // Catch if view template
            if (view.IsTemplate)
            {
                viewPrefix += " Template";
            }
            
            // Return key with Id
            if (includeId)
            {
                return $"{viewPrefix}: {view.Name} [{view.Id.ToString()}]";
            }
            // Return key without Id
            else
            {
                return $"{viewPrefix}: {view.Name}";
            }
        }

        #endregion

        #region Editable check

        /// <summary>
        /// Returns if an view is editable, with an optional message.
        /// </summary>
        /// <param name="view">The Element to check (extended).</param>
        /// <param name="doc">The Revit document (optional).</param>
        /// <param name="showMessage">Show a message if the view, presumed active, is not editable.</param>
        /// <returns>A boolean.</returns>
        public static bool Ext_ViewIsEditable(this View view, Document doc = null, bool showMessage = false)
        {
            // Get editability
            var isEdtiable = (view as Element).Ext_IsEditable(doc);

            // Message if not editable
            if (showMessage && !isEdtiable)
            {
                gFrm.Custom.Cancelled("Active view is not editable.");
            }

            // Return the result
            return isEdtiable;
        }

        #endregion

        #region View templates

        /// <summary>
        /// Assigns a view template to a view.
        /// </summary>
        /// <param name="view">The view (extended).</param>
        /// <param name="viewTemplate">The view template to assign.</param>
        /// <returns>A Result</returns>
        public static Result Ext_ApplyViewTemplate(this View view, View viewTemplate)
        {
            // Null check
            if (view is null || viewTemplate is null) { return Result.Failed; }

            // If the view template is a template
            if (viewTemplate.IsTemplate)
            {
                // Assign it
                view.ViewTemplateId = viewTemplate.Id;
                return Result.Succeeded;
            }
            else
            {
                // Otherwise, we failed
                return Result.Failed;
            }
        }

        /// <summary>
        /// Removes the view template from a view.
        /// </summary>
        /// <param name="view">The view (extended).</param>
        /// <returns>A Result</returns>
        public static Result Ext_RemoveViewTemplate(this View view)
        {
            // Null check
            if (view is null) { return Result.Failed; }
            
            // If the view template Id is not invalid
            if (view.ViewTemplateId != ElementId.InvalidElementId)
            {
                // Remove it
                view.ViewTemplateId = ElementId.InvalidElementId;
                return Result.Succeeded;
            }
            else
            {
                // Otherwise, we failed
                return Result.Failed;
            }
        }

        /// <summary>
        /// Returns if a view has a template.
        /// </summary>
        /// <param name="view">The view (extended).</param>
        /// <returns>A Boolean</returns>
        public static bool Ext_HasViewTemplate(this View view)
        {
            // Null check
            if (view is null) { return false; }

            // Return if it has a template
            return view.ViewTemplateId != ElementId.InvalidElementId;
        }

        /// <summary>
        /// Gets the view template from a view.
        /// </summary>
        /// <param name="view">The view (extended).</param>
        /// <returns>A Result</returns>
        public static View Ext_GetViewTemplate(this View view)
        {
            // Null check
            if (view is null) { return null; }

            // If the view has a template
            if (view.ViewTemplateId != ElementId.InvalidElementId)
            {
                // Return it
                return view.ViewTemplateId.Ext_GetElement<View>(view.Document);
            }
            else
            {
                // Otherwise, return null
                return null;
            }
        }

        #endregion

        #region Other

        /// <summary>
        /// Returns the Scope box of a view, if any.
        /// </summary>
        /// <param name="view">The view (extended).</param>
        /// <returns>A Scope Box Element.</returns>
        public static Element Ext_GetScopeBox(this View view)
        {
            // Null check
            if (view is null) { return null; }

            // Get parameter
            var parameter = (view as Element).Ext_GetBuiltInParameter(BuiltInParameter.VIEWER_VOLUME_OF_INTEREST_CROP);

            // Return null if it has no value or is invalid
            if (!parameter.HasValue || parameter.AsElementId() != ElementId.InvalidElementId) { return null; }

            // Return the element
            return view.Document.GetElement(parameter.AsElementId());
        }

        /// <summary>
        /// Returns the ViewFamilyType of a View.
        /// </summary>
        /// <param name="view">The view (extended).</param>
        /// <returns>A ViewFamilyType.</returns>
        public static ViewFamilyType Ext_GetViewFamilyType(this View view)
        {
            // Null check
            if (view is null) { return null; }

            // Return view family type
            if ((view as Element).Ext_GetElementType() is ViewFamilyType viewFamilyType)
            {
                return viewFamilyType;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}