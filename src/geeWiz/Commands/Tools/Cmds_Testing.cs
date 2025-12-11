// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
// geeWiz
using geeWiz.Utilities;
using Mvvm = geeWiz.Forms.Mvvm;

// The class belongs to the Commands namespace
namespace geeWiz.Cmds_Testing
{
    #region Cmd_Testing

    /// <summary>
    /// Testing.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_TestGeneral: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the document
            var uiApp = commandData.Application;
            var uiDoc = uiApp.ActiveUIDocument;
            var doc = uiDoc.Document;
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// Testing.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_TestMvvm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (WindowController.Focus< Mvvm.Views.ViewSample>())
            {
                return Result.Succeeded;
            }

            var viewModel = new Mvvm.Models.ModelSample();
            var view = new Mvvm.Views.ViewSample(viewModel);

            WindowController.Show(view, Globals.UiApp.MainWindowHandle);

            return Result.Succeeded;
        }
    }

    #endregion
}