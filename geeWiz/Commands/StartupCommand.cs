using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Toolkit.External;
using ClosedXML;
using DocumentFormat.OpenXml.Spreadsheet;
using UCol = geeWiz.Utilities.ColUtils;

namespace geeWiz.Commands
{
    /// <summary>
    ///     External command entry point invoked from the Revit interface
    /// </summary>
    [UsedImplicitly]
    [Transaction(TransactionMode.Manual)]
    public class StartupCommand : ExternalCommand
    {
        public override void Execute()
        {
            Workbook workbook = new Workbook();
            UCol.returnNothing();
            TaskDialog.Show(Document.Title, "geeWiz");
        }
    }
}