// System
using System.Runtime.InteropServices;
using Clipboard = System.Windows.Forms.Clipboard;
// Revit API
using Autodesk.Revit.UI;
// geeWiz utilities
using gFrm = geeWiz.Forms;

// The class belongs to the utility namespace
// using gScr = geeWiz.Utilities.ScriptUtils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to script behavior and states.
    /// </summary>
    public static class ScriptUtils
    {
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
        /// Verifies if the user is holding down the shift key.
        /// </summary>
        /// <returns>A boolean.</returns>
        public static bool ShiftKeyHeld()
        {
            return KeyboardHelper.KeyIsPressedShift();
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

        /// <summary>
        /// Returns a tooltip from the tooltips resource.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>The related tooltip, if found.</returns>
        public static string GetTooltip(string key)
        {
            if (Globals.Tooltips.TryGetValue(key, out string value))
            {
                return value;
            }
            return "Tooltip not found.";
        }
    }

    /// <summary>
    /// This class deals with the state of key presses.
    /// </summary>
    internal class KeyboardHelper
    {
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);
        private const int VK_SHIFT = 0x10; // Virtual key code for Shift key

        /// <summary>
        /// Returns if the shift key is held.
        /// </summary>
        public static bool KeyIsPressedShift()
        {
            return (GetKeyState(VK_SHIFT) & 0x8000) != 0;
        }
    }
}