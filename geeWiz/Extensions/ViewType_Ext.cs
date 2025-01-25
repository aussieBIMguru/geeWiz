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
    }
}