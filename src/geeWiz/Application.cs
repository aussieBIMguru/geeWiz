// Revit API
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
// geeWiz
using geeWiz.Extensions;
using gAva = geeWiz.Availability.AvailabilityNames;
using gRib = geeWiz.Utilities.Ribbon_Utils;

// The class belongs to the geeWiz namespace
namespace geeWiz
{
    /// <summary>
    /// This interface handles startup and shutdown of the application.
    /// </summary>
    public class Application : IExternalApplication
    {
        #region Class properties

        // Temporary variable to pass the UI controlled app to an idling event
        private static UIControlledApplication _uiCtlApp;

        // Ribbon construction constants
        public const string PANEL1_NAME = "General";
        public const string PANEL2_NAME = "Tools";

        #endregion

        /// <summary>
        /// Runs when the application starts.
        /// We use this part of the interface to initialize geeWiz.
        /// </summary>
        public Result OnStartup(UIControlledApplication uiCtlApp)
        {
            #region Register UiApp

            // Set private variable
            _uiCtlApp = uiCtlApp;

            // Try to subscribe to the idling event, which sets uiApp global ASAP
            try
            {
                _uiCtlApp.Idling += OnIdling;
            }
            catch
            {
                Globals.UiApp = null;
            }

            #endregion

            #region Register Globals and Warden

            // Store all other global variables and tooltips
            Globals.RegisterVariables(uiCtlApp);
            Globals.RegisterTooltips($"{Globals.AddinName}.Resources.Files.Tooltips");

            // Register the warden commands
            Warden.Register(uiCtlApp);

            #endregion

            #region Construct Panel 1

            /// <summary>
            /// We will load our commands here later on.
            /// </summary>

            // Create the tab
            uiCtlApp.Ext_AddRibbonTab(Globals.AddinName);

            // Add Panel1 to the tab
            var ribbonPanel1 = uiCtlApp.Ext_AddRibbonPanelToTab(Globals.AddinName, PANEL1_NAME);

            // Panel 1 - Add Cmd_About button
            ribbonPanel1.Ext_AddPushButton(buttonName: "About",
                commandClass: "geeWiz.Cmds_General.Cmd_About",
                availability: gAva.ZeroDoc);

            // Panel 1 - Add separator
            ribbonPanel1.AddSeparator();

            // Panel 1 - Add Settings pulldown
            var pullDownSettings = ribbonPanel1.Ext_AddPulldownButton(
                baseName: "Settings",
                buttonName: "Settings");

            // Panel 1 - Add Cmd_Warden button to Settings pulldown
            pullDownSettings.Ext_AddPushButton(
                buttonName: "Warden",
                commandClass: "geeWiz.Cmds_Settings.Cmd_Warden",
                availability: gAva.Document);

            // Panel 1 - Add Cmd_ColourTabs button to Settings pulldown
            pullDownSettings.Ext_AddPushButton(
                buttonName: "Coloured Tabs",
                commandClass: "geeWiz.Cmds_Settings.Cmd_ColourTabs",
                availability: gAva.Document);

            // Panel 1 - Add Cmd_UiToggle button to Settings pulldown
            gRib.AddButton_UiToggle(
                pulldownButton: pullDownSettings,
                commandClass: "geeWiz.Cmds_Settings.Cmd_UiToggle",
                availability: gAva.ZeroDoc);

            #endregion

            #region Construct Panel 2

            // Add Panel2 to the tab
            var ribbonPanel2 = uiCtlApp.Ext_AddRibbonPanelToTab(Globals.AddinName, PANEL2_NAME);

            #region Construct PulldownButton data

            // Construct pulldown data objects
            var dataAudit = gRib.NewPulldownButtonData(
                buttonName: "Audit",
                commandClass: "geeWiz.Cmds_Audit");
            var dataRevision = gRib.NewPulldownButtonData(
                buttonName: "Revision",
                commandClass: "geeWiz.Cmds_Revision");
            var dataSelect = gRib.NewPulldownButtonData(
                buttonName: "Select",
                commandClass: "geeWiz.Cmds_Select");
            var dataWorkset = gRib.NewPulldownButtonData(
                buttonName: "Workset",
                commandClass: "geeWiz.Cmds_Workset");
            var dataImport = gRib.NewPulldownButtonData(
                buttonName: "Import",
                commandClass: "geeWiz.Cmds_Import");
            var dataExport = gRib.NewPulldownButtonData(
                buttonName: "Export",
                commandClass: "geeWiz.Cmds_Export");

            #endregion

            #region Stack pulldowns

            // Construct stacks
            var stackedGroup2a = ribbonPanel2.AddStackedItems(dataAudit, dataRevision, dataSelect);
            var stackedGroup2b = ribbonPanel2.AddStackedItems(dataWorkset, dataImport, dataExport);

            // Retrieve pulldownbuttons
            var pulldownAudit = (PulldownButton)stackedGroup2a[0];
            var pulldownRevision = (PulldownButton)stackedGroup2a[1];
            var pulldownSelect = (PulldownButton)stackedGroup2a[2];
            var pulldownWorkset = (PulldownButton)stackedGroup2b[0];
            var pulldownImport = (PulldownButton)stackedGroup2b[1];
            var pulldownExport = (PulldownButton)stackedGroup2b[2];

            #endregion

            #region Pulldown - Audit

            // Add pushbuttons to Audit
            pulldownAudit.Ext_AddPushButton(
                buttonName: "Delete imported patterns",
                commandClass: "geeWiz.Cmds_Audit.Cmd_DeletePatterns",
                availability: gAva.Document);

            pulldownAudit.AddSeparator();

            pulldownAudit.Ext_AddPushButton(
                buttonName: "Purge unplaced rooms",
                commandClass: "geeWiz.Cmds_Audit.Cmd_PurgeRooms",
                availability: gAva.Project);
            pulldownAudit.Ext_AddPushButton(
                buttonName: "Purge unused view templates",
                commandClass: "geeWiz.Cmds_Audit.Cmd_PurgeTemplates",
                availability: gAva.Document);
            pulldownAudit.Ext_AddPushButton(
                buttonName: "Purge unused view filters",
                commandClass: "geeWiz.Cmds_Audit.Cmd_PurgeFilters",
                availability: gAva.Document);

            #endregion

            #region Pulldown - Revision

            // Add pushbuttons to Revision
            pulldownRevision.Ext_AddPushButton(
                buttonName: "Bulk revision",
                commandClass: "geeWiz.Cmds_Revision.Cmd_BulkRev",
                availability: gAva.Document);
            pulldownRevision.Ext_AddPushButton(
                buttonName: "Sheet set by revision",
                commandClass: "geeWiz.Cmds_Revision.Cmd_RevSet",
                availability: gAva.Document);
            pulldownRevision.Ext_AddPushButton(
                buttonName: "Create Excel transmittal",
                commandClass: "geeWiz.Cmds_Revision.Cmd_DocTrans",
                availability: gAva.Document);

            #endregion

            #region Pulldown - Select

            // Add pushbuttons to Select
            pulldownSelect.Ext_AddPushButton(
                buttonName: "Pick rooms",
                commandClass: "geeWiz.Cmds_Select.Cmd_PickRooms",
                availability: gAva.Document);
            pulldownSelect.Ext_AddPushButton(
                buttonName: "Pick walls",
                commandClass: "geeWiz.Cmds_Select.Cmd_PickWalls",
                availability: gAva.Document);

            pulldownSelect.AddSeparator();

            pulldownSelect.Ext_AddPushButton(
                buttonName: "Get hidden elements",
                commandClass: "geeWiz.Cmds_Select.Cmd_GetHidden",
                availability: gAva.Document);
            pulldownSelect.Ext_AddPushButton(
                buttonName: "Get sheet titleblocks",
                commandClass: "geeWiz.Cmds_Select.Cmd_GetTtbs",
                availability: gAva.SelectionOnlySheets);

            pulldownSelect.AddSeparator();

            pulldownSelect.Ext_AddPushButton(
                buttonName: "Remove grouped elements",
                commandClass: "geeWiz.Cmds_Select.Cmd_RemoveGrouped",
                availability: gAva.Selection);

            #endregion

            #region Pulldown - Workset

            // Add pushbuttons to Workset
            pulldownWorkset.Ext_AddPushButton(
                buttonName: "Create worksets",
                commandClass: "geeWiz.Cmds_Workset.Cmd_Create",
                availability: gAva.Workshared);

            #endregion

            #region Pulldown - Import

            // Add pushbuttons to Import
            pulldownImport.Ext_AddPushButton(
                buttonName: "Sheets to Excel",
                commandClass: "geeWiz.Cmds_Import.Cmd_SheetsExcel",
                availability: gAva.Project);

            pulldownImport.AddSeparator();

            pulldownImport.Ext_AddPushButton(
                buttonName: "Create/update sheets",
                commandClass: "geeWiz.Cmds_Import.Cmd_CreateSheets",
                availability: gAva.Project);

            #endregion

            #region Pulldown - Export

            // Add pushbuttons to Export
            pulldownExport.Ext_AddPushButton(
                buttonName: "Schedule to Excel",
                commandClass: "geeWiz.Cmds_Export.Cmd_Schedule",
                availability: gAva.ActiveViewSchedule);

            pulldownExport.AddSeparator();

            pulldownExport.Ext_AddPushButton(
                buttonName: "Sheets to Pdf",
                commandClass: "geeWiz.Cmds_Export.Cmd_SheetsPdf",
                availability: gAva.Project);
            pulldownExport.Ext_AddPushButton(
                buttonName: "Sheets to Dwg",
                commandClass: "geeWiz.Cmds_Export.Cmd_SheetsDwg",
                availability: gAva.Project);

            #endregion

            #endregion

            // Return succeeded
            return Result.Succeeded;
        }

        /// <summary>
        /// Runs when the application closes down.
        /// We use this part of the interface to cleanup geeWiz.
        /// </summary>
        public Result OnShutdown(UIControlledApplication application)
        {
            #region Unsubscribe from events

            // Deregister coloured tabs
            if (Globals.ColouringTabs) { ColouredTabs.DeRegister(); }

            // Deregister Warden
            if (Globals.WardenActive) { Warden.DeRegister(Globals.UiCtlApp); }

            #endregion

            // Return succeeded
            return Result.Succeeded;
        }

        #region Register UiApp on Idling

        /// <summary>
        /// Registers the uiApp global whenever first possible.
        /// </summary>
        /// <param name="sender"">The event sender object (the uiApp).</param>
        /// <param name="e"">The idling event arguments, unused.</param>
        /// <returns>Void (nothing).</returns>
        private void OnIdling(object sender, IdlingEventArgs e)
        {
            // Unsubscribe from the event (only runs once)
            _uiCtlApp.Idling -= OnIdling;

            // Register if possible (will generally be)
            if (sender is UIApplication uiApp)
            {
                Globals.UiApp = uiApp;
                Globals.UsernameRevit = uiApp.Application.Username;
            }
        }

        #endregion
    }
}