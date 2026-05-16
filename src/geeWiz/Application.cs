// Revit API
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
// geeWiz
using geeWiz.Extensions;
using gAva = geeWiz.General.Availability.AvailabilityNames;
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
        public const string PANELD_NAME = "DEBUG";

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

            #region Register Globals and Automations

            // Store all other global variables and tooltips
            Globals.RegisterVariables(uiCtlApp);
            Globals.RegisterTooltips($"{Globals.AddinName}.Resources.Files.Tooltips");

            // Register the warden commands
            General.Warden.Register(uiCtlApp);

            // Register the sync timer
            General.SyncTimer.Register(uiCtlApp.ControlledApplication);

            #endregion

            #region Construct Panel 1

            /// <summary>
            /// We will load our commands here later on.
            /// </summary>

            // Root command namespace
            string commandNamespace = $"{Globals.AddinName}.Commands";

            // Create the tab
            uiCtlApp.Ext_AddRibbonTab(Globals.AddinName);

            // Add Panel1 to the tab
            RibbonPanel ribbonPanel1 = uiCtlApp.Ext_AddRibbonPanelToTab(Globals.AddinName, PANEL1_NAME);

            // Panel 1 - Add Cmd_About button
            ribbonPanel1.Ext_AddPushButton<Commands.Cmds_General.Cmd_About>(
                buttonName: "About", availability: gAva.ZeroDoc);

            // Panel 1 - Add separator
            ribbonPanel1.AddSeparator();

            // Panel 1 - Add Settings pulldown
            PulldownButton pullDownSettings = ribbonPanel1.Ext_AddPulldownButton(
                buttonName: "Settings",
                nameSpace: $"{commandNamespace}.Cmds_Settings");

            // Panel 1 - Add Cmd_Warden button to Settings pulldown
            pullDownSettings.Ext_AddPushButton<Commands.Cmds_Settings.Cmd_Warden>(
                buttonName: "Warden", availability: gAva.Document);

            // Panel 1 - Add Cmd_ColourTabs button to Settings pulldown
            pullDownSettings.Ext_AddPushButton<Commands.Cmds_Settings.Cmd_ColourTabs>(
                buttonName: "Coloured Tabs",
                availability: gAva.Document);

            // Panel 1 - Add Cmd_UiToggle button to Settings pulldown
            gRib.AddButton_UiToggle<Commands.Cmds_Settings.Cmd_UiToggle>(
                pulldownButton: pullDownSettings, availability: gAva.ZeroDoc);

            #endregion

            #region Construct Panel 2

            // Add Panel2 to the tab
            RibbonPanel ribbonPanel2 = uiCtlApp.Ext_AddRibbonPanelToTab(Globals.AddinName, PANEL2_NAME);

            #region Construct PulldownButton data

            // Construct pulldown data objects
            PulldownButtonData dataAudit = gRib.NewPulldownButtonData(
                buttonName: "Audit",
                nameSpace: $"{commandNamespace}.Cmds_Audit");

            PulldownButtonData dataRevision = gRib.NewPulldownButtonData(
                buttonName: "Revision",
                nameSpace: $"{commandNamespace}.Cmds_Revision");

            PulldownButtonData dataSelect = gRib.NewPulldownButtonData(
                buttonName: "Select",
                nameSpace: $"{commandNamespace}.Cmds_Select");

            PulldownButtonData dataWorkset = gRib.NewPulldownButtonData(
                buttonName: "Workset",
                nameSpace: $"{commandNamespace}.Cmds_Workset");

            PulldownButtonData dataImport = gRib.NewPulldownButtonData(
                buttonName: "Import",
                nameSpace: $"{commandNamespace}.Cmds_Import");

            PulldownButtonData dataExport = gRib.NewPulldownButtonData(
                buttonName: "Export",
                nameSpace: $"{commandNamespace}.Cmds_Export");

            #endregion

            #region Stack pulldowns

            // Construct stacks
            IList<RibbonItem> stackedGroup2a = ribbonPanel2.AddStackedItems(dataAudit, dataRevision, dataSelect);
            IList<RibbonItem> stackedGroup2b = ribbonPanel2.AddStackedItems(dataWorkset, dataImport, dataExport);

            // Retrieve pulldownbuttons
            PulldownButton pulldownAudit = (PulldownButton)stackedGroup2a[0];
            PulldownButton pulldownRevision = (PulldownButton)stackedGroup2a[1];
            PulldownButton pulldownSelect = (PulldownButton)stackedGroup2a[2];
            PulldownButton pulldownWorkset = (PulldownButton)stackedGroup2b[0];
            PulldownButton pulldownImport = (PulldownButton)stackedGroup2b[1];
            PulldownButton pulldownExport = (PulldownButton)stackedGroup2b[2];

            #endregion

            #region Pulldown - Audit

            // Add pushbuttons to Audit
            pulldownAudit.Ext_AddPushButton<Commands.Cmds_Audit.Cmd_DeletePatterns>(
                buttonName: "Delete imported patterns", availability: gAva.Document);

            pulldownAudit.AddSeparator();

            pulldownAudit.Ext_AddPushButton<Commands.Cmds_Audit.Cmd_PurgeRooms>(
                buttonName: "Purge unplaced rooms", availability: gAva.Project);
            
            pulldownAudit.Ext_AddPushButton<Commands.Cmds_Audit.Cmd_PurgeTemplates>(
                buttonName: "Purge unused view templates", availability: gAva.Document);
            
            pulldownAudit.Ext_AddPushButton<Commands.Cmds_Audit.Cmd_PurgeFilters>(
                buttonName: "Purge unused view filters", availability: gAva.Document);

            #endregion

            #region Pulldown - Revision

            // Add pushbuttons to Revision
            pulldownRevision.Ext_AddPushButton<Commands.Cmds_Revision.Cmd_BulkRev>(
                buttonName: "Bulk revision", availability: gAva.Document);

            pulldownRevision.Ext_AddPushButton<Commands.Cmds_Revision.Cmd_RevSet>(
                buttonName: "Sheet set by revision", availability: gAva.Document);

            pulldownRevision.Ext_AddPushButton<Commands.Cmds_Revision.Cmd_DocTrans>(
                buttonName: "Create Excel transmittal", availability: gAva.Document);

            #endregion

            #region Pulldown - Select

            // Add pushbuttons to Select
            pulldownSelect.Ext_AddPushButton<Commands.Cmds_Select.Cmd_PickRooms>(
                buttonName: "Pick rooms", availability: gAva.Document);

            pulldownSelect.Ext_AddPushButton<Commands.Cmds_Select.Cmd_PickWalls>(
                buttonName: "Pick walls", availability: gAva.Document);

            pulldownSelect.AddSeparator();

            pulldownSelect.Ext_AddPushButton<Commands.Cmds_Select.Cmd_GetHidden>(
                buttonName: "Get hidden elements", availability: gAva.Document);

            pulldownSelect.Ext_AddPushButton<Commands.Cmds_Select.Cmd_GetTtbs>(
                buttonName: "Get sheet titleblocks", availability: gAva.SelectionOnlySheets);

            pulldownSelect.AddSeparator();

            pulldownSelect.Ext_AddPushButton<Commands.Cmds_Select.Cmd_RemoveGrouped>(
                buttonName: "Remove grouped elements", availability: gAva.Selection);

            #endregion

            #region Pulldown - Workset

            // Add pushbuttons to Workset
            pulldownWorkset.Ext_AddPushButton<Commands.Cmds_Workset.Cmd_Create>(
                buttonName: "Create worksets", availability: gAva.Workshared);

            #endregion

            #region Pulldown - Import

            // Add pushbuttons to Import
            pulldownImport.Ext_AddPushButton<Commands.Cmds_Import.Cmd_SheetsExcel>(
                buttonName: "Sheets to Excel", availability: gAva.Project);

            pulldownImport.AddSeparator();

            pulldownImport.Ext_AddPushButton<Commands.Cmds_Import.Cmd_CreateSheets>(
                buttonName: "Create/update sheets", availability: gAva.Project);

            #endregion

            #region Pulldown - Export

            // Add pushbuttons to Export
            pulldownExport.Ext_AddPushButton<Commands.Cmds_Export.Cmd_Schedule>(
                buttonName: "Schedule to Excel", availability: gAva.ActiveViewSchedule);

            pulldownExport.AddSeparator();

            pulldownExport.Ext_AddPushButton<Commands.Cmds_Export.Cmd_SheetsPdf>(
                buttonName: "Sheets to Pdf", availability: gAva.Project);

            pulldownExport.Ext_AddPushButton<Commands.Cmds_Export.Cmd_SheetsDwg>(
                buttonName: "Sheets to Dwg", availability: gAva.Project);

            #endregion

            #endregion

            #region Panel Debug

            // Only add the Debug panel when in debug mode
#if DEBUG
            var ribbonPanelDebug = uiCtlApp.Ext_AddRibbonPanelToTab(Globals.AddinName, PANELD_NAME);
            ribbonPanelDebug.Ext_AddPushButton<Commands.Cmds_Testing.Cmd_TestGeneral>("Test", gAva.Project);
            ribbonPanelDebug.Ext_AddPushButton<Commands.Cmds_Testing.Cmd_TestMvvm>("Mvvm", gAva.Project);
#endif
            #endregion

            // Return succeeded
            return Result.Succeeded;
        }

        /// <summary>
        /// Runs when the application closes down.
        /// We use this part of the interface to cleanup geeWiz.
        /// </summary>
        public Result OnShutdown(UIControlledApplication uiCtlApp)
        {
            #region Unsubscribe from events

            // Deregister coloured tabs
            General.ColouredTabs.DeRegister(uiCtlApp.ControlledApplication, Globals.UiApp);

            // Deregister Warden
            General.Warden.DeRegister(uiCtlApp);

            // Deregister SyncTimer
            General.SyncTimer.DeRegister(uiCtlApp.ControlledApplication);

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