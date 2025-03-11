// System
using System.Diagnostics;
// Revit API
using Autodesk.Revit.UI;
using PushButton = Autodesk.Revit.UI.PushButton;
// geeWiz
using gFil = geeWiz.Utilities.File_Utils;
using gRib = geeWiz.Utilities.Ribbon_Utils;

// The class belongs to the extensions namespace
// PulldownButton pulldownPanel.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Extension methods to the PulldownButton class.
    /// </summary>
    public static class PulldownButton_Ext
    {
        #region Add PushButton to pulldown

        /// <summary>
        /// Adds a Pushbutton to the pulldown.
        /// </summary>
        /// <param name="pulldownButton">The PulldownButton (extended).</param>
        /// <param name="buttonName">The name for the button.</param>
        /// <param name="commandClass">The command the button will run.</param>
        /// <param name="availability">The availability name.</param>
        /// <param name="suffix">The icon suffix (if any).</param>
        /// <returns>A Pushbutton object.</returns>
        public static PushButton Ext_AddPushButton(this PulldownButton pulldownButton, string buttonName, string commandClass, string availability = "", string suffix = "")
        {
            // Return an error message if the pulldownbutton is null
            if (pulldownButton == null)
            {
                Debug.WriteLine($"ERROR: {buttonName} not created, pulldownButton was null.");
                return null;
            }

            // Deconstruct the commmandclass into its basic name, to align to tooltips and icons
            string baseName = gRib.CommandClassToBaseName(commandClass);

            // Make pushbuttondata
            PushButtonData pushButtonData = new PushButtonData(baseName, buttonName, Globals.AssemblyPath, commandClass);

            // Make pushbutton, add to panel
            if (pulldownButton.AddPushButton(pushButtonData) is PushButton pushButton)
            {
                // If provided, set availability
                if (availability != "")
                {
                    pushButton.AvailabilityClassName = availability;
                }

                // Set tooltip and icons
                pushButton.ToolTip = gFil.GetDictValue(Globals.Tooltips, baseName);
                pushButton.LargeImage = gFil.GetImageSource(baseName, resolution: 32, suffix: suffix);
                pushButton.Image = gFil.GetImageSource(baseName, resolution: 16, suffix: suffix);

                // Return the PushButton
                return pushButton;
            }
            // Return an error message if it could not be made
            else
            {
                Debug.WriteLine($"ERROR: Button could not be created ({commandClass})");
                return null;
            }
        }

        #endregion

        #region Get PushButton on pulldown

        /// <summary>
        /// Returns a PushButton from a PulldownButton.
        /// </summary>
        /// <param name="pulldownButton">The PulldownButton (extended).</param>
        /// <param name="buttonName">The name of the button to search for.</param>
        /// <returns>A PushButton.</returns>
        public static PushButton Ext_GetPushButtonByName(this PulldownButton pulldownButton, string buttonName)
        {
            // For each item in the pulldownButton
            foreach (PushButton pushButton in pulldownButton.GetItems())
            {
                // If the name matches, return it
                if (pushButton.Name == buttonName)
                {
                    return pushButton;
                }
            }

            // If not found, we finally return null
            return null;
        }

        /// <summary>
        /// Returns a PushButton from a PulldownButton.
        /// </summary>
        /// <param name="pulldownButton">The PulldownButton (extended).</param>
        /// <param name="buttonText">The text of the button to search for.</param>
        /// <returns>A PushButton.</returns>
        public static PushButton Ext_GetPushButtonByText(this PulldownButton pulldownButton, string buttonText)
        {
            // For each item in the pulldownButton
            foreach (PushButton pushButton in pulldownButton.GetItems())
            {
                // If the name matches, return it
                if (pushButton.ItemText == buttonText)
                {
                    return pushButton;
                }
            }

            // If not found, we finally return null
            return null;
        }

        #endregion
    }
}