// System
using System.Diagnostics;
// Revit API
using Autodesk.Revit.UI;

// The class belongs to the extensions namespace
// UIControlledApplication uiApp.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Extension methods to the UIControlledApplication class.
    /// </summary>
    public static class UIControlledApplication_Ext
    {
        #region Tabs

        /// <summary>
        /// Adds a new tab to the Ribbon.
        /// </summary>
        /// <param name="uiApp">The UIControlledApplication (extended).</param>
        /// <param name="tabName">The name of the tab to add.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_AddRibbonTab(this UIControlledApplication uiApp, string tabName)
        {
            // Try to create new tab
            try
            {
                uiApp.CreateRibbonTab(tabName);
                return Result.Succeeded;
            }
            // Debug error if it failed (probably already exists)
            catch
            {
                Debug.WriteLine($"ERROR: Tab '{tabName}' could not be created.");
                return Result.Failed;
            }
        }

        #endregion

        #region RibbonPanels

        /// <summary>
        /// Adds a new RibbonPanel to the tab.
        /// </summary>
        /// <param name="uiApp">The UIControlledApplication (extended).</param>
        /// <param name="tabName">The name of the tab on which to add the panel.</param>
        /// <param name="panelName">The name of the panel to create.</param>
        /// <returns>A RibbonPanel.</returns>
        public static RibbonPanel Ext_AddRibbonPanelToTab(this UIControlledApplication uiApp, string tabName, string panelName)
        {
            // Try to create the RibbonPanel
            try
            {
                uiApp.CreateRibbonPanel(tabName, panelName);
            }
            // Debug error if it failed (probably already exists)
            catch
            {
                Debug.WriteLine($"ERROR: RibbonPanel '{panelName}' could not be created on Tab '{tabName}'");
            }

            // Get the newly created ribbon panel
            return uiApp.Ext_GetRibbonPanelByName(panelName, tabName);
        }

        /// <summary>
        /// Returns a RibbonPanel from a tab.
        /// </summary>
        /// <param name="uiApp">The UIControlledApplication (extended).</param>
        /// <param name="panelName">The name of the panel to search for.</param>
        /// <param name="tabName">The name of the tab on which to search (geeWiz by default).</param>
        /// <returns>A RibbonPanel.</returns>
        public static RibbonPanel Ext_GetRibbonPanelByName(this UIControlledApplication uiApp, string panelName, string tabName = "geeWiz")
        {
            // For each panel in the tab
            foreach (RibbonPanel ribbonPanel in uiApp.GetRibbonPanels(tabName))
            {
                // If the name matches, return it
                if (ribbonPanel.Name == panelName)
                {
                    return ribbonPanel;
                }
            }

            // If not found, we finally return null
            return null;
        }

        #endregion
    }
}