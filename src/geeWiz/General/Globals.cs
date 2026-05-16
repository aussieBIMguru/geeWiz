// System
using Autodesk.Revit.ApplicationServices;
// Revit API
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DocumentFormat.OpenXml.Bibliography;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using Assembly = System.Reflection.Assembly;

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
            Globals.UiCtlApp = uiApp;
            Globals.CtlApp = uiApp.ControlledApplication;
            // (uiApp set by idling event)

            // Store all paths
            Globals.Assembly = Assembly.GetExecutingAssembly();
            Globals.AssemblyPath = Assembly.GetExecutingAssembly().Location;
            Globals.SubAssemblyPath = Globals.AssemblyPath.Replace("\\geeWiz.dll", "");
            Globals.ResourcesPath = Path.Combine(Path.GetDirectoryName(Globals.AssemblyPath), "Resources");

            // Store Revit version
            Globals.RevitVersion = uiApp.ControlledApplication.VersionNumber;
            Globals.RevitVersionInt = Int32.Parse(Globals.RevitVersion);

            // Store user names
            Globals.UsernameWindows = Environment.UserName;
            // (UsernameRevit stored by idling event)

            // Store versions and Ids
            Globals.VersionNumber = Assembly.GetName().Version?.ToString();
            Globals.AddinGuid = "8FFC127F-9CD7-46E2-8506-C5F36D057B4B";
            Globals.AddinName = nameof(geeWiz);
        }

        #endregion

        #region Misc

        /// <summary>
        /// Gets the active document, if any.
        /// </summary>
        /// <param name="doc">If not null, return this instead.</param>
        /// <returns>A Document.</returns>
        public static Document CurrentDocument(Document doc = null)
        {
            doc ??= Globals.UiApp?.ActiveUIDocument?.Document;
            return doc;
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
            ResourceSet resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);

            // Get all tooltip entries, store globally
            foreach (DictionaryEntry entry in resourceSet)
            {
                string key = entry.Key.ToString();
                Globals.Tooltips[key] = entry.Value.ToString();
            }
        }

        #endregion
    }
}