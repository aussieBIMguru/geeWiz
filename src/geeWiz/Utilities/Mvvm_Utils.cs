// System
using System.Windows;
using Window = System.Windows.Window;
using Visibility = System.Windows.Visibility;

// Note: WindowController class lifted from Nice3point approach
// Reference: https://github.com/Nice3point/RevitTemplates/blob/1be110f421801339e54bf3403306443245212221/samples/SingleProjectWpfModelessApplication/RevitAddIn/Utils/WindowController.cs#L12

// Using the utility namespace
namespace geeWiz.Utilities
{
    /// <summary>
    /// Controller for modeless windows, to be handled by the class.
    /// </summary>
    public static class WindowController
    {
        // Property - list of windows the controller handles
        private static readonly List<Window> ControlledWindows = new();

        /// <summary>
        /// Attempts to focus to a window of given type.
        /// </summary>
        /// <typeparam name="T">The type of window.</typeparam>
        /// <returns>True if found.</returns>
        public static bool Focus<T>() where T : Window
        {
            // Get type of window for comparison
            var type = typeof(T);

            // For each window in the controller...
            foreach (var window in ControlledWindows)
            {
                // If the window is the given type...
                if (window.GetType() == type)
                {
                    // Restore it if it is minimized
                    if (window.WindowState == WindowState.Minimized)
                    {
                        window.WindowState = WindowState.Normal;
                    }

                    // Show it if it is hidden
                    if (window.Visibility != Visibility.Visible)
                    {
                        window.Show();
                    }

                    // Focus to it
                    window.Focus();
                    return true;

                }
            }

            // If we didn't focus to it
            return false;
        }

        /// <summary>
        /// Registers a window to the controller.
        /// </summary>
        /// <param name="window">The window to register.</param>
        private static void RegisterWindow(Window window)
        {
            // Add the window
            ControlledWindows.Add(window);

            // When the window is closed...
            window.Closed += (sender, _) =>
            {
                // Remove it from the controller
                var modelessWindow = (Window)sender;
                ControlledWindows.Remove(modelessWindow);
            };
        }

        /// <summary>
        /// Opens a window and registers it to the controller.
        /// </summary>
        /// <param name="window">The window to show.</param>
        public static void Show(Window window)
        {
            // Register the window if it isn't controlled yet
            RegisterWindow(window);

            // Show the window
            window.Show();
        }

        /// <summary>
        /// Opens a window and registers it to the controller.
        /// </summary>
        /// <param name="window">The window to show.</param>
        /// <param name="handle">Child/parent pointer.</param>
        public static void Show(Window window, IntPtr handle)
        {
            // Register the window if it isn't controlled yet
            RegisterWindow(window);

            // Show the window, set pointer
            window.Show(handle);
        }

        /// <summary>
        /// Shows all windows of a given type.
        /// </summary>
        /// <typeparam name="T">The type of window(s) to show.</typeparam>
        public static void Show<T>() where T : Window
        {
            // Get type of window for comparison
            var type = typeof(T);

            // For each controlled window...
            foreach (var window in ControlledWindows)
            {
                // Show window if of given type
                if (window.GetType() == type)
                {
                    window.Show();
                }
            }
                
        }

        /// <summary>
        /// Hides windows of a given type.
        /// </summary>
        /// <typeparam name="T">The type of window(s) to hide.</typeparam>
        public static void Hide<T>() where T : Window
        {
            // Get type of window for comparison
            var type = typeof(T);

            // For each controlled window...
            foreach (var window in ControlledWindows)
            {
                // Hide window if of given type
                if (window.GetType() == type)
                {
                    window.Hide();
                }
            }
        }

        /// <summary>
        /// Closes windows of a given type.
        /// </summary>
        /// <typeparam name="T">The type of window(s) to close.</typeparam>
        public static void Close<T>() where T : Window
        {
            // Get type of window for comparison
            var type = typeof(T);

            // Work back through the windows
            // We do this so that we don't get an error on i if a window is removed
            for (var i = ControlledWindows.Count - 1; i >= 0; i--)
            {
                // Get the window
                var window = ControlledWindows[i];

                // Close window if of given type
                if (window.GetType() == type)
                {
                    window.Close();
                }  
            }
        }
    }

    /// <summary>
    /// Methods of this class generally relate to Mvvm based operations.
    /// </summary>
    public static class Mvvm_Utils
    {
        // Nothing yet, just a place for WindowController to live
    }
}