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
                var clipboardText = Clipboard.GetText();
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

        #region User folders

        /// <summary>
        /// Returns a path to a special folder.
        /// </summary>
        /// <returns>A string.</returns>
        public static string SpecialFolder(Environment.SpecialFolder folder)
        {
            return Environment.GetFolderPath(folder);
        }

        /// <summary>
        /// Returns a path to the user's desktop.
        /// </summary>
        /// <returns>A string.</returns>
        public static string Desktop()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        /// <summary>
        /// Returns a path to the user's documents folder.
        /// </summary>
        /// <returns>A string.</returns>
        public static string MyDocuments()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        /// <summary>
        /// Returns a path to the user's roaming folder.
        /// </summary>
        /// <returns>A string.</returns>
        public static string AppData()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        /// <summary>
        /// Returns a path to the user's folder.
        /// </summary>
        /// <returns>A string.</returns>
        public static string UserFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        #endregion

        #region DateTime

        /// <summary>
        /// Returns a YYMMDD prefix based on the current date.
        /// </summary>
        /// <returns>A string.</returns>
        public static string NowAsFormattedDate()
        {
            return DateTime.Now.ToString("yyMMdd");
        }

        /// <summary>
        /// Returns a HH-MM-SS suffix based on the current time.
        /// </summary>
        /// <param name="includeSeconds">Includes the seconds on the end.</param>
        /// <param name="separator">The separtor to use.</param>
        /// <returns>A string.</returns>
        public static string NowAsFormattedTime(bool includeSeconds = true, string separator = "-")
        {
            if (includeSeconds)
            {
                return DateTime.Now.ToString($"HH{separator}mm{separator}ss");
            }
            else
            {
                return DateTime.Now.ToString($"HH{separator}mm");
            }
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
                var commandId = RevitCommandId.LookupCommandId(commandName);
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