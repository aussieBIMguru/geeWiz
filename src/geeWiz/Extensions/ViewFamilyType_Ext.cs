// The class belongs to the extensions namespace
// ViewFamilyType viewFamilyType.ExtensionMethod()
using Autodesk.Revit.UI;
using DocumentFormat.OpenXml.Spreadsheet;
using View = Autodesk.Revit.DB.View;

namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to ViewFamilyTypes.
    /// </summary>
    public static class ViewFamilyType_Ext
    {
        #region Namekey

        /// <summary>
        /// Constructs a name key based on a Revit ViewFamilyType.
        /// </summary>
        /// <param name="viewFamilyType">A Revit ViewFamilyType.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToViewFamilyTypeKey(this ViewFamilyType viewFamilyType, bool includeId = false)
        {
            // Null catch
            if (viewFamilyType is null) { return "???"; }

            // Return the name key
            var nameKey = $"{viewFamilyType.ViewFamily.ToString()}: {viewFamilyType.Name}";
            return viewFamilyType.Ext_FinalizeKey(nameKey, includeId);
        }

        #endregion

        #region View templates

        /// <summary>
        /// Assigns a view template to a ViewFamilyType.
        /// </summary>
        /// <param name="viewFamilyType">The ViewFamilyType (extended).</param>
        /// <param name="viewTemplate">The view template to assign.</param>
        /// <returns>A Result</returns>
        public static Result Ext_ApplyViewTemplate(this ViewFamilyType viewFamilyType, View viewTemplate)
        {
            // Null check
            if (viewFamilyType is null || viewTemplate is null) { return Result.Failed; }

            // If the view template is a template
            if (viewTemplate.IsTemplate)
            {
                // Assign it
                viewFamilyType.DefaultTemplateId = viewTemplate.Id;
                return Result.Succeeded;
            }
            else
            {
                // Otherwise, we failed
                return Result.Failed;
            }
        }

        /// <summary>
        /// Removes the view template from a ViewFamilyType.
        /// </summary>
        /// <param name="viewFamilyType">The ViewFamilyType (extended).</param>
        /// <returns>A Result</returns>
        public static Result Ext_RemoveViewTemplate(this ViewFamilyType viewFamilyType)
        {
            // Null check
            if (viewFamilyType is null) { return Result.Failed; }

            // If the view template Id is not invalid
            if (viewFamilyType.DefaultTemplateId != ElementId.InvalidElementId)
            {
                // Remove it
                viewFamilyType.DefaultTemplateId = ElementId.InvalidElementId;
                return Result.Succeeded;
            }
            else
            {
                // Otherwise, we failed
                return Result.Failed;
            }
        }

        /// <summary>
        /// Returns if a ViewFamilyType has a template.
        /// </summary>
        /// <param name="viewFamilyType">The ViewFamilyType (extended).</param>
        /// <returns>A Boolean</returns>
        public static bool Ext_HasViewTemplate(this ViewFamilyType viewFamilyType)
        {
            // Null check
            if (viewFamilyType is null) { return false; }

            // Return if it has a template
            return viewFamilyType.DefaultTemplateId != ElementId.InvalidElementId;
        }

        /// <summary>
        /// Gets the view template from a ViewFamilyType.
        /// </summary>
        /// <param name="viewFamilyType">The ViewFamilyType (extended).</param>
        /// <returns>A Result</returns>
        public static View Ext_GetViewTemplate(this ViewFamilyType viewFamilyType)
        {
            // Null check
            if (viewFamilyType is null) { return null; }

            // If the view has a template
            if (viewFamilyType.DefaultTemplateId != ElementId.InvalidElementId)
            {
                // Return it
                return viewFamilyType.DefaultTemplateId.Ext_GetElement<View>(viewFamilyType.Document);
            }
            else
            {
                // Otherwise, return null
                return null;
            }
        }

        #endregion

        #region Collect views

        /// <summary>
        /// Gets all views of the given ViewFamilyType.
        /// </summary>
        /// <param name="viewFamilyType">The ViewFamilyType (extended).</param>
        /// <param name="doc">The document.</param>
        /// <param name="sorted">Sort views by name.</param>
        /// <returns>A list of views.</returns>
        public static List<View> Ext_AllViewsOfType(this ViewFamilyType viewFamilyType, Document doc = null, bool sorted = true)
        {
            // Null check
            if (viewFamilyType is null) { return new List<View>(); }
            
            // Get current document
            doc ??= Globals.UiApp.ActiveUIDocument.Document;

            // View family type Id
            var viewFamilyTypeId = viewFamilyType.Id;

            // Get all views of given view type
            return doc.Ext_GetViews(sorted: sorted)
                .Where(v => v.GetTypeId() == viewFamilyTypeId)
                .ToList();
        }

        #endregion
    }
}