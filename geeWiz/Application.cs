#region Usings
// Revit API
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
// geeWiz
using geeWiz.Extensions;
using gAva = geeWiz.Availability.AvailabilityNames;
#endregion

// The class belongs to the geeWiz namespace
namespace geeWiz
{
    /// <summary>
    /// This interface handles startup and shutdown of the application.
    /// </summary>
    public class Application : IExternalApplication
    {
        #region Class properties

        // Temporary variable to pass the UI controlled app to an idling event
        private static UIControlledApplication _uiCtlApp;

        // Ribbon construction constants
        private const string TAB_NAME = "geeWiz";
        private const string PANEL1_NAME = "Testing";
        private string COMMANDCLASS_CURRENT = "";

        #endregion

        /// <summary>
        /// Runs when the application starts.
        /// We use this part of the interface to initialize geeWiz.
        /// </summary>
        public Result OnStartup(UIControlledApplication uiCtlApp)
        {
            #region Register UiApp

            // Set private variable
            _uiCtlApp = uiCtlApp;

            // Try to subscribe to the idling event, which sets uiApp global ASAP
            try
            {
                _uiCtlApp.Idling += OnIdling;
            }
            catch
            {
                Globals.UiApp = null;
            }

            #endregion

            #region Register Globals and Warden

            // Store all other global variables and tooltips
            Globals.RegisterVariables(uiCtlApp);
            Globals.RegisterTooltips("geeWiz.Resources.Files.Tooltips");

            // Register the warden commands
            Warden.Register(uiCtlApp);

            #endregion

            #region Construct toolbar

            /// <summary>
            /// We will load our commands here later on.
            /// </summary>

            // Create the tab
            uiCtlApp.Ext_AddRibbonTab(TAB_NAME);

            // Add Panel1 to the tab
            var ribbonPanel1 = uiCtlApp.Ext_AddRibbonPanelToTab(TAB_NAME, PANEL1_NAME);

            // Set the current command class prefix (handy for many tools)
            COMMANDCLASS_CURRENT = "geeWiz.Testing";

            // Add Github and Testing buttons
            ribbonPanel1.Ext_AddPushButton(buttonName: "Github", commandClass: $"{COMMANDCLASS_CURRENT}.Cmd_Github", availability: gAva.ZeroDoc);
            ribbonPanel1.Ext_AddPushButton(buttonName: "Testing", commandClass: $"{COMMANDCLASS_CURRENT}.Cmd_Testing", availability: gAva.Project);

            // Example of version management (not used yet)
            #if REVIT2024_OR_GREATER
            // TESTING
            #else
            //TESTING 2
            #endif

            #endregion

            // Return succeeded
            return Result.Succeeded;
        }

        /// <summary>
        /// Runs when the application closes down.
        /// We use this part of the interface to cleanup geeWiz.
        /// </summary>
        public Result OnShutdown(UIControlledApplication application)
        {
            // Still to add - Unsubscriptions
            
            // Return succeeded
            return Result.Succeeded;
        }

        #region Register UiApp on Idling

        /// <summary>
        /// Registers the uiApp global whenever first possible.
        /// </summary>
        /// <param name="sender"">The event sender object (the uiApp).</param>
        /// <param name="e"">The idling event arguments, unused.</param>
        /// <returns>Void (nothing).</returns>
        private void OnIdling(object sender, IdlingEventArgs e)
        {
            // Unsubscribe from the event (only runs once)
            _uiCtlApp.Idling -= OnIdling;

            // Register if possible (will generally be)
            if (sender is UIApplication uiApp)
            {
                Globals.UiApp = uiApp;
                Globals.UsernameRevit = uiApp.Application.Username;
            }
        }

        #endregion
    }
}