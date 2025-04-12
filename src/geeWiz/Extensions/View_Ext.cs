// Autodesk
using View = Autodesk.Revit.DB.View;
// geeWiz
using gFrm = geeWiz.Forms;

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
    }
}