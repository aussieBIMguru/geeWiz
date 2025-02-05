// System
using System.ComponentModel;
using System.Windows;

// The base form will belong to the forms namespace
namespace geeWiz.Forms
{
    #region Class summary

    /// <summary>
    /// Standard class for showing the progress bar Wpf form.
    /// 
    /// Use UpdateProgress() to uptick the progress bar.
    /// Will close when cancelled or maximum tick reached.
    /// 
    /// Will run on a separate thread so it can be cancelled even if Revit is idle.
    /// </summary>

    #endregion

    public partial class ProgressView : Window, IDisposable, INotifyPropertyChanged
    {
        #region Class properties and handlers

        // Create an event when the property changes of the progress bar
        public event PropertyChangedEventHandler PropertyChanged;

        // Handling/detecting change in count, listeners
        private int pb_count;
        public int PbCount
        {
            get => pb_count;
            set
            {
                pb_count = value;
                OnPropertyChanged(nameof(PbCount)); // Update Pbcount when count changes
                OnPropertyChanged(nameof(ProgressText)); // Update label when count changes
            }
        }

        // Handling/detecting change in total, listeners
        private int pb_total;
        public int PbTotal
        {
            get => pb_total;
            set
            {
                pb_total = value;
                OnPropertyChanged(nameof(PbTotal)); // Update PbTotal when count changes
                OnPropertyChanged(nameof(ProgressText)); // Update label when total changes
            }
        }

        // When properties change, update the displayed values
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // The process message
        public string ProgressText => $"Processing: {PbCount} of {PbTotal}";

        // Other properties such: Did the form close early, async task, cancel token, delay
        public bool IsClosed { get; private set; }
        private Task taskDoEvent { get; set; }
        private CancellationTokenSource cancellationTokenSource;
        private int delay;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the progress view class.
        /// </summary>
        /// <param name="title"">The title for the form.</param>
        /// <param name="total"">The total number of ticks to progress through.</param>
        /// <param name="delay"">The delay between ticks (in ms), in addition to related processing.</param>
        /// <returns>A progress bar object.</returns>
        public ProgressView(string title = "Progress bar", int total = 100, int delay = 50)
        {
            // Establish the form and size
            InitializeComponent();
            InitializeSize();

            // Pass the properties to the form
            this.Title = title;
            this.progressBar.Maximum = total;
            this.delay = delay;

            // Set the IsClosed property when the form closes
            this.Closed += (s, e) => { IsClosed = true;};

            // Apply a cancellation token
            cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Locate and size the progress bar.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        private void InitializeSize()
        {
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.Topmost = true;
            this.ShowInTaskbar = false;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        #endregion

        #region Disposer

        /// <summary>
        /// Runs when the form closes for any reason (if it is open).
        /// </summary>
        /// <returns>Void (nothing).</returns>
        public void Dispose()
        {
            if (!this.IsClosed) Close();
            cancellationTokenSource?.Dispose();
        }

        #endregion

        #region Click Cancel button

        /// <summary>
        /// Runs when the user triggers the cancel button manually.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Process the cancellation token
            cancellationTokenSource.Cancel();
            
            // Form to the user
            System.Windows.MessageBox.Show(messageBoxText: "Process cancelled.",
                caption: "Cancelled",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            
            // Close the form (also runs the disposal)
            this.Close();
        }

        #endregion

        #region Update/check for cancellation

        /// <summary>
        /// Update the progress bar values displayed, catching cancellation.
        /// </summary>
        /// <param name="countIn"">The count to display.</param>
        /// <param name="totalIn"">The total to display.</param>
        /// <returns>A boolean (if the form has been cancelled/closed).</returns>
        public bool UpdateProgress(int countIn, int totalIn)
        {
            // Trigger do events
            UpdateTaskDoEvent();

            // Update the public properties of the form
            PbCount = countIn;
            PbTotal = totalIn;

            // Ensure maximum is not exceeded
            if (countIn >= totalIn)
            {
                progressBar.Maximum = countIn;
            }

            // Update graphical value of progress bar
            progressBar.Value = PbCount;

            // Return if the form has closed, to catch early cancellation
            return IsClosed;
        }

        #endregion

        #region Run async task

        /// <summary>
        /// Verify if we can run the delayed tick, run it if we can.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        private void UpdateTaskDoEvent()
        {
            // Run the async task if possible
            if (taskDoEvent == null)
            {
                taskDoEvent = GetTaskUpdateEvent();
            }

            // If we completed the task, do events and reset task
            if (taskDoEvent.IsCompleted)
            {
                Show();
                DoEvents();
                taskDoEvent = null;
            }
        }

        /// <summary>
        /// Process the delay between ticks.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        private Task GetTaskUpdateEvent()
        {
            // Run async progress bar tick
            return Task.Run(async () =>
            {
                // Try to add the delay if not cancelled
                try
                {
                    await Task.Delay(this.delay, cancellationTokenSource.Token);
                }
                // If we can't, do nothing
                catch (TaskCanceledException)
                {
                    {; }
                }
            });
        }

        /// <summary>
        /// Generic do events.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        private void DoEvents()
        {
            // Do events on the other thread
            System.Windows.Forms.Application.DoEvents();

            // Update the cursor to the wait cursor
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        }

        #endregion
    }
}