// Revit API
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
// geeWiz
using gFil = geeWiz.Utilities.File_Utils;

// The class belongs to the Commands namespace
namespace geeWiz.Commands.Cmds_General
{
    /// <summary>
    /// Opens the project Github page.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_About : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Open the Url
            string linkPath = @"https://github.com/aussieBIMguru/geeWiz";
            return gFil.OpenLinkPath(linkPath);
        }
    }
}