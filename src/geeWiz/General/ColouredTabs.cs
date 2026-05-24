// System
using System.Windows.Media;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using Brush = System.Windows.Media.Brush;
// Revit API
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI.Events;
// UI Libraries
using UIFramework;
using Xceed.Wpf.AvalonDock.Controls;
using Autodesk.Revit.ApplicationServices;

// This class belongs to the root namespace
namespace geeWiz.General
{
    /// <summary>
    /// This class manages the colouring of View tabs by Document.
    /// </summary>
    public static class ColouredTabs
    {
        #region Constants

        /// <summary>
        /// A default list of Colours to use for tab colouring.
        /// </summary>
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

        /// <summary>
        /// The Regex to detect a valid Document title.
        /// </summary>
        private static readonly string REGEX_TITLE = @"^(.*?)(\.\w{3,5} - )";

        /// <summary>
        /// Invalid Documents will use this title.
        /// </summary>
        private static readonly string ERROR_TITLE = "Error.xxx";

        /// <summary>
        /// The Colour to use for coloured tab text.
        /// </summary>
        private static readonly Brush COLOUR_WHITE = new SolidColorBrush(Colors.White);

        /// <summary>
        /// A list of Document titles involved in the tab colouring process.
        /// </summary>
        private static List<string> DOC_TITLES = new List<string>();

        /// <summary>
        /// If the system is activated.
        /// </summary>
        public static bool ACTIVE = false;

        #endregion

        #region Registration to events

        /// <summary>
        /// Registers the system to related Revit events.
        /// </summary>
        /// <param name="ctlApp">The ControlledApplication.</param>
        /// <param name="uiApp">The UIApplication.</param>
        public static void Register(ControlledApplication ctlApp = null, UIApplication uiApp = null)
        {
            ctlApp ??= Globals.CtlApp;
            uiApp ??= Globals.UiApp;

            ctlApp.DocumentOpened += new EventHandler<DocumentOpenedEventArgs>(DocumentOpened);
            uiApp.ViewActivated += new EventHandler<ViewActivatedEventArgs>(ViewActivated);

            ACTIVE = true;
        }

        /// <summary>
        /// Deregisters the system from related Revit events.
        /// </summary>
        /// <param name="ctlApp">The ControlledApplication.</param>
        /// <param name="uiApp">The UIApplication.</param>
        public static void DeRegister(ControlledApplication ctlApp = null, UIApplication uiApp = null)
        {
            ctlApp ??= Globals.CtlApp;
            uiApp ??= Globals.UiApp;

            ctlApp.DocumentOpened -= new EventHandler<DocumentOpenedEventArgs>(DocumentOpened);
            uiApp.ViewActivated -= new EventHandler<ViewActivatedEventArgs>(ViewActivated);

            ACTIVE = false;
        }

        #endregion

        #region Event triggered methods

        /// <summary>
        /// Runs the routine when a Document is opened.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Related event arguments.</param>
        private static void DocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            TabColouringRoutine();
        }

        /// <summary>
        /// Runs the routine when a View is activated.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Related event arguments.</param>
        private static void ViewActivated(object sender, ViewActivatedEventArgs args)
        {
            TabColouringRoutine();
        }

        #endregion

        #region Color tabs method

        /// <summary>
        /// The core Tab colouring routine.
        /// </summary>
        public static void TabColouringRoutine()
        {
            // Get main window children document panes (view tabs)
            IEnumerable<LayoutDocumentPaneControl> documentPanes = MainWindow.getMainWnd()
                .FindVisualChildren<LayoutDocumentPaneControl>();

            // For each document pane
            foreach (var pane in documentPanes)
            {
                // Get the tabs of the document pane
                IEnumerable<TabItem> tabItems = pane.FindVisualChildren<TabItem>();

                // For each tab
                foreach (TabItem tabItem in tabItems)
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

        #region Tooltip to document title

        /// <summary>
        /// Given a valid tooltip value, returns the document title.
        /// </summary>
        /// <param name="toolTip">A TabItem tooltip.</param>
        /// <returns>A String.</returns>
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
