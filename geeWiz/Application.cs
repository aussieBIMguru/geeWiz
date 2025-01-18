// Revit API
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
// geeWiz
using geeWiz.Extensions;
using gAva = geeWiz.Availability.AvailabilityNames;
using gRib = geeWiz.Utilities.RibbonUtils;

// The class belongs to the geeWiz namespace
namespace geeWiz
{
    /// <summary>
    /// This interface handles startup and shutdown of the application.
    /// </summary>
    public class Application : IExternalApplication
    {
        // Temporary variable to pass the UI controlled app to an idling event
        private static UIControlledApplication _uiCtlApp;

        /// <summary>
        /// Runs when the application starts.
        /// We use this part of the interface to initialize geeWiz.
        /// </summary>
        public Result OnStartup(UIControlledApplication uiCtlApp)
        {
            // Collect the uiApp using idling event
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

            // Store all other global variables and tooltips
            Globals.RegisterVariables(uiCtlApp);
            Globals.RegisterTooltips("geeWiz.Files.Tooltips");


            /// <summary>
            /// We will load our commands here later on.
            /// </summary>

            // Return succeeded
            return Result.Succeeded;
        }

        /// <summary>
        /// Runs when the application closes down.
        /// We use this part of the interface to cleanup geeWiz.
        /// </summary>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

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
    }
}