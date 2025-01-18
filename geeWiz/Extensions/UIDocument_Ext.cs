// System
using System.Diagnostics;
// Revit API
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;
// geeWiz
using gFrm = geeWiz.Forms;

// The class belongs to the extensions namespace
// UIDocument uiDoc.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Extension methods to the UIDocument class.
    /// </summary>
    public static class UIDocument_Ext
    {
        /// <summary>
        /// Activate and switch to the given Revit view.
        /// </summary>
        /// <param name="uiDoc">The UIDocument (extended).</param>
        /// <param name="view">The view to open.</param>
        /// <param name="showMessage">If you want to show error/completion messages.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_OpenView(this UIDocument uiDoc, View view, bool showMessage = true)
        {
            // Ensure both are valid
            if (uiDoc is null || view is null)
            {
                return Result.Failed;
            }

            // Catch if view is already active and opened
            if (uiDoc.ActiveGraphicalView.Id == view.Id)
            {
                if (showMessage)
                {
                    gFrm.Custom.Completed("View is already active and opened.");
                }
                return Result.Succeeded;
            }

            // Try to open the view and switch to it (even if opened)
            try
            {
                uiDoc.RequestViewChange(view);
                return Result.Succeeded;
            }
            catch
            {
                if (showMessage)
                {
                    gFrm.Custom.Cancelled("View could not be opened.");
                }
                return Result.Cancelled;
            }
        }
    }
}