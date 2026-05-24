// System
using Autodesk.Revit.ApplicationServices;
// Revit API
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DocumentFormat.OpenXml.Bibliography;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Resources;
using Assembly = System.Reflection.Assembly;

// The class belongs to the geeWiz namespace
namespace geeWiz
{
    /// <summary>
    /// Variables that persist beyond the running of commands.
    /// They can be accessed via this static class where they would otherwise be inaccessible.
    /// </summary>
    public static class Globals
    {
        #region Global properties

        /// <summary>
        /// The UIControlledApplication for the session.
        /// </summary>
        public static UIControlledApplication UiCtlApp { get; set; }

        /// <summary>
        /// The ControlledApplication for the session.
        /// </summary>
        public static ControlledApplication CtlApp { get; set; }

        /// <summary>
        /// The UIApplication for the session.
        /// </summary>
        public static UIApplication UiApp { get; set; }

        /// <summary>
        /// The add-in Assembly.
        /// </summary>
        public static Assembly Assembly { get; set; }

        /// <summary>
        /// The path to the add-in Assembly.
        /// </summary>
        public static string AssemblyPath { get; set; }

        /// <summary>
        /// The path to the root folder of the add-in.
        /// </summary>
        public static string RootAddinPath { get; set; }

        /// <summary>
        /// The full Revit version as a string.
        /// </summary>
        public static string RevitVersion { get; set; }

        /// <summary>
        /// The major Revit version as an integer.
        /// </summary>
        public static int RevitVersionInt { get; set; }

        /// <summary>
        /// The Revit username of the current user.
        /// </summary>
        public static string UsernameRevit { get; set; }

        /// <summary>
        /// The Windows username of the current user.
        /// </summary>
        public static string UsernameWindows { get; set; }

        /// <summary>
        /// The add-in version number.
        /// </summary>
        public static string VersionNumber { get; set; }

        /// <summary>
        /// The add-in GUID.
        /// </summary>
        public static string AddinGuid { get; set; }

        /// <summary>
        /// The add-in name.
        /// </summary>
        public static string AddinName { get; set; }

        /// <summary>
        /// A dictionary containing all tooltips by command key.
        /// </summary>
        public static Dictionary<string, string> Tooltips { get; set; } = new Dictionary<string, string>();

        #endregion

        #region Register variables

        /// <summary>
        /// Sets the Global variable values.
        /// </summary>
        /// <param name="uiCtlApp">The UIControlledApplication.</param>
        public static void RegisterVariables(UIControlledApplication uiCtlApp)
        {
            // Store all available global variable values (available anywhere, effectively)
            Globals.UiCtlApp = uiCtlApp;
            Globals.CtlApp = uiCtlApp.ControlledApplication;
            // (uiApp set by idling event)

            // Store all paths
            Globals.Assembly = Assembly.GetExecutingAssembly();
            Globals.AssemblyPath = Assembly.GetExecutingAssembly().Location;
            Globals.RootAddinPath = Globals.AssemblyPath.Replace("\\geeWiz.dll", "");

            // Store Revit version
            Globals.RevitVersion = uiCtlApp.ControlledApplication.VersionNumber;
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
        /// Gets the current document, if any.
        /// </summary>
        /// <param name="doc">If not null, return this instead.</param>
        /// <returns>The current Document.</returns>
        public static Document CurrentDocument(Document doc = null)
        {
            doc ??= Globals.UiApp?.ActiveUIDocument?.Document;
            return doc;
        }

        #endregion

        #region Register tooltips

        /// <summary>
        /// Sets up the Global tooltips dictionary.
        /// </summary>
        /// <param name="resourcePath">The full path to the tooltip resource.</param>
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