// Autodesk specific (Adwindows)
using ResultClickEventArgs = Autodesk.Internal.InfoCenter.ResultClickEventArgs;
// geeWiz specific
using gFil = geeWiz.Utilities.File_Utils;

// The base form will belong to the forms namespace (we decorate in the custom class)
namespace geeWiz.Forms
{
    #region Class summary

    /// <summary>
    /// Bubble messages appear at the top right of the screen.
    /// 
    /// They are supported by the AdWindows library, but not officially but Autodesk.
    /// 
    /// Use method 'Show()' to display after creation.
    /// If a file or link path is provided on creation, clicking the form will attempt to open it.
    /// 
    /// </summary>

    #endregion

    internal class BubbleMessage
    {
        #region Class properties

        // Title and message properties
        private string title;
        private string message;
        // File and link paths to open on click
        private string filePath;
        private string linkPath;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a bubble message object (but does not show it).
        /// </summary>
        /// <param name="title"">The title for the form.</param>
        /// <param name="message"">The message for the form.</param>
        /// <param name="filePath"">An optional file path to open on click.</param>
        /// <param name="linkPath"">An optional link path to open on click (file path takes priority).</param>
        /// <returns>A BubbleMessage form.</returns>
        public BubbleMessage(string title, string message, string linkPath = null, string filePath = null)
        {
            // Construct the object, pass its properties
            this.title = title;
            this.message = message;
            this.filePath = filePath;
            this.linkPath = linkPath;
        }

        #endregion

        #region Show method

        /// <summary>
        /// Shows the bubble message form after construction.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        public void Show()
        {
            // Create the result item, set its properties
            var resultItem = new Autodesk.Internal.InfoCenter.ResultItem();
            resultItem.Category = this.title;
            resultItem.Title = this.message;
            resultItem.IsFavorite = false;
            resultItem.IsNew = true;

            // If the link path is valid, convert to a unique resource identifier (Uri)
            if (this.linkPath != null && gFil.LinkIsAccessible(this.linkPath))
            {
                resultItem.Uri = new System.Uri(this.linkPath);
            }

            // If we have a file path, apply the result clicked event to the bubble message
            if (this.filePath != null || this.linkPath != null)
            {
                resultItem.ResultClicked += new EventHandler<ResultClickEventArgs>(resultItem_ResultClicked);
            }

            // Show the result item
            Autodesk.Windows.ComponentManager.InfoCenterPaletteManager.ShowBalloon(resultItem);
        }

        #endregion

        #region On click event

        /// <summary>
        /// Opens the filepath or linkpath attached to the bubble message.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void resultItem_ResultClicked(object sender, ResultClickEventArgs e)
        {
            // Opens either the file or link path
            if (this.filePath != null)
            {
                gFil.OpenFilePath(this.filePath);
            }
            else
            {
                gFil.OpenLinkPath(this.linkPath);
            }
        }

        #endregion
    }
}