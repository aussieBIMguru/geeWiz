// Revit API
using Autodesk.Revit.DB;

// The class belongs to the utility namespace
// using gView = geeWiz.Utilities.View_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Static methods container related to Views.
    /// </summary>
    public static class View_Utils
    {
        #region Constants - ViewType

        /// <summary>
        /// Graphical view related ViewType values.
        /// </summary>
        public static readonly List<ViewType> VIEWTYPES_GRAPHICAL = new List<ViewType>()
        {
            ViewType.AreaPlan, ViewType.CeilingPlan, ViewType.Detail, ViewType.DraftingView,
            ViewType.Elevation, ViewType.EngineeringPlan, ViewType.FloorPlan, ViewType.Section,
            ViewType.ThreeD, ViewType.Rendering, ViewType.Walkthrough
        };

        /// <summary>
        /// Plan view related ViewType values.
        /// </summary>
        public static readonly List<ViewType> VIEWTYPES_PLAN = new List<ViewType>()
        {
            ViewType.AreaPlan, ViewType.CeilingPlan, ViewType.EngineeringPlan, ViewType.FloorPlan
        };

        #endregion

        #region Constants - ViewFamily

        /// <summary>
        /// Graphical view related ViewFamily values.
        /// </summary>
        public static readonly List<ViewFamily> VIEWFAMILIES_GRAPHICAL = new List<ViewFamily>()
        {
            ViewFamily.AreaPlan, ViewFamily.CeilingPlan, ViewFamily.Detail, ViewFamily.Drafting,
            ViewFamily.Elevation, ViewFamily.StructuralPlan, ViewFamily.FloorPlan, ViewFamily.Section,
            ViewFamily.ThreeDimensional, ViewFamily.ImageView, ViewFamily.Walkthrough
        };

        /// <summary>
        /// Plan view related ViewFamily values.
        /// </summary>
        public static readonly List<ViewFamily> VIEWFAMILIES_PLAN = new List<ViewFamily>()
        {
            ViewFamily.AreaPlan, ViewFamily.CeilingPlan, ViewFamily.StructuralPlan, ViewFamily.FloorPlan
        };

        #endregion

        #region Export options

        /// <summary>
        /// Return default PDF export options.
        /// </summary>
        /// <param name="hideCrop">Hide crop boundaries.</param>
        /// <returns>A PDFExportOptions object.</returns>
        public static PDFExportOptions DefaultPdfExportOptions(bool hideCrop = true)
        {
            // New options
            return new PDFExportOptions()
            {
                AlwaysUseRaster = false,
                ColorDepth = ColorDepthType.Color,
                ExportQuality = PDFExportQualityType.DPI300,
                HideCropBoundaries = hideCrop,
                HideReferencePlane = true,
                HideScopeBoxes = true,
                HideUnreferencedViewTags = true,
                MaskCoincidentLines = true,
                PaperFormat = ExportPaperFormat.Default,
                PaperOrientation = PageOrientationType.Auto,
                RasterQuality = RasterQualityType.High,
                ReplaceHalftoneWithThinLines = true,
                StopOnError = false,
                ViewLinksInBlue = false,
                ZoomPercentage = 100,
                ZoomType = ZoomType.Zoom
            };
        }

        /// <summary>
        /// Return default DWG export options.
        /// </summary>
        /// <param name="shared">Export as shared coordinates.</param>
        /// <returns>A DWGExportOptions object.</returns>
        public static DWGExportOptions DefaultDwgExportOptions(bool shared = false)
        {
            // New options
            return new DWGExportOptions()
            {
                SharedCoords = shared,
                MergedViews = true,
                FileVersion = ACADVersion.R2013
            };
        }

        #endregion
    }
}