// Autodesk
using View = Autodesk.Revit.DB.View;
// geeWiz
using gView = geeWiz.Utilities.View_Utils;

// The class belongs to the extensions namespace
// ViewType viewType.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to ViewTypes.
    /// </summary>
    public static class ViewType_Ext
    {
        #region ViewFamily to ViewType

        /// <summary>
        /// Gets equivalent ViewFamily of a given ViewType.
        /// </summary>
        /// <param name="viewFamily">A ViewFamily (extended).</param>
        /// <returns>A ViewType.</returns>
        public static ViewType Ext_ToViewType(this ViewFamily viewFamily)
        {
            if (gView.VIEWFAMILIES_GRAPHICAL.Contains(viewFamily))
            {
                return gView.VIEWTYPES_GRAPHICAL
                    [gView.VIEWFAMILIES_GRAPHICAL.IndexOf(viewFamily)];
            }
            return ViewType.Undefined;
        }

        #endregion

        #region Collect views

        /// <summary>
        /// Gets all views of the given ViewType.
        /// </summary>
        /// <param name="viewType">The ViewType (extended).</param>
        /// <param name="doc">The document.</param>
        /// <param name="sorted">Sort views by name.</param>
        /// <returns>A list of views.</returns>
        public static List<View> Ext_AllViewsOfType(this ViewType viewType, Document doc = null, bool sorted = true)
        {
            // Get current document
            doc ??= Globals.UiApp.ActiveUIDocument.Document;
            
            // Get all views of given view type
            return doc.Ext_GetViews(new List<ViewType>() { viewType }, sorted: sorted);
        }

        #endregion
    }
}