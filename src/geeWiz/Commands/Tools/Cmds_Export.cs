// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
// geeWiz
using geeWiz.Extensions;
using gFrm = geeWiz.Forms;

// The class belongs to the Commands namespace
namespace geeWiz.Cmds_Export
{
    #region Cmd_Name

    /// <summary>
    /// Command description here.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_Testing : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            return Result.Succeeded;
        }
    }

    #endregion
}