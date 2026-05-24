// Revit API
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using View = Autodesk.Revit.DB.View;
// geeWiz
using geeWiz.Extensions;
using gFrm = geeWiz.Forms;
using gWsh = geeWiz.Utilities.Workshare_Utils;

// The class belongs to the Commands namespace
namespace geeWiz.Commands.Cmds_Audit
{
    /// <summary>
    /// A Revit command ran from the ribbon.
    /// Delete PatternElements from a list.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_DeletePatterns : IExternalCommand
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

            // Collect fill and line patterns
            List<Element> deletePatterns = doc.Ext_Collector()
                .OfClass(typeof(FillPatternElement))
                .Concat(
                    doc.Ext_Collector()
                    .OfClass(typeof(LinePatternElement))
                    )
                .Cast<Element>()
                .Where(e => e.Name.ToUpper().StartsWith("IMPORT"))
                .ToList();

            // Keep editable elements only
            if (doc.IsWorkshared)
            {
                var worksharingResults = gWsh.ProcessElements<Element>(deletePatterns, doc);
                deletePatterns = worksharingResults.Editable;
            }

            // Deletion routine
            return doc.Ext_DeleteElementsRoutine<Element>(deletePatterns, typeName: "Fill/Line Pattern");
        }
    }

    /// <summary>
    /// A Revit command ran from the ribbon.
    /// Purges unplaced rooms from a list.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_PurgeRooms : IExternalCommand
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

            // Collect unplaced rooms
            List<SpatialElement> rooms = doc.Ext_GetRooms(
                includePlaced: false,
                includeUnplaced: true,
                sorted: true);

            // Check if we have no unplaced rooms
            if (rooms.Count == 0)
            {
                return gFrm.Custom.Completed("No unplaced Rooms found in the current document.");
            }

            // Select rooms from a list
            var formResult = doc.Ext_SelectRooms(rooms: rooms, title: "Select rooms to delete");
            if (formResult.Cancelled) { return Result.Cancelled; }
            List<SpatialElement> deleteRooms = formResult.Objects;

            // Keep editable elements only
            if (doc.IsWorkshared)
            {
                var worksharingResults = gWsh.ProcessElements<SpatialElement>(deleteRooms, doc);
                deleteRooms = worksharingResults.Editable;
            }

            // Deletion routine
            return doc.Ext_DeleteElementsRoutine<SpatialElement>(deleteRooms, typeName: "unplaced Room");
        }
    }

    /// <summary>
    /// A Revit command ran from the ribbon.
    /// Purges unused View Templates from a list.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_PurgeTemplates : IExternalCommand
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

            // Get used view templates Id strings
            List<string> usedIdStrings = doc.Ext_GetViewFamilyTypes()
                .Select(vft => vft.DefaultTemplateId.ToString())
                .Concat(
                    doc.Ext_GetViews()
                    .Select(v => v.ViewTemplateId.ToString())
                    )
                .Distinct()
                .ToList();

            // Get unused view templates
            List<View> unusedTemplates = doc.Ext_GetViewTemplates(sorted: true)
                .Where(vt => !usedIdStrings.Contains(vt.Id.ToString()))
                .ToList();

            // Check if we have no unused templates
            if (unusedTemplates.Count == 0)
            {
                return gFrm.Custom.Completed("No unused View Templates found in the current document.");
            }

            // Select view templates from a list
            var formResult = doc.Ext_SelectViewTemplates(unusedTemplates, title: "Select templates to delete");
            if (formResult.Cancelled) { return Result.Cancelled; }
            List<View> deleteTemplates = formResult.Objects;

            // Keep editable elements only
            if (doc.IsWorkshared)
            {
                var worksharingResults = gWsh.ProcessElements<View>(deleteTemplates, doc);
                deleteTemplates = worksharingResults.Editable;
            }

            // Deletion routine
            return doc.Ext_DeleteElementsRoutine<View>(deleteTemplates, typeName: "View Template");
        }
    }

    /// <summary>
    /// A Revit command ran from the ribbon.
    /// Purges unused View Filters from a list.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Cmd_PurgeFilters : IExternalCommand
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

            // Get used view filter Id strings
            HashSet<string> usedIdStrings = doc.Ext_GetViews()
                .Concat(doc.Ext_GetViewTemplates())
                .SelectMany(v => v.GetFilters())
                .Select(i => i.ToString())
                .Distinct()
                .ToHashSet<string>();

            // Get unused view filters
            List<Element> unusedFilters = doc.Ext_Collector()
                .OfClass(typeof(ParameterFilterElement))
                .Where(f => !usedIdStrings.Contains(f.Id.ToString()))
                .OrderBy(f => f.Name)
                .ToList();

            // Check if we have no unused templates
            if (unusedFilters.Count == 0)
            {
                return gFrm.Custom.Completed("No unused View Filters found in the current document.");
            }

            // Construct keys
            List<string> keys = unusedFilters.Select(f => f.Name).ToList();

            // Select view filters from a list
            var formResult = gFrm.Custom.SelectFromList(keys, unusedFilters, "Select view filters to delete");
            if (formResult.Cancelled) { return Result.Cancelled; }
            List<Element> deleteFilters = formResult.Objects;

            // Keep editable elements only
            if (doc.IsWorkshared)
            {
                var worksharingResults = gWsh.ProcessElements(deleteFilters, doc);
                deleteFilters = worksharingResults.Editable;
            }

            // Deletion routine
            return doc.Ext_DeleteElementsRoutine(deleteFilters, typeName: "View Filter");
        }
    }
}