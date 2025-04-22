// System
using Form = System.Windows.Forms.Form;
using FormsApplication = System.Windows.Forms.Application;

// The base form will belong to the forms namespace
namespace geeWiz.Forms.Bases
{
    #region Class summary
    /// <summary>
    /// Standard class for present a progress bar to the user.
    /// 
    /// Control with a using statement wrapped around your task.
    /// </summary>
    #endregion

    public partial class ProgressBar : Form
    {
        #region Class properties

        // Properties belonging to the form
        private bool isCancelled;
        private int pbCount;
        private int pbTotal;
        private bool showProgress;
        private bool cancellable;
        private string taskName;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a ProgressBar form.
        /// </summary>
        /// <param name="taskName">The message in the progress bar.</param>
        /// <param name="pbTotal">Total steps to take.</param>
        /// <param name="showProgress">Include the count/total in taskName.</param>
        /// <param name="cancellable">Allow cancellation to occur.</param>
        /// <returns>A ProgressBar form.</returns>
        public ProgressBar(string taskName = null, int pbTotal = 100, bool showProgress = true, bool cancellable = true)
        {
            // Initialize the form
            InitializeComponent();

            // Initialize variables
            this.pbCount = 1;
            this.pbTotal = pbTotal;
            this.progressBarObj.Minimum = 0;
            this.progressBarObj.Maximum = this.pbTotal;
            progressBarObj.Value = this.pbCount;

            // Set the task name
            taskName ??= "Processing task";
            this.taskName = taskName;
            this.progressLabel.Text = this.taskName;
            this.showProgress = showProgress;

            // Cancellation settings
            this.isCancelled = false;
            this.cancellable = cancellable;
            this.buttonCancel.Enabled = this.cancellable;

            // Show form
            Show();

            // Process messages in queue
            FormsApplication.DoEvents();
        }

        #endregion

        #region Increase progress

        /// <summary>
        /// Increase progress.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        public void Increment()
        {
            // Increase the progress value
            if (this.pbCount < this.pbTotal) { this.pbCount++; }
            this.progressBarObj.Value = this.pbCount;

            // Decrement to fix the slow progress graphic
            if (this.pbCount > 0)
            {
                this.progressBarObj.Value = this.pbCount - 1;
                this.progressBarObj.Value = this.pbCount;
            }

            // If not cancelled and we want to show progress
            if (this.showProgress && !this.isCancelled)
            {
                this.progressLabel.Text = $"{this.taskName} {this.pbCount} of {this.pbTotal}";
            }

            // Process messages in queue
            FormsApplication.DoEvents();
        }

        #endregion

        #region Cancellation check

        /// <summary>
        /// Check if cancelled, rollback related trasnsaction if provided.
        /// </summary>
        /// <param name="transaction"">A related transaction (optional).</param>
        /// <returns>True if cancelled, false if not.</returns>
        public bool CancelCheck(Transaction transaction = null)
        {
            // If we are cancelled and have a transaction
            if (this.isCancelled && transaction is not null)
            {
                // Roll it back (undo it)
                transaction.RollBack();
            }
            
            // Return if are cancelled for our task
            return this.isCancelled;
        }

        #endregion

        #region Commit the outcome

        /// <summary>
        /// Check if cancelled, commit a transaction if not.
        /// </summary>
        /// <param name="transaction"">A related transaction (optional).</param>
        /// <returns>Void (nothing).</returns>
        public void Commit(Transaction transaction)
        {
            // If we did not cancel
            if (!this.isCancelled)
            {
                // Commit the related transaction
                transaction.Commit();
            }
        }

        #endregion

        #region Cancel the progress bar

        /// <summary>
        /// Cancel the progress bar.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        private void Cancel()
        {
            // If we have not cancelled yet
            if (!this.isCancelled)
            {
                // Establish cancellation state
                this.isCancelled = true;
                this.progressLabel.Text = "Cancelling task...";
                this.buttonCancel.Enabled = false;
            }
        }

        #endregion

        #region Click cancel button

        /// <summary>
        /// Event handler when cancel button is clicked.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        #endregion

        #region Catch escape press

        /// <summary>
        /// Event handler to catch escape press.
        /// </summary>
        /// <param name="keyData"">Used keys.</param>
        /// <returns>A boolean.</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // If we meet the requirements on key entries
            if (Form.ModifierKeys == Keys.None
                && keyData == Keys.Escape
                && this.cancellable)
            {
                // Cancel and return true
                Cancel();
                return true;
            }

            // Otherwise process the key normally
            return base.ProcessDialogKey(keyData);
        }

        #endregion
    }
}