// System
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using ProgressBar = System.Windows.Controls.ProgressBar;

// Using the Mvvm namespace
namespace geeWiz.Forms.Mvvm.Views
{
    /// <summary>
    /// Manages the Mvvm model.
    /// </summary>
    public partial class ViewProgress : Window
    {
        #region Properties

        public string CancelMessage { get; set; }
        public string TaskName { get; set; }
        public bool CancelledByUser { get; set; }

        #endregion

        #region Control bar properties

        // AI Written - no idea how it actually works...
        // The point of all of this is to block access to minimize/close controls
        // We only want user to cancel via the provided button we can subscribe to

        // Constants for window styles
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        private const int WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Executes when the form loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // AI Written - removes the close button in principle
            var hwnd = new WindowInteropHelper(this).Handle;
            int style = GetWindowLong(hwnd, GWL_STYLE);
            SetWindowLong(hwnd, GWL_STYLE, (style & ~WS_SYSMENU) | WS_MINIMIZEBOX);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Form constructor.
        /// </summary>
        /// <param name="viewModel">ViewModel to relate to the view.</param>
        /// <param name="total">Total steps to take.</param>
        /// <param name="title">Title of the form.</param>
        /// <param name="taskName">Taskname to display.</param>
        /// <param name="cancelMessage">Optional message on cancellation.</param>
        public ViewProgress(Models.ModelProgress viewModel, int total, string title = null, string taskName = null, string cancelMessage = null)
        {
            // Set title and task name
            title ??= "Progress bar";
            taskName ??= "Running task";
            this.Title = title;
            this.TaskName = taskName;

            // Set cancel state as false by default
            this.CancelMessage = cancelMessage;
            this.CancelledByUser = false;

            // Initialize the form
            InitializeComponent();

            // Handle the removal of the default controls
            this.Loaded += OnLoaded;

            // Associate to the model
            this.DataContext = viewModel;

            // Register the closing event as cancellation
            // This is here to guard if the user closes the form without our button
            this.Closed += (s, e) =>
            {
                this.Cancel(cancelledByUser: false);
            };

            // Bind the progress value and set the range
            this.progressBar.SetBinding(ProgressBar.ValueProperty, "ProgressValue");
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = total;
        }

        #endregion

        #region Cancellation

        /// <summary>
        /// Runs when cancel button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Set flag, close window
            this.CancelledByUser = true;
            this.Cancel(cancelledByUser: true);
        }

        /// <summary>
        /// Runs when form is closed to tell the model we closed also.
        /// </summary>
        public void Cancel(bool cancelledByUser = false)
        {
            // If the model is valid (nearly always should be)...
            if (this.DataContext is Models.ModelProgress viewModel)
            {
                // Close the model
                viewModel.CloseWindow(cancelledByUser: cancelledByUser);
            }
        }

        /// <summary>
        /// Run an action on the View thread safely.
        /// </summary>
        /// <param name="action"></param>
        public void ThreadSafeAction(Action action)
        {
            // If we are on the UI thread, run the action
            if (this.Dispatcher.CheckAccess())
            {
                action();
            }
            // If not, queue the action to run on the UI thread when available
            else if (!this.Dispatcher.HasShutdownStarted && !this.Dispatcher.HasShutdownFinished)
            {
                this.Dispatcher.BeginInvoke(action);
            }
        }

        /// <summary>
        /// Runs a dispatcher closure.
        /// </summary>
        public void CloseThreadSafe()
        {
            // If the dispatcher is shutting down, do nothing
            if (this.Dispatcher.HasShutdownStarted || this.Dispatcher.HasShutdownFinished)
            {
                return;
            }

            // If the view is visible, close it
            if (this.IsVisible)
            {
                this.Close();
            }

            // If the dispatcher is still available and running, shut it down
            if (!this.Dispatcher.HasShutdownStarted && !this.Dispatcher.HasShutdownFinished)
            {
                this.Dispatcher.InvokeShutdown();
            }
        }

        #endregion
    }
}
