// System
// Revit API
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
// Navi testing
using Navi.InfoItems;
using System.IO;
using System.Reflection;
using gFrm = geeWiz.Forms;
// geeWiz
using gWin = geeWiz.Utilities.WindowController;
using Mvvm = geeWiz.Forms.Mvvm;

// The class belongs to the Commands namespace
namespace geeWiz.Commands.Cmds_Testing
{
    /// <summary>
    /// A Revit command ran from the ribbon.
    /// A general button you can use for testing things.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_TestGeneral: IExternalCommand
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
            // Get the document
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // TESTING NAVI ADDIN FOR A FRIEND
            // https://github.com/Exervsi/Navi

            // Select directory to read from
            var formResultDir1 = gFrm.Custom.SelectFolder("Select where files live");
            if (formResultDir1.Cancelled) { return Result.Cancelled; }

            // Make paths to needed files
            string dllPath = Path.Combine(formResultDir1.Object, $"{Globals.AddinName}.dll");
            string xmlPath = Path.Combine(formResultDir1.Object, $"{Globals.AddinName}.xml");

            if (!File.Exists(dllPath) || !File.Exists(xmlPath))
            {
                return gFrm.Custom.Error("Dll and or Xml file(s) not found.\n\n" +
                    "Expected paths:\n" +
                    $"- {dllPath}\n" +
                    $"- {xmlPath}");
            }

            // Produce docutree
            DocuTree docuTree = new DocuTree(dllPath, xmlPath);

            // Select directory to save to
            var formResultDir2 = gFrm.Custom.SelectFolder("Select where to save output");
            if (formResultDir2.Cancelled) { return Result.Cancelled; }

            // Produce docs (testing)
            docuTree.PrintDocusaurus(formResultDir2.Object);

            // Return success
            return Result.Succeeded;
        }
    }

    /// <summary>
    /// A Revit command ran from the ribbon.
    /// Demonstration of implementing a Mvvm system.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_TestMvvm : IExternalCommand
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
            // Focus to the window if it is already visible
            if (gWin.Focus< Mvvm.Views.ViewSample>())
            {
                return Result.Succeeded;
            }

            // Create and wire the view model
            UIApplication uiApp = commandData.Application;
            var viewModel = new Mvvm.Models.ModelSample();
            viewModel.WireExternalEvents(commandData.Application);

            // Create and show the view (pointer not doing anything)
            var view = new Mvvm.Views.ViewSample(viewModel);
            gWin.ShowWindow(view);

            // Return success
            return Result.Succeeded;
        }
    }
}