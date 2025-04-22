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
    }
}