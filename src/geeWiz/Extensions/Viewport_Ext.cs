// Autodesk
using View = Autodesk.Revit.DB.View;

// The class belongs to the extensions namespace
// Viewport viewport.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to viewports.
    /// </summary>
    public static class Viewport_Ext
    {
        // Category of base point for view isolation
        private static readonly ElementId CATEGORYID_BASEPOINT = new ElementId(BuiltInCategory.OST_ProjectBasePoint);

        #region Owner view and sheet

        /// <summary>
        /// Returns the related view of a viewport.
        /// </summary>
        /// <param name="viewport">The viewport (extended).</param>
        /// <returns>A View.</returns>
        public static View Ext_GetView(this Viewport viewport)
        {
            // Null check
            if (viewport == null) { return null; }

            // Return view
            return viewport.ViewId.Ext_GetElement(viewport.Document) as View;
        }

        /// <summary>
        /// Returns the related sheet of a viewport.
        /// </summary>
        /// <param name="viewport">The viewport (extended).</param>
        /// <returns>A Sheet.</returns>
        public static ViewSheet Ext_GetSheet(this Viewport viewport)
        {
            // Null check
            if (viewport == null) { return null; }

            // Return view
            return viewport.SheetId.Ext_GetElement(viewport.Document) as ViewSheet;
        }

        #endregion

        #region Box centre

        /// <summary>
        /// Get the box centre of a viewport.
        /// </summary>
        /// <param name="viewport">The viewport (extended).</param>
        /// <param name="doc">A document (optional).</param>
        /// <returns>An XYZ.</returns>
        public static XYZ Ext_GetViewportCentre(this Viewport viewport, Document doc = null)
        {
            // Get document if null
            doc ??= viewport.Document;

            // Isolate all elements in viewports view
            var viewportView = viewport.ViewId.Ext_GetElement<View>(doc);
            viewportView.IsolateCategoryTemporary(CATEGORYID_BASEPOINT);

            // Get actual box centre
            var vpBoxCenter = viewport.GetBoxCenter();

            // Disable temporary view override
            viewportView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);

            // Return box centre
            return vpBoxCenter;
        }

        #endregion
    }
}