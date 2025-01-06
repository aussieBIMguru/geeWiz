// Revit API
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

// The class belongs to the geeWiz namespace
namespace geeWiz
{
    /// <summary>
    /// Variables that persist beyond the running of commands.
    /// Many of them are set once at app startup.
    /// </summary>
    public static class Globals
    {
        // Applications
        public static UIControlledApplication uiCtlApp { get; set; }
        public static ControlledApplication ctlApp { get; set; }
        public static UIApplication uiApp { get; set; }
        public static System.Reflection.Assembly Assembly { get; set; }
        public static string AssemblyPath { get; set; }
        public static bool isIdling { get; set; }

        // Revit versions
        public static string RevitVersion { get; set; }
        public static int RevitVersionInt { get; set; }

        // User names
        public static string UsernameRevit { get; set; }
        public static string UsernameWindows { get; set; }

        // Guids
        public static string AddinGuid { get; set; }

        // Tooltips resource
        public static Dictionary<string, string> Tooltips { get; set; } = new Dictionary<string, string>();
    }
}
