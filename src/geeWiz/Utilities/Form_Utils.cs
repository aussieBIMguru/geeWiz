// System
using System.Windows;
using Visibility = System.Windows.Visibility;
using Window = System.Windows.Window;

// Note: WindowController class lifted from Nice3point approach
// Reference: https://github.com/Nice3point/RevitTemplates/blob/1be110f421801339e54bf3403306443245212221/samples/SingleProjectWpfModelessApplication/RevitAddIn/Utils/WindowController.cs#L12

// Using the utility namespace
namespace geeWiz.Utilities
{
    /// <summary>
    /// This class handles modeless windows.
    /// </summary>
    public static class WindowController
    {
        /// <summary>
        /// The windows handles by the controller.
        /// </summary>
        private static readonly List<Window> ControlledWindows = new();

        /// <summary>
        /// Attempts to focus to a window of given type.
        /// </summary>
        /// <typeparam name="T">The type of window.</typeparam>
        /// <returns>True if found.</returns>
        public static bool Focus<T>() where T : Window
        {
            // Get type of window for comparison
            Type type = typeof(T);

            // For each window in the controller...
            foreach (Window window in ControlledWindows)
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
                Window modelessWindow = (Window)sender;
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
        public static void ShowWindow(Window window)
        {
            // Register the window if it isn't controlled yet
            RegisterWindow(window);

            // Show the window
            window.Show();
        }

        /// <summary>
        /// Shows all windows of a given type.
        /// </summary>
        /// <typeparam name="T">The type of window(s) to show.</typeparam>
        public static void Show<T>() where T : Window
        {
            // Get type of window for comparison
            Type type = typeof(T);

            // For each controlled window...
            foreach (Window window in ControlledWindows)
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
            Type type = typeof(T);

            // For each controlled window...
            foreach (Window window in ControlledWindows)
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
            Type type = typeof(T);

            // Work back through the windows
            // We do this so that we don't get an error on i if a window is removed
            for (var i = ControlledWindows.Count - 1; i >= 0; i--)
            {
                // Get the window
                Window window = ControlledWindows[i];

                // Close window if of given type
                if (window.GetType() == type)
                {
                    window.Close();
                }  
            }
        }
    }

    /// <summary>
    /// Static methods container related to Forms.
    /// </summary>
    public static class Form_Utils
    {
        /// <summary>
        /// Sets the selection behavior of a listbox in Wpf.
        /// </summary>
        /// <param name="multiSelect">If we want multiselection behavior.</param>
        /// <param name="listBox">The related listbox.</param>
        /// <param name="checkAllButton">Optional button for check all.</param>
        /// <param name="uncheckAllButton">Optional button for uncheck all.</param>
        /// <returns>The name of the item template to use.</returns>
        public static string Wpf_SetListBoxMode(bool multiSelect, System.Windows.Controls.ListBox listBox,
            System.Windows.Controls.Button checkAllButton = null, System.Windows.Controls.Button uncheckAllButton = null)
        {
            // Set state of check all buttons (single select = off)
            checkAllButton?.IsEnabled = multiSelect;
            uncheckAllButton?.IsEnabled = multiSelect;

            // Return resource and set the behavior of the listbox
            if (multiSelect)
            {
                listBox.SelectionMode = System.Windows.Controls.SelectionMode.Extended;
                return "DataTemplate_MultiSelect";
            }
            else
            {
                listBox.SelectionMode = System.Windows.Controls.SelectionMode.Single;
                return "DataTemplate_SingleSelect";
            }
        }

        /// <summary>
        /// Runs a shift click process on a listbox.
        /// </summary>
        /// <typeparam name="T">The type of object bound to the checkbox.</typeparam>
        /// <param name="sender">The </param>
        /// <param name="multiSelect"></param>
        /// <param name="listBox"></param>
        public static void Wpf_ShiftClickProcess<T>(object sender, bool multiSelect, System.Windows.Controls.ListBox listBox)
        {
            // Stop here if we are single selecting
            if (!multiSelect) { return; }

            // Ensure a valid check box sent the event
            if (sender is not System.Windows.Controls.CheckBox cb) { return; }
            if (cb.DataContext is not KeyedValue<T> clickedItem) { return; }

            // State to assign to other selected objects
            bool newState = cb.IsChecked == true;

            // Switch to checkbox if it was not selected
            if (!listBox.SelectedItems.Contains(clickedItem))
            {
                listBox.SelectedItems.Clear();
                listBox.SelectedItem = clickedItem;
            }

            // Apply the state to all selected items
            foreach (var obj in listBox.SelectedItems)
            {
                if (obj is KeyedValue<T> t)
                {
                    t.Checked = newState;
                }
            }

            // Force UI to refresh all item states
            listBox.Items.Refresh();
        }
    }
}