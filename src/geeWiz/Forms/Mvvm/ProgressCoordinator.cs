// System
using System.Windows.Threading;
using Dispatcher = System.Windows.Threading.Dispatcher;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;
// Revit API
using Autodesk.Revit.DB;
// geeWiz
using geeWiz.Extensions;

// Belongs to the forms namespace
namespace geeWiz.Forms
{
    /// <summary>
    /// MVC Progress bar coordinator.
    /// </summary>
    public class ProgressCoordinator
    {
        #region Properties

        /// <summary>
        /// Related View to the coordinator.
        /// </summary>
        private Mvvm.Views.ViewProgress _view { get; set; }

        /// <summary>
        /// Related Model to the coordinator.
        /// </summary>
        private Mvvm.Models.ModelProgress _model { get; set; }

        /// <summary>
        /// Related Thread to the coordinator.
        /// </summary>
        private Thread _thread { get; set; }

        /// <summary>
        /// Title of the task.
        /// </summary>
        private string _title { get; set; }

        /// <summary>
        /// Description of the task.
        /// </summary>
        private string _taskName { get; set; }

        /// <summary>
        /// Total number of steps to take.
        /// </summary>
        private int _totalSteps { get; set; }

        /// <summary>
        /// Message to show if cancelled.
        /// </summary>
        private string _cancelMessage { get; set; }

        /// <summary>
        /// Delay between steps in ms.
        /// </summary>
        private int _stepDelay { get; set; }

        /// <summary>
        /// If the progress bar has been cancelled by the user.
        /// </summary>
        public bool CancelledByUser { get; set; }

        /// <summary>
        /// The related thread dispatcher.
        /// </summary>
        private Dispatcher _dispatcher { get; set; }

        /// <summary>
        /// Has shutdown of the thread been requested yet.
        /// </summary>
        private bool _shutdownRequested { get; set; } = false;

        /// <summary>
        /// Has the coordinator been intialized yet.
        /// </summary>
        private readonly ManualResetEventSlim _initialised = new ManualResetEventSlim(false);

        /// <summary>
        /// Is the progress related task completed.
        /// </summary>
        private bool _isCompleted { get; set; } = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a progress Mvc form.
        /// </summary>
        /// <param name="total">Total steps to take.</param>
        /// <param name="title">Optional title.</param>
        /// <param name="taskName">Optional task name.</param>
        /// <param name="cancelMessage">Message to display if the user cancels.</param>
        /// <param name="desiredSeconds">Desired duration in seconds.</param>
        public ProgressCoordinator(int total, string title = null, string taskName = null, string cancelMessage = null, int desiredSeconds = 5)
        {
            // Set title and task name
            title ??= "Progress Bar";
            taskName ??= "Processing task";
            this._title = title;
            this._taskName = taskName;

            // Set cancel flags
            this._cancelMessage = cancelMessage;
            this.CancelledByUser = false;

            // Set the step delay based on desired run time
            this._totalSteps = total;
            this.SetStepDelay(desiredSeconds);

            // Start the viewmodel on a new thread
            // We need to do this as Revit uses its own thread for UI updates
            // The thread dispatcher keeps running the Mvvm on a separate thread to Revit
            this._thread = new Thread(() =>
            {
                // Store the dispatcher/thread
                this._dispatcher = Dispatcher.CurrentDispatcher;
                this._thread.Name = "ProgressControllerThread";

                // Establish the model/view and management
                this._model = new Mvvm.Models.ModelProgress();
                this._view = new Mvvm.Views.ViewProgress(this._model, total, title, taskName, cancelMessage);
                this._view.FunctionIfCompleted = () => _isCompleted;
                this._initialised.Set();

                // Only show the form if we have at least 2 steps to take
                // (Technically the Mvvm is still functioning BTS)
                if (total >= 2)
                {
                    this._view.Show();
                }

                // When the model is completed...
                this._model.ModelCompleted += (s, e) =>
                {
                    this._isCompleted = true;

                    // Ensuring we use the UI thread...
                    this._view.ThreadSafeAction(() =>
                    {
                        // If the model is cancelled and we have a cancel message...
                        if (this._model.IsCancelled && this._cancelMessage.Ext_HasChars())
                        {
                            // Show it via a message box (thread safe)
                            MessageBox.Show(this._cancelMessage,
                                            "Cancelled",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Exclamation);
                        }

                        // Close the view via the UI thread
                        this._view.CloseThreadSafe();
                    });

                    // Shutdown the dispatcher for thread safety
                    if (this._dispatcher != null &&
                    !this._dispatcher.HasShutdownStarted &&
                    !this._dispatcher.HasShutdownFinished &&
                    !this._shutdownRequested)
                    {
                        this._shutdownRequested = true;
                        this._dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                    }
                };

                // Start the dispatcher loop
                Dispatcher.Run();
            });

            // Start the dispatcher/thread
            this._thread.SetApartmentState(ApartmentState.STA);
            this._thread.IsBackground = true;
            this._thread.Start();
        }

        /// <summary>
        /// Wait until initiailization has finished.
        /// </summary>
        private void WaitUntilReady()
        {
            this._initialised.Wait();
        }

        #endregion

        #region Delay of progress

        /// <summary>
        /// Sets the required delay between steps.
        /// </summary>
        /// <param name="durationInSeconds">Desired duration to delay across.</param>
        /// <param name="minDelay">Minimum delay in ms.</param>
        /// <param name="maxDelay">Maximum delay in ms.</param>
        public void SetStepDelay(int durationInSeconds, int minDelay = 1, int maxDelay = 100)
        {
            // Delay variable
            int delay;

            // If we have a valid step count...
            if (this._totalSteps > 1 && durationInSeconds > 0)
            {
                // Calculate the precise increment
                delay = (int)Math.Round((durationInSeconds * 1000.0) / this._totalSteps);
            }
            else
            {
                // 1 or less steps = max delay
                delay = maxDelay;
            }

            // Clamp the value between min and max
            delay = Math.Max(minDelay, Math.Min(delay, maxDelay));

            // Set the delay value
            this._stepDelay = delay;
        }

        /// <summary>
        /// Adds delay between steps.
        /// </summary>
        public void DelayProgress()
        {
            // Delay by the given step
            Task.Delay(Math.Max(this._stepDelay, 1)).Wait();
        }

        #endregion

        #region Progress behaviors

        /// <summary>
        /// Increments the progress bar.
        /// </summary>
        /// <param name="stepSize">Count, begins at 1.</param>
        /// <param name="delay">A custom delay to apply.</param>
        public void Increment(int stepSize = 1, bool delay = true)
        {
            // Ensure we have valid state
            if (this._isCompleted) { return; }
            this.WaitUntilReady();
            if (this._isCompleted) { return; }

            // Optional delay
            if (delay)
            {
                this.DelayProgress();
            }

            // Ensure view and model are available
            if (this._model is null || this._view is null) { return; }

            // Ensuring we use the UI thread...
            this._view.ThreadSafeAction(() =>
            {
                // Determine the next progress value
                var newValue = this._model.ProgressValue + stepSize;

                // Ensure we do not over or under flow
                newValue = Math.Max(0, Math.Min(newValue, this._totalSteps));
                if (newValue < 0) { newValue = 0; }

                // Set the models progress value
                this._model.ProgressValue = newValue;
            });
        }

        /// <summary>
        /// Commits the progress bar and any related transactions.
        /// </summary>
        /// <param name="t">Related transaction.</param>
        /// <param name="tg">Related transaction group.</param>
        public void Commit(Transaction t = null, TransactionGroup tg = null)
        {
            // Ensure we have valid state
            if (this._isCompleted) { return; }
            this.WaitUntilReady();
            if (this._isCompleted) { return; }

            // If we closed already, we should not commit
            if (this._model.IsClosed || this._model.IsCancelled)
            {
                return;
            }

            // Commit any transactions/groups
            t.Ext_SafeCommit();
            tg.Ext_SafeAssimilate();

            // Mark model complete – raises Completed, which closes the UI
            this._model.Complete(cancelledByUser: false);
        }

        /// <summary>
        /// Runs a cancel check, updates if not.
        /// </summary>
        /// <param name="delay">Delay option.</param>
        /// <param name="stepsTaken">Increment (+1 by default).</param>
        /// <param name="t">The related transaction.</param>
        /// <param name="tg">The related transaction group.</param>
        /// <returns></returns>
        public bool CancelCheckOrUpdate(bool delay = true, int stepsTaken = 1, Transaction t = null, TransactionGroup tg = null)
        {
            // Ensure we have valid state
            if (this._isCompleted) { return true; }
            this.WaitUntilReady();
            if (this._isCompleted) { return true; }

            // Ensure view and model are available
            if (this._model is null || this._view is null) { return false; }

            // If the model has been cancelled by the user...
            if (this._model.IsCancelled)
            {
                // Rollback any transactions/groups
                t.Ext_SafeRollBack();
                tg.Ext_SafeRollBack();

                // Flag user cancellation
                this.CancelledByUser = true;

                // Ensuring we use the UI thread...
                this._view.ThreadSafeAction(() =>
                {
                    // Set the view's cancelled by user also
                    this._view.CancelledByUser = true;
                });

                // If the model is not yet closed...
                if (!this._model.IsClosed)
                {
                    // Close it, telling the model the user cancelled
                    this._model.CloseWindow(cancelledByUser: true);
                }

                // We return a cancel check of true
                return true;
            }

            // Proceed to increment and/or delay
            if (stepsTaken > 0) { this.Increment(stepsTaken, delay); }
            else { this.DelayProgress(); }

            // Return that we are not completed
            return false;
        }

        /// <summary>
        /// Cancels the progress bar.
        /// </summary>
        /// <param name="t">The related transaction.</param>
        /// <param name="tg">The related transaction group.</param>
        /// <param name="cancelledByUser">Has the user caused this cancellation.</param>
        public void Cancel(Transaction t = null, TransactionGroup tg = null, bool cancelledByUser = true)
        {
            // Rollback any transactions/groups
            t.Ext_SafeRollBack();
            tg.Ext_SafeRollBack();

            // Flag the coordinator as cancelled by user
            this.CancelledByUser = true;

            // If the model isn't closed, close it
            if (this._model is not null && !this._model.IsClosed)
            {
                this._model.CloseWindow(cancelledByUser: cancelledByUser);
            }
        }

        #endregion
    }
}