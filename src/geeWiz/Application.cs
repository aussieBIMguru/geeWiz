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
        public const string TAB_NAME = "geeWiz";
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
            Globals.RegisterTooltips("geeWiz.Resources.Files.Tooltips");

            // Register the warden commands
            Warden.Register(uiCtlApp);

            #endregion

            #region Construct Panel 1

            /// <summary>
            /// We will load our commands here later on.
            /// </summary>

            // Create the tab
            uiCtlApp.Ext_AddRibbonTab(TAB_NAME);

            // Add Panel1 to the tab
            var ribbonPanel1 = uiCtlApp.Ext_AddRibbonPanelToTab(TAB_NAME, PANEL1_NAME);

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
            var ribbonPanel2 = uiCtlApp.Ext_AddRibbonPanelToTab(TAB_NAME, PANEL2_NAME);

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

            // Panel 2 - Add Select pulldown
            // Panel 2 - Add Cmd_PickRooms to Select pulldown
            // Panel 2 - Add Cmd_PickWalls to Select pulldown
            // Panel 2 - Add Cmd_GetHidden to Select pulldown
            // Panel 2 - Add Cmd_GetTtbs to Select pulldown
            // Panel 2 - Add Cmd_RemoveGrouped to Select pulldown

            #region Pulldown - Workset

            // Add pushbuttons to Workset
            pulldownRevision.Ext_AddPushButton(
                buttonName: "Create worksets",
                commandClass: "geeWiz.Cmds_Workset.Cmd_Create",
                availability: gAva.Workshared);

            #endregion

            // Panel 2 - Add Import pulldown
            // Panel 2 - Add Cmd_ExcelTemplate to Import pulldown
            // Panel 2 - Add Cmd_ImportSheets to Import pulldown

            // Panel 2 - Add Export pulldown
            // Panel 2 - Add Cmd_ExportSchedule to Export pulldown
            // Panel 2 - Add Cmd_ExportPdf to Export pulldown
            // Panel 2 - Add Cmd_ExportDwg to Export pulldown

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