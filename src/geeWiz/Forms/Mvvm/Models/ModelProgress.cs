// Mvvm toolkit
using CommunityToolkit.Mvvm.ComponentModel;

// Using the Mvvm Models namespace
namespace geeWiz.Forms.Mvvm.Models
{
    /// <summary>
    /// The Model of the MVC Progress Bar system.
    /// </summary>
    public sealed partial class ModelProgress : ObservableObject
    {
        #region Properties

        /// <summary>
        /// Has the progress bar been closed.
        /// </summary>
        public bool IsClosed = false;

        /// <summary>
        /// Has the progress bar been cancelled.
        /// </summary>
        public bool IsCancelled = false;

        /// <summary>
        /// The current progress bar value.
        /// </summary>
        private int _progressValue;

        /// <summary>
        /// The total steps to take.
        /// </summary>
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
        /// <param name="cancelledByUser">Was the form cancelled by the user.</param>
        public void Complete(bool cancelledByUser = false)
        {
            this.CloseWindow(cancelledByUser: cancelledByUser);
        }

        #endregion
    }
}