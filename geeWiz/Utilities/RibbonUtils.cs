// System
using System.Diagnostics;
// Revit API
using Autodesk.Revit.UI;
using RibbonPanel = Autodesk.Revit.UI.RibbonPanel;

// The class belongs to the geeWiz namespace
// using gRib = geeWiz.Utilities.RibbonUtils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to ribbon setup.
    /// </summary>
    public static class RibbonUtils
    {
        /// <summary>
        /// Adds a new tab to Revit
        /// </summary>
        /// <param name="tabName">The name of the tab to create.</param>
        /// <returns>A Result object.</returns>
        public static Result CreateTab(string tabName)
        {
            // Try to create new tab
            try
            {
                Globals.UiCtlApp.CreateRibbonTab(tabName);
                return Result.Succeeded;
            }
            catch
            {
                // If we fail, it probably exists already by name
                return Result.Failed;
            }
        }

        /// <summary>
        /// Adds a new ribbon panel to a tab.
        /// </summary>
        /// <param name="tabName">The name of the tab.</param>
        /// /// <param name="panelName">The name of the panel.</param>
        /// <returns>A RibbonPanel.</returns>
        public static RibbonPanel CreatePanel(string tabName, string panelName)
        {
            // Try to add ribbon panel to tab
            // NOTE: Create your tab by name before making panels
            try
            {
                return Globals.UiCtlApp.CreateRibbonPanel(tabName, panelName);
            }
            catch (Exception ex)
            {
                // If we could not, it is likely an error
                Debug.WriteLine($"\nERROR: {panelName} not created.\nError message: {ex.Message}\n");
                return null;
            }
        }

        /// <summary>
        /// Gets a RibbonPanel from a Tab by names.
        /// </summary>
        /// <param name="tabName">The name of the tab to check.</param>
        /// /// <param name="panelName">The name of the panel to find.</param>
        /// <returns>A RibbonPanel.</returns>
        public static RibbonPanel GetPanelOnTab(string tabName, string panelName)
        {
            // The list of panels we will try to get
            List<RibbonPanel> panels;

            // Try to get panels of the tab by name
            try
            {
                panels = Globals.UiCtlApp.GetRibbonPanels(tabName);
            }
            catch (Exception ex)
            {
                // If we could not, it is likely an error
                Debug.WriteLine($"\nERROR: {tabName} not found.\nError message: {ex.Message}\n");
                return null;
            }

            // Retrieve the ribbon panel object
            foreach (RibbonPanel panel in panels)
            {
                if (panel.Name == panelName)
                {
                    return panel;
                }
            }

            // If we did not find a match, return null
            return null;
        }

        /// <summary>
        /// Retrieves a PushButton from a panel by name.
        /// </summary>
        /// <param name="panel">The panel object to search.</param>
        /// <param name="buttonName">The name of the button to find.</param>
        /// <returns>A PushButton object.</returns>
        public static PushButton GetButtonOnPanel(RibbonPanel panel, string buttonName)
        {
            // If we have no panel, early return
            if (panel == null) { return null; }

            // Check each item on panel, return any valid matches
            foreach (var item in panel.GetItems())
            {
                if (item is PushButton pushButton && pushButton.Name == buttonName)
                {
                    return pushButton;
                }
            }

            // If we did not find a match, return null
            return null;
        }
    }
}