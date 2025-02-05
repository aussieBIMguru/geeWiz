// System
using System.Windows.Media;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using Brush = System.Windows.Media.Brush;
// Autodesk
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;
// UI Libraries
using UIFramework;
using Xceed.Wpf.AvalonDock.Controls;

// This class belongs to the root namespace
namespace geeWiz
{
    /// <summary>
    /// Coloured tabs will colour opened view tabs by Document title.
    /// </summary>
    public class ColouredTabs
    {
        #region Constants

        // A list of colours (will wrap around to start if needed)
        private static readonly List<Brush> COLOURS = new List<Brush>()
        {
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Indigo),
            new SolidColorBrush(Colors.Maroon),
            new SolidColorBrush(Colors.Orange),
            new SolidColorBrush(Colors.OrangeRed),
            new SolidColorBrush(Colors.Purple),
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.SeaGreen),
            new SolidColorBrush(Colors.Teal)
        };

        // Other private variables used in the class
        private static readonly string REGEX_TITLE = @"^(.*?)(\.\w{3,5} - )";
        private static readonly string ERROR_TITLE = "Error.xxx";
        private static readonly Brush COLOUR_WHITE = new SolidColorBrush(Colors.White);

        // List of unique document titles we will add to and index
        private static List<string> DOC_TITLES = new List<string>();

        #endregion

        #region Event triggered methods

        /// <summary>
        /// Activated when a document is opened (if tabs are being coloured).
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        public static void DocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            ColorTabs();
        }

        /// <summary>
        /// Activated when a view is activated (if tabs are being coloured).
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        public static void ViewActivated(object sender, ViewActivatedEventArgs args)
        {
            ColorTabs();
        }

        #endregion

        #region Color tabs method

        /// <summary>
        /// Runs the tab recolouring routine.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        public static void ColorTabs()
        {
            // Get main window children document panes (view tabs)
            var documentPanes = MainWindow
                .getMainWnd()
                .FindVisualChildren<LayoutDocumentPaneControl>();

            // For each document pane
            foreach (var pane in documentPanes)
            {
                // Get the tabs of the document pane
                var tabItems = pane.FindVisualChildren<TabItem>();

                // For each tab
                foreach (var tabItem in tabItems)
                {
                    // Get its title, extract the prefix
                    string tooltip = tabItem.ToolTip.ToString();
                    string docTitle = TitleFromTooltip(tooltip);

                    // Index for colour to assign
                    int ind;

                    // If the document is already in our document list
                    if (DOC_TITLES.Contains(docTitle))
                    {
                        // Get the index of the title
                        ind = DOC_TITLES.IndexOf(docTitle);
                    }
                    else
                    {
                        // We know it will be the next index
                        ind = DOC_TITLES.Count;

                        // Add the docuement title
                        DOC_TITLES.Add(docTitle);
                    }

                    // Assign the colour at index (wrapped)
                    tabItem.Background = COLOURS[ind % COLOURS.Count];

                    // Set the text to white (dark tab colours)
                    tabItem.Foreground = COLOUR_WHITE;
                }
            }

        }

        #endregion

        #region Registration to events

        /// <summary>
        /// Register the events to the document opened/activated events.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        public static void Register()
        {
            Globals.CtlApp.DocumentOpened += new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(DocumentOpened);
            Globals.UiApp.ViewActivated += new EventHandler<Autodesk.Revit.UI.Events.ViewActivatedEventArgs>(ViewActivated);
        }

        /// <summary>
        /// De-register the events from the document opened/activated events.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        public static void DeRegister()
        {
            Globals.CtlApp.DocumentOpened -= new EventHandler<Autodesk.Revit.DB.Events.DocumentOpenedEventArgs>(DocumentOpened);
            Globals.UiApp.ViewActivated -= new EventHandler<Autodesk.Revit.UI.Events.ViewActivatedEventArgs>(ViewActivated);
        }

        #endregion

        #region Tooltip to document title

        /// <summary>
        /// Given a valid tooltip value, returns the document title.
        /// </summary>
        /// <param name="toolTip">A TabItem tooltip.</param>
        /// <returns>A string.</returns>
        private static string TitleFromTooltip(string toolTip)
        {
            // Return error value if invalid
            if (string.IsNullOrEmpty(toolTip))
            {
                return ERROR_TITLE;
            }

            // Use regex to find a match
            Match match = Regex.Match(toolTip, REGEX_TITLE);

            // If we found the match
            if (match.Success)
            {
                // Return the title value
                return match.Groups[1].Value;
            }
            else
            {
                // Otherwise return error value
                return ERROR_TITLE;
            }
        }

        #endregion
    }
}
