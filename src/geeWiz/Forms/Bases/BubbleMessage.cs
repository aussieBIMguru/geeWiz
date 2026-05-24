// Revit API
using ResultClickEventArgs = Autodesk.Internal.InfoCenter.ResultClickEventArgs;
// geeWiz specific
using gFil = geeWiz.Utilities.File_Utils;

// The base form will belong to the forms namespace (we decorate in the custom class)
namespace geeWiz.Forms.Bases
{
    /// <summary>
    /// Bubble messages appear at the top right of the screen.
    /// 
    /// They are supported by the AdWindows library, but not officially but Autodesk.
    /// 
    /// Use method 'Show()' to display after creation.
    /// If a file or link path is provided on creation, clicking the form will attempt to open it.
    /// </summary>
    public class BubbleMessage
    {
        #region Class properties

        /// <summary>
        /// Title of window.
        /// </summary>
        private string _title;

        /// <summary>
        /// Message of window.
        /// </summary>
        private string _message;
        
        /// <summary>
        /// Related file path.
        /// </summary>
        private string _filePath;

        /// <summary>
        /// Related URL path.
        /// </summary>
        private string _urlPath;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a bubble message object (but does not show it).
        /// </summary>
        /// <param name="title">The title for the form.</param>
        /// <param name="message">The message for the form.</param>
        /// <param name="filePath">An optional file path to open on click.</param>
        /// <param name="linkPath">An optional link path to open on click (file path takes priority).</param>
        /// <returns>A BubbleMessage form.</returns>
        public BubbleMessage(string title, string message, string linkPath = null, string filePath = null)
        {
            // Construct the object, pass its properties
            this._title = title;
            this._message = message;
            this._filePath = filePath;
            this._urlPath = linkPath;
        }

        #endregion

        #region Show method

        /// <summary>
        /// Shows the bubble message form after construction.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability",
            "CA1416:Validate platform compatibility",
            Justification ="Windows support only.")]
        public void Show()
        {
            // Create the result item, set its properties
            var resultItem = new Autodesk.Internal.InfoCenter.ResultItem()
            {
                Category = this._title,
                Title = this._message,
                IsFavorite = false,
                IsNew = true
            };

            // If the link path is valid, convert to a unique resource identifier (Uri)
            if (this._urlPath != null && gFil.LinkIsAccessible(this._urlPath))
            {
                resultItem.Uri = new System.Uri(this._urlPath);
            }

            // If we have a file path, apply the result clicked event to the bubble message
            if (this._filePath != null || this._urlPath != null)
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
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void resultItem_ResultClicked(object sender, ResultClickEventArgs e)
        {
            // Opens either the file or link path
            if (this._filePath != null)
            {
                gFil.OpenFilePath(this._filePath);
            }
            else
            {
                gFil.OpenLinkPath(this._urlPath);
            }
        }

        #endregion
    }
}