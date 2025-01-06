// System
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.IO;
// Revit API
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

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
        public Result OnStartup(UIControlledApplication application)
        {
            // Collect the uiApp using idling event
            _uiCtlApp = application;

            // Try to subscribe to the idling event, which will capture the uiApp global
            // Idling event = whenever Revit becomes available for commands
            try
            {
                _uiCtlApp.Idling += OnIdling;
            }
            catch
            {
                Globals.uiApp = null;
            }

            // Store all available global variable values (available anywhere, effectively)
            Globals.uiCtlApp = application;
            Globals.ctlApp = application.ControlledApplication;
            // (uiApp set by idling event)
            Globals.Assembly = Assembly.GetExecutingAssembly();
            Globals.AssemblyPath = Assembly.GetExecutingAssembly().Location;
            Globals.isIdling = false;
            Globals.RevitVersion = application.ControlledApplication.VersionNumber;
            Globals.RevitVersionInt = Int32.Parse(Globals.RevitVersion);
            Globals.UsernameWindows = Environment.UserName;
            Globals.AddinGuid = "{8FFC127F-9CD7-46E2-8506-C5F36D057B4B}";

            // Construct the assembly, resource and sub-assembly paths
            string thisAssemblyPath = Globals.AssemblyPath;
            string thisResourcePath = Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources");
            string subAssemblyPath = thisAssemblyPath.Replace("\\geeWiz.dll", "");

            // Access the resource manager, for collection of tooltips
            var resourceManager = new ResourceManager("geeWiz.Files.Tooltips", typeof(Application).Assembly);
            var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            // Get all tooltip entries, store globally
            foreach (DictionaryEntry entry in resourceSet)
            {
                string key = entry.Key.ToString();
                string value = entry.Value.ToString().Replace("\\n", Environment.NewLine);
                Globals.Tooltips[key] = value;
            }

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

            // Get the uiApp as the event sender
            UIApplication uiApp = sender as UIApplication;

            // If uiApp was collected, set the related globals
            if (uiApp != null)
            {
                Globals.uiApp = uiApp;
                Globals.UsernameRevit = uiApp.Application.Username;
            }
        }
    }
}