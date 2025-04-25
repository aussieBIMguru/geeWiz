// geeWiz
using Autodesk.Revit.UI;

// The class belongs to the extensions namespace
// UIApplication uiApp.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to the UIApplication.
    /// </summary>
    public static class UIApplication_Ext
    {
        #region Family document actions

        /// <summary>
        /// Opens a Revit family file from a file path.
        /// </summary>
        /// <param name="uiApp">The UiApp (extended).</param>
        /// <param name="filePath">The file path to open.</param>
        /// <param name="ensureFamily">Only opens the path if it ends with ".rfa".</param>
        /// <returns>A Document.</returns>
        public static Document Ext_OpenFamilyFileFromPath(this UIApplication uiApp, string filePath, bool ensureFamily = true)
        {
            // Ensure it is a family file
            if (!filePath.EndsWith(".rfa") && ensureFamily) { return null; }
            
            // Try to open the file
            try
            {
                // Return as a document
                return uiApp.Application.OpenDocumentFile(filePath);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}