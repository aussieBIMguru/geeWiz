using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

// In the forms namespace
namespace geeWiz.Forms
{
    // Progress bar view class
    public partial class ProgressView : Window, IDisposable, INotifyPropertyChanged
    {
        // Add a handler so the progress updates as we go
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

        // On property changed event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // The string for our progress message
        public string ProgressText => $"Processing: {PbCount} of {PbTotal}";
        // Other properties such: Did the form close early, placeholder task, cancel token, delay
        public bool IsClosed { get; private set; }
        private Task taskDoEvent { get; set; }
        private CancellationTokenSource cancellationTokenSource;
        private int delay;

        // Initialize the class object
        public ProgressView(string title = "Progress bar", int total = 100, int delay = 50)
        {
            InitializeComponent();
            InitializeSize();
            // Set title and total count
            this.Title = title;
            this.progressBar.Maximum = total;
            this.delay = delay;
            // When the window closes, set IsClosed to true
            this.Closed += (s, e) => { IsClosed = true;};
            cancellationTokenSource = new CancellationTokenSource();
        }

        // Close the form and dispose of token when disposed
        public void Dispose()
        {
            if (!IsClosed) Close();
            cancellationTokenSource?.Dispose();
        }

        // Method to update the progress to given values
        public bool UpdateProgress(int countIn, int totalIn)
        {
            UpdateTaskDoEvent();
            // Pass in provided values
            PbCount = countIn;
            PbTotal = totalIn;
            // If count is larger than total, set maximum to count
            if (countIn >= totalIn)
            {
                progressBar.Maximum = countIn;
            }
            // Update graphical value of progress bar
            progressBar.Value = PbCount;
            // Return if form has been closed
            return IsClosed;
        }

        // Placeholder event
        private void UpdateTaskDoEvent()
        {
            if (taskDoEvent == null) taskDoEvent = GetTaskUpdateEvent();
            if (taskDoEvent.IsCompleted)
            {
                Show();
                DoEvents();
                taskDoEvent = null;
            }
        }

        // Runs between tasks, adds slight delay to simulate progress for quick events
        private Task GetTaskUpdateEvent()
        {
            return Task.Run(async () =>
            {
                try
                {
                    // This adds a slight delay between tasks
                    await Task.Delay(this.delay, cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    // Handle cancellation if needed
                }
            });
        }

        // Set the cursor and run events of form
        private void DoEvents()
        {
            System.Windows.Forms.Application.DoEvents();
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        }

        // Set the initial size of the form
        private void InitializeSize()
        {
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.Topmost = true;
            this.ShowInTaskbar = false;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        // If user hits the cancel button (not used)
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
            System.Windows.MessageBox.Show("Operation cancelled.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}