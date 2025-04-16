// System
using System.IO;
using Assembly = System.Reflection.Assembly;
// Revit API
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using System.Collections;
using System.Globalization;
using System.Resources;

// The class belongs to the geeWiz namespace
namespace geeWiz
{
    /// <summary>
    /// Variables that persist beyond the running of commands.
    /// Many of them are set once at app startup.
    /// </summary>
    public static class Globals
    {
        #region Global properties

        // Applications
        public static UIControlledApplication UiCtlApp { get; set; }
        public static ControlledApplication CtlApp { get; set; }
        public static UIApplication UiApp { get; set; }
        public static bool Idling { get; set; }
        public static bool IsDarkMode { get; set; }

        // General utilities
        public static bool WardenActive { get; set; }
        public static string LastCommandId { get; set; }
        public static bool ColouringTabs { get; set; }

        // Key paths
        public static Assembly Assembly { get; set; }
        public static string AssemblyPath { get; set; }
        public static string SubAssemblyPath { get; set; }
        public static string ResourcesPath { get; set; }

        // Revit versions
        public static string RevitVersion { get; set; }
        public static int RevitVersionInt { get; set; }

        // User names
        public static string UsernameRevit { get; set; }
        public static string UsernameWindows { get; set; }

        // Guids and versioning
        public static string VersionNumber { get; set; }
        public static string VersionName { get; set; }
        public static string AddinGuid { get; set; }
        public static string AddinName { get; set; }

        // Tooltips resource
        public static Dictionary<string, string> Tooltips { get; set; } = new Dictionary<string, string>();

        #endregion

        #region Register variables

        /// <summary>
        /// Sets the global values.
        /// </summary>
        /// <param name="uiApp"">The UIApplication.</param>
        /// <returns>Void (nothing).</returns>
        public static void RegisterVariables(UIControlledApplication uiApp)
        {
            // Store all available global variable values (available anywhere, effectively)
            UiCtlApp = uiApp;
            CtlApp = uiApp.ControlledApplication;
            // (uiApp set by idling event)
            Idling = true;
            IsDarkMode = false;

            // General utilities
            WardenActive = true;
            LastCommandId = null;
            ColouringTabs = false;

            // Store all paths
            Assembly = Assembly.GetExecutingAssembly();
            AssemblyPath = Assembly.GetExecutingAssembly().Location;
            SubAssemblyPath = Globals.AssemblyPath.Replace("\\geeWiz.dll", "");
            ResourcesPath = Path.Combine(Path.GetDirectoryName(Globals.AssemblyPath), "Resources");

            // Store Revit version
            RevitVersion = uiApp.ControlledApplication.VersionNumber;
            RevitVersionInt = Int32.Parse(Globals.RevitVersion);

            // Store user names
            UsernameWindows = Environment.UserName;
            // (UsernameRevit stored by idling event)

            // Store versions and Ids
            VersionNumber = "25.XX.XX.00";
            VersionName = "WIP";
            AddinGuid = "8FFC127F-9CD7-46E2-8506-C5F36D057B4B";
            AddinName = "geeWiz";
        }

        #endregion

        #region Register tooltips

        /// <summary>
        /// Sets the tooltip values.
        /// </summary>
        /// <param name="resourcePath"">The full path to the tooltip resource.</param>
        /// <returns>Void (nothing).</returns>
        public static void RegisterTooltips(string resourcePath)
        {
            // Construct the assembly, resource and sub-assembly paths
            var resourceManager = new ResourceManager(resourcePath, Globals.Assembly);
            var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            // Get all tooltip entries, store globally
            foreach (DictionaryEntry entry in resourceSet)
            {
                string key = entry.Key.ToString();
                Tooltips[key] = entry.Value.ToString();
            }
        }

        #endregion
    }
}