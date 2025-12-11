// System
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using MessageBox = System.Windows.MessageBox;
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

        // Constants for window styles
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        private const int WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

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
            Loaded += OnLoaded;

            // Associate to the model
            this.DataContext = viewModel;

            // Register the closing event as cancellation
            this.Closed += (s, e) =>
            {
                Cancel();
            };

            // Bind the progress value and set the range
            this.progressBar.SetBinding(ProgressBar.ValueProperty, "ProgressValue");
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = total;
        }

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

        #region Cancellation

        /// <summary>
        /// Runs when cancel button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Set flag, close window
            this.CancelledByUser = true;
            this.Close();
        }

        /// <summary>
        /// Runs when form is closed to tell the model we closed also.
        /// </summary>
        public void Cancel()
        {
            // Tell the model to run its closing function
            if (this.DataContext is Models.ModelProgress viewModel)
            {
                viewModel.CloseWindow(this.CancelMessage, this.CancelledByUser);
            }
            // Catch if the model is not valid
            else
            {
                MessageBox.Show("Error: DataContext is not set correctly.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Runs a dispatcher closure.
        /// </summary>
        public void CloseSafelyWithDispatcher()
        {
            // Close the current dispatcher
            if (!this.Dispatcher.HasShutdownStarted && !this.Dispatcher.HasShutdownFinished)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Close();
                    this.Dispatcher.InvokeShutdown();
                });
            }
        }

        #endregion
    }
}
