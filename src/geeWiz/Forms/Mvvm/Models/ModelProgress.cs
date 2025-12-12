// Using the Mvvm Models namespace
namespace geeWiz.Forms.Mvvm.Models
{
    /// <summary>
    /// The code to manage the Wpf model
    /// </summary>
    public sealed partial class ModelProgress : ObservableObject
    {
        #region Properties

        public bool IsClosed = false;
        public bool IsCancelled = false;

        private int _progressValue;
        private int _progressTotal;

        /// <summary>
        /// Property to track and trigger progress value changes.
        /// </summary>
        public int ProgressValue
        {
            get => this._progressValue;
            
            set
            {
                if (this._progressValue != value)
                {
                    this._progressValue = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Property to track and trigger progress total changes.
        /// </summary>
        public int ProgressTotal
        {
            get => this._progressTotal;

            set
            {
                if (this._progressTotal != value)
                {
                    this._progressTotal = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Fired when the model signals that progress is complete or cancelled
        /// </summary>
        public event EventHandler ModelCompleted;

        #endregion

        #region Close/cancel the viewmodel

        /// <summary>
        /// Closes the window.
        /// </summary>
        /// <param name="cancelledByUser">Was the form cancelled by the user.</param>
        public void CloseWindow(bool cancelledByUser = false)
        {
            // If already closed, stop here
            if (this.IsClosed)
            { 
                return;
            }

            // Set closed and cancelled properties
            this.IsClosed = true;
            this.IsCancelled = cancelledByUser;

            // Fire the completion handler
            this.ModelCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Shortcut to mark as complete (typically without cancellation).
        /// </summary>
        public void Complete(bool cancelledByUser = false)
        {
            this.CloseWindow(cancelledByUser: cancelledByUser);
        }

        #endregion
    }
}