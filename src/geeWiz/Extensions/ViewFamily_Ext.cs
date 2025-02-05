// geeWiz
using gView = geeWiz.Utilities.View_Utils;

// The class belongs to the extensions namespace
// ViewFamily viewFamily.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to ViewFamilies.
    /// </summary>
    public static class ViewFamily_Ext
    {
        /// <summary>
        /// Gets equivalent ViewType of a given ViewFamily.
        /// </summary>
        /// <param name="viewType">A ViewType (extended).</param>
        /// <returns>A ViewType.</returns>
        public static ViewFamily Ext_ToViewFamily(this ViewType viewType)
        {
            if (gView.VIEWTYPES_GRAPHICAL.Contains(viewType))
            {
                return gView.VIEWFAMILIES_GRAPHICAL
                    [gView.VIEWTYPES_GRAPHICAL.IndexOf(viewType)];
            }
            return ViewFamily.Invalid;
        }
    }
}