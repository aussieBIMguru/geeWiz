// System
using Clipboard = System.Windows.Forms.Clipboard;
using System.Windows.Input;
// Revit API
using Autodesk.Revit.UI;
// geeWiz utilities
using gFrm = geeWiz.Forms;

// The class belongs to the utility namespace
// using gScr = geeWiz.Utilities.Script_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to script behavior and states.
    /// </summary>
    public static class Script_Utils
    {
        #region Clipboard send/receive

        /// <summary>
        /// Attempts to send a string to the clipboard.
        /// </summary>
        /// <param name="text">Text to send.</param>
        /// <param name="showMessage">Shows error messages (if any).</param>
        /// <returns>A result.</returns>
        [STAThread]
        public static Result ClipboardSend(string text, bool showMessage = true)
        {
            // Copy the text to the clipboard
            try
            {
                Clipboard.SetText(text);
                return Result.Succeeded;
            }
            // Catch if it could not be sent
            catch
            {
                // Optional message to user (assume script is cancelled)
                if (showMessage)
                {
                    gFrm.Custom.Cancelled("Clipboard could not be accessed.");
                }
                return Result.Failed;
            }
        }

        /// <summary>
        /// Attempts to receive text from the clipboard.
        /// </summary>
        /// <param name="showMessage">Shows error messages (if any).</param>
        /// <returns>A result.</returns>
        [STAThread]
        public static string ClipboardReceive(bool showMessage = true)
        {
            // Receive text from the clipboard
            try
            {
                string clipboardText = Clipboard.GetText();
                return clipboardText;
            }
            // Catch if it could not be received
            catch
            {
                // Optional message to user (assume script is cancelled)
                if (showMessage)
                {
                    gFrm.Custom.Cancelled("Clipboard could not be accessed.");
                }
                return null;
            }
        }

        #endregion

        #region Keyboard key checks

        /// <summary>
        /// Verifies if the user is holding down the shift key.
        /// </summary>
        /// <returns>A boolean.</returns>
        public static bool KeyHeldShift()
        {
            return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        }

        /// <summary>
        /// Verifies if the user is holding down the control key.
        /// </summary>
        /// <returns>A boolean.</returns>
        public static bool KeyHeldControl()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }

        /// <summary>
        /// Verifies if the user is holding down the alt key.
        /// </summary>
        /// <returns>A boolean.</returns>
        public static bool KeyHeldAlt()
        {
            return Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
        }

        #endregion

        #region Other

        /// <summary>
        /// Verifies if the user is the developer (change name as desired).
        /// </summary>
        /// <returns>A boolean.</returns>
        public static bool UserIsDeveloper()
        {
            return Globals.UsernameWindows.Equals("gavin.crump",
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Attempts to run a postable command.
        /// </summary>
        /// <param name="uiApp">The UI Application.</param>
        /// <param name="commandName">The name of the command to post.</param>
        /// <returns>A result.</returns>
        public static Result PostCommand(UIApplication uiApp, string commandName)
        {
            try
            {
                RevitCommandId commandId = RevitCommandId.LookupCommandId(commandName);
                uiApp.PostCommand(commandId);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        #endregion
    }
}