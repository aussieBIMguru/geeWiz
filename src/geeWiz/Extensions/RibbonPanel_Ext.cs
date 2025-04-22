// System
using System.Diagnostics;
// Revit API
using Autodesk.Revit.UI;
using PushButton = Autodesk.Revit.UI.PushButton;
// geeWiz
using gRib = geeWiz.Utilities.Ribbon_Utils;

// The class belongs to the extensions namespace
// RibbonPanel ribbonPanel.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Extension methods to the RibbonPanel class.
    /// </summary>
    public static class RibbonPanel_Ext
    {
        #region Add PushButton to panel

        /// <summary>
        /// Adds a Pushbutton to the panel.
        /// </summary>
        /// <typeparam name="CommandClass">The related Command class.</typeparam>
        /// <param name="ribbonPanel">The RibbonPanel (extended).</param>
        /// <param name="buttonName">The name for the button.</param>
        /// <param name="availability">The availability name.</param>
        /// <param name="suffix">The icon suffix (none by default).</param>
        /// <returns>A Pushbutton object.</returns>
        public static PushButton Ext_AddPushButton<CommandClass>(this RibbonPanel ribbonPanel,
            string buttonName, string availability = "", string suffix = "")
        {
            // Return an error message if panel is null
            if (ribbonPanel == null)
            {
                Debug.WriteLine($"ERROR: {buttonName} not created, ribbonPanel was null.");
                return null;
            }

            // Make pushbuttondata
            var pushButtonData = gRib.NewPushButtonData<CommandClass>(buttonName);

            // Make pushbutton, add to panel
            if (ribbonPanel.AddItem(pushButtonData) is PushButton pushButton)
            {
                // If provided, set availability
                if (availability != "")
                {
                    pushButton.AvailabilityClassName = availability;
                }

                // Return the PushButton
                return pushButton;
            }
            // Return an error message if it could not be made
            else
            {
                Debug.WriteLine($"ERROR: Button could not be created ({buttonName})");
                return null;
            }
        }

        #endregion

        #region Add Pulldown to panel

        /// <summary>
        /// Creates a pulldownbutton on a panel.
        /// </summary>
        /// <param name="ribbonPanel">The RibbonPanel (extended).</param>
        /// <param name="buttonName">The displayed name of the pulldown.</param>
        /// <param name="nameSpace">The namespace the button relates to.</param>
        /// <param name="suffix">The icon suffix (none by default).</param>
        /// <returns>A pulldownButton object.</returns>
        public static PulldownButton Ext_AddPulldownButton(this RibbonPanel ribbonPanel, string buttonName, string nameSpace, string suffix = "")
        {
            // Return an error message if panel is null
            if (ribbonPanel == null)
            {
                Debug.WriteLine($"ERROR: {buttonName} not created, ribbonPanel was null.");
                return null;
            }

            // Make pulldownButtonData
            var pulldownButtonData = gRib.NewPulldownButtonData(buttonName, nameSpace);

            // Make pulldown, add to panel
            if (ribbonPanel.AddItem(pulldownButtonData) is PulldownButton pulldownButton)
            {
                // Return the pulldown
                return pulldownButton;
            }
            // Return an error message if it could not be made
            else
            {
                Debug.WriteLine($"ERROR: Pulldown could not be created ({buttonName})");
                return null;
            }
        }

        #endregion

        #region Get PushButton

        /// <summary>
        /// Returns a PushButton from a RibbonPanel.
        /// </summary>
        /// <param name="ribbonPanel">The RibbonPanel (extended).</param>
        /// <param name="buttonName">The name of the button to find.</param>
        /// <returns>A PushButton.</returns>
        public static PushButton Ext_GetPushButtonByName(this RibbonPanel ribbonPanel, string buttonName)
        {
            // For each panel in the tab
            foreach (RibbonItem ribbonItem in ribbonPanel.GetItems())
            {
                // If the name matches, return it
                if (ribbonItem.Name == buttonName && ribbonItem is PushButton pushButton)
                {
                    return pushButton;
                }
            }

            // If not found, we finally return null
            return null;
        }

        /// <summary>
        /// Returns a PushButton from a RibbonPanel.
        /// </summary>
        /// <param name="ribbonPanel">The RibbonPanel (extended).</param>
        /// <param name="buttonText">The text of the button to find.</param>
        /// <returns>A PushButton.</returns>
        public static PushButton Ext_GetPushButtonByText(this RibbonPanel ribbonPanel, string buttonText)
        {
            // For each panel in the tab
            foreach (RibbonItem ribbonItem in ribbonPanel.GetItems())
            {
                // If the name matches, return it
                if (ribbonItem.ItemText == buttonText && ribbonItem is PushButton pushButton)
                {
                    return pushButton;
                }
            }

            // If not found, we finally return null
            return null;
        }

        #endregion

        #region Get Pulldown

        /// <summary>
        /// Returns a PuslldownButton from a RibbonPanel.
        /// </summary>
        /// <param name="ribbonPanel">The RibbonPanel (extended).</param>
        /// <param name="buttonName">The name of the button to find.</param>
        /// <returns>A PushButton.</returns>
        public static PulldownButton Ext_GetPulldownButtonByName(this RibbonPanel ribbonPanel, string buttonName)
        {
            // For each panel in the tab
            foreach (RibbonItem ribbonItem in ribbonPanel.GetItems())
            {
                // If the name matches, return it
                if (ribbonItem.Name == buttonName && ribbonItem is PulldownButton pulldownButton)
                {
                    return pulldownButton;
                }
            }

            // If not found, we finally return null
            return null;
        }

        #endregion
    }
}