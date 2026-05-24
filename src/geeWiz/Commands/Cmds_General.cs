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
    /// A Revit command ran from the ribbon.
    /// Displays information about the addin.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_About : IExternalCommand
    {
        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="commandData">Command related data.</param>
        /// <param name="message">Command related message.</param>
        /// <param name="elements">Command related elements.</param>
        /// <returns>A Result.</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Open the Url
            string linkPath = @"https://github.com/aussieBIMguru/geeWiz";
            return gFil.OpenLinkPath(linkPath);
        }
    }
}