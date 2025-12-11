// System
using System.Windows.Threading;
using Dispatcher = System.Windows.Threading.Dispatcher;

// Belongs to the forms namespace
namespace geeWiz.Forms
{
    /// <summary>
    /// Mvvm Progress bar controller.
    /// </summary>
    public class ProgressController
    {
        #region Properties

        // Mvvm and thread
        private Mvvm.Views.ViewProgress _view { get; set; }
        private Mvvm.Models.ModelProgress _model { get; set; }
        private Thread _thread { get; set; }

        // Title and taskname
        private string _title { get; set; }
        private string _taskName { get; set; }

        // Internal properties
        private int _totalSteps { get; set; }
        private string _cancelMessage { get; set; }
        private int _stepDelay { get; set; }

        // Check for cancellation
        public bool CancelledByUser { get; set; }

        #endregion

        #region Constructor


        public ProgressController(int total, string title = null, string taskName = null, string cancelMessage = null, int desiredSeconds = 5)
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

            // Establish the model
            this._model = new Mvvm.Models.ModelProgress();

            // Start the viewmodel on a new thread
            // We need to do this as Revit uses its own thread for UI updates
            // The thread dispatcher keeps running the Mvvm on a separate thread to Revit
            this._thread = new Thread(() =>
            {
                // Establish the Mvvm pairing
                this._thread.Name = "ProgressWindowThread";
                this._view = new Mvvm.Views.ViewProgress(this._model, total, title, taskName, cancelMessage);

                // Only show the form if we have at least 2 steps to take
                // (Technically the Mvvm is still functioning BTS)
                if (total >= 2)
                {
                    this._view.Show();
                }

                // When the model is completed, the view closes with dispatcher
                this._model.Completed += (s, e) =>
                {
                    this._view.CloseSafelyWithDispatcher();
                };

                // Start the dispatcher loop
                Dispatcher.Run();
            });

            this._thread.SetApartmentState(ApartmentState.STA);
            this._thread.Start();
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
            // Figure out required delay
            int delay;

            if (this._totalSteps > 1)
            {
                delay = (durationInSeconds * 1000) / this._totalSteps;
            }
            else
            {
                delay = durationInSeconds * 1000;
            }

            // Ensure it falls within acceptable limits
            if (delay < minDelay)
            {
                this._stepDelay = minDelay;
            }
            else if (delay > maxDelay)
            {
                this._stepDelay = maxDelay;
            }
            else
            {
                this._stepDelay = delay;
            }
        }

        /// <summary>
        /// Adds delay between steps.
        /// </summary>
        public void DelayProgress()
        {
            Task.Delay(Math.Max(_stepDelay, 1)).Wait();
        }

        #endregion

        #region Progress behaviors

        /// <summary>
        /// Increments the progress bar.
        /// </summary>
        /// <param name="count">Count, begins at 1.</param>
        /// <param name="delay">A custom delay to apply.</param>
        public void Increment(int count = 1, bool delay = true)
        {
            // Optional delay
            if (delay)
            {
                this.DelayProgress();
            }

            // Run the task on the view's thread
            this._view.Dispatcher.BeginInvoke(new Action(() =>
            {
                // Skip if we exceed the total limit
                if (count > this._totalSteps)
                {
                    return;
                }

                // Set the progress value to suit (catching first step)
                this._model.ProgressValue = count < 2 ? this._model.ProgressValue + 1 : count;

            }), DispatcherPriority.Background);
        }

        /// <summary>
        /// Commits the progress bar and any related transactions.
        /// </summary>
        /// <param name="t">Related transaction.</param>
        /// <param name="tg">Related transaction group.</param>
        public void Commit(Transaction t = null, TransactionGroup tg = null)
        {
            // If we closed or cancelled, do not commit/assimilate
            if (this._model.IsClosed || this._model.IsCancelled)
            {
                return;
            }

            // Commit any transactions/groups
            t?.Commit();
            tg?.Assimilate();

            // Close the view safely
            this.CloseDispatcher();
        }

        /// <summary>
        /// Runs a cancel check, updates if not.
        /// </summary>
        /// <param name="delay">Delay option.</param>
        /// <param name="increment">Increment (+1 by default).</param>
        /// <returns></returns>
        public bool CancelCheck(bool delay = true, int increment = 1, Transaction t = null, TransactionGroup tg = null)
        {
            // Rollback any transactions/groups if cancelled
            if (this._model.IsCancelled)
            {
                this.CancelledByUser = true;
                t?.RollBack();
                tg?.RollBack();
                this.CloseDispatcher();
                return true;
            }

            // Update process if not cancelled
            if (delay) { this.DelayProgress(); }
            if (increment > 0) { this.Increment(increment); }
            return false;
        }

        /// <summary>
        /// Cancels the progress bar manually.
        /// </summary>
        public void Cancel(Transaction t = null, TransactionGroup tg = null)
        {
            // Rollback any active transactions/groups
            t?.RollBack();
            tg?.RollBack();

            // Set user-cancelled flag
            this.CancelledByUser = true;

            // Always attempt safe dispatcher closure, using internal checks
            this.CloseDispatcher();
        }

        /// <summary>
        /// Safely closes the process and dispatcher.
        /// </summary>
        private void CloseDispatcher()
        {
            // Only proceed if view exists and dispatcher is not shutting down
            if (this._view?.Dispatcher != null &&
                !this._view.Dispatcher.HasShutdownStarted &&
                !this._view.Dispatcher.HasShutdownFinished)
            {
                this._view.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this._view.CancelledByUser = true;
                    this._view.CloseSafelyWithDispatcher();
                }), DispatcherPriority.Background);
            }
        }

        #endregion
    }
}