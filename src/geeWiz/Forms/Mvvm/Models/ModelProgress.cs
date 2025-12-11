// System
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxImage = System.Windows.MessageBoxImage;
// geeWiz
using geeWiz.Extensions;

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
            get => _progressTotal;

            set
            {
                if (_progressTotal != value)
                {
                    _progressTotal = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Fired when the model signals that progress is complete or cancelled
        /// </summary>
        public event EventHandler Completed;

        #endregion

        #region Close/cancel the viewmodel

        /// <summary>
        /// Closes the window (sent via the view closure).
        /// </summary>
        /// <param name="cancelMessage">Message to display to user.</param>
        /// <param name="cancelledByUser">Was the form cancelled by the user.</param>
        public void CloseWindow(string cancelMessage = null, bool cancelledByUser = false)
        {
            // Set the models respective properties
            this.IsClosed = true;
            this.IsCancelled = cancelledByUser;

            // If cancel message is valid and user cancelled the form...
            if (cancelMessage.Ext_HasChars() && cancelledByUser)
            {
                // Display a cancellation dialog
                MessageBox.Show(cancelMessage,"Cancelled",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            // Fire the Completed event so any subscriber (e.g., view or controller) can shutdown the dispatcher
            this.Completed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Shortcut to mark as complete without user cancellation
        /// </summary>
        public void Complete()
        {
            CloseWindow(cancelledByUser: false);
        }

        #endregion
    }
}