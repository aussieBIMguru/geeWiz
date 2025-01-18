// geeWiz (for extensions)
using gView = geeWiz.Utilities.ViewUtils;

// The class belongs to the utility namespace
// using gView = geeWiz.Utilities.ViewUtils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to view based operations.
    /// </summary>
    public static class ViewUtils
    {
        // All view types which generally correlate to a graphical view
        public static readonly List<ViewType> VIEWTYPES_GRAPHICAL = new List<ViewType>()
        {
            ViewType.AreaPlan, ViewType.CeilingPlan, ViewType.Detail, ViewType.DraftingView,
            ViewType.Elevation, ViewType.EngineeringPlan, ViewType.FloorPlan, ViewType.Section,
            ViewType.ThreeD, ViewType.Rendering, ViewType.Walkthrough
        };

        // All view types which generally correlate to a plan view
        public static readonly List<ViewType> VIEWTYPES_PLAN = new List<ViewType>()
        {
            ViewType.AreaPlan, ViewType.CeilingPlan, ViewType.EngineeringPlan, ViewType.FloorPlan
        };

        // All view families which generally correlate to a graphical view (aligned to view types)
        public static readonly List<ViewFamily> VIEWFAMILIES_GRAPHICAL = new List<ViewFamily>()
        {
            ViewFamily.AreaPlan, ViewFamily.CeilingPlan, ViewFamily.Detail, ViewFamily.Drafting,
            ViewFamily.Elevation, ViewFamily.StructuralPlan, ViewFamily.FloorPlan, ViewFamily.Section,
            ViewFamily.ThreeDimensional, ViewFamily.ImageView, ViewFamily.Walkthrough
        };

        // All view families which generally correlate to a plan view (aligned to view types)
        public static readonly List<ViewFamily> VIEWFAMILIES_PLAN = new List<ViewFamily>()
        {
            ViewFamily.AreaPlan, ViewFamily.CeilingPlan, ViewFamily.StructuralPlan, ViewFamily.FloorPlan
        };
    }
}

// The class belongs to the utility namespace
// using gView = geeWiz.Utilities.ViewUtils
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to views and related classes.
    /// </summary>
    public static class View_Ext
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