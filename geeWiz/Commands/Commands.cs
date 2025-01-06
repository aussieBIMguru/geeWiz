// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

// The class belongs to the Commands namespace
namespace geeWiz.Commands
{
    /// <summary>
    /// Describe your command here
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Temporary : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Return the result
            return Result.Succeeded;
        }
    }
}