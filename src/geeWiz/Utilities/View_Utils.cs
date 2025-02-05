// No dependencies yet

// The class belongs to the utility namespace
// using gView = geeWiz.Utilities.View_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to view based operations.
    /// </summary>
    public static class View_Utils
    {
        #region Constants - ViewType

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

        #endregion

        #region Constants - ViewFamily

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

        #endregion

        #region Export options

        /// <summary>
        /// Return default PDF export options.
        /// </summary>
        /// <param name="hideCrop"">Hide crop boundaries.</param>
        /// <returns>A PDFExportOptions object.</returns>
        public static PDFExportOptions DefaultPdfExportOptions(bool hideCrop = true)
        {
            // New options
            var options = new PDFExportOptions();

            // Configure the settings
            options.AlwaysUseRaster = false;
            options.ColorDepth = ColorDepthType.Color;
            options.ExportQuality = PDFExportQualityType.DPI300;
            options.HideCropBoundaries = hideCrop;
            options.HideReferencePlane = true;
            options.HideScopeBoxes = true;
            options.HideUnreferencedViewTags = true;
            options.MaskCoincidentLines = true;
            options.PaperFormat = ExportPaperFormat.Default;
            options.PaperOrientation = PageOrientationType.Auto;
            options.RasterQuality = RasterQualityType.High;
            options.ReplaceHalftoneWithThinLines = true;
            options.StopOnError = false;
            options.ViewLinksInBlue = false;
            options.ZoomPercentage = 100;
            options.ZoomType = ZoomType.Zoom;

            // Return the options
            return options;
        }

        /// <summary>
        /// Return default DWG export options.
        /// </summary>
        /// <param name="shared"">Export as shared coordinates.</param>
        /// <returns>A DWGExportOptions object.</returns>
        public static DWGExportOptions DefaultDwgExportOptions(bool shared = false)
        {
            // New options
            var options = new DWGExportOptions();

            // Configure the settings
            options.SharedCoords = shared;
            options.MergedViews = true;
            options.FileVersion = ACADVersion.R2013;
            
            // Return the options
            return options;
        }

        #endregion
    }
}