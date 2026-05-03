// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
// geeWiz
using gWin = geeWiz.Utilities.WindowController;
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

            // Return success
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
            // Focus to the window if it is already visible
            if (gWin.Focus< Mvvm.Views.ViewSample>())
            {
                return Result.Succeeded;
            }

            // Create and wire the view model
            var uiApp = commandData.Application;
            var viewModel = new Mvvm.Models.ModelSample();
            viewModel.WireExternalEvents(commandData.Application);

            // Create and show the view (pointer not doing anything)
            var view = new Mvvm.Views.ViewSample(viewModel);
            gWin.ShowWindow(view, uiApp.MainWindowHandle);

            // Return success
            return Result.Succeeded;
        }
    }

    #endregion
}