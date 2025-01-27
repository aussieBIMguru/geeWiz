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
        public const string COMMANDCLASS_1A = "geeWiz.Cmds_General";
        public const string COMMANDCLASS_1B = "geeWiz.Cmds_Settings";
        public const string COMMANDCLASS_2A = "geeWiz.Cmds_Tools";

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
                commandClass: $"{COMMANDCLASS_1A}.Cmd_About",
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
                commandClass: $"{COMMANDCLASS_1B}.Cmd_Warden",
                availability: gAva.Document);

            // Panel 1 - Add Cmd_ColourTabs button to Settings pulldown
            pullDownSettings.Ext_AddPushButton(
                buttonName: "Coloured Tabs",
                commandClass: $"{COMMANDCLASS_1B}.Cmd_ColourTabs",
                availability: gAva.Document);

            // Panel 1 - Add Cmd_UiToggle button to Settings pulldown
            gRib.AddButton_UiToggle(
                pulldownButton: pullDownSettings,
                commandClass: $"{COMMANDCLASS_1B}.Cmd_UiToggle",
                availability: gAva.ZeroDoc);

            #endregion

            #region Construct Panel 2

            // Add Panel2 to the tab
            var ribbonPanel2 = uiCtlApp.Ext_AddRibbonPanelToTab(TAB_NAME, PANEL2_NAME);

            // Panel 2 - Add Cmd_Testing button
            ribbonPanel2.Ext_AddPushButton(
                buttonName: "Testing",
                commandClass: $"{COMMANDCLASS_2A}.Cmd_Testing",
                availability: gAva.Project);

            // Panel 2 - Add Audit pulldown
            // Panel 2 - Add Cmd_DeletePatterns to Audit pulldown
            // Panel 2 - Add Cmd_PurgeRooms to Audit pulldown
            // Panel 2 - Add Cmd_PurgeTemplates to Audit pulldown
            // Panel 2 - Add Cmd_PurgeFilters to Audit pulldown

            // Panel 2 - Add Revision pulldown
            // Panel 2 - Add Cmd_BulkRev to Revision pulldown
            // Panel 2 - Add Cmd_RevSet to Revision pulldown
            // Panel 2 - Add Cmd_DocTrans to Revision pulldown

            // Panel 2 - Add Import pulldown
            // Panel 2 - Add Cmd_ExcelTemplate to Import pulldown
            // Panel 2 - Add Cmd_ImportSheets to Import pulldown

            // Panel 2 - Add Export pulldown
            // Panel 2 - Add Cmd_ExportSchedule to Export pulldown
            // Panel 2 - Add Cmd_ExportPdf to Export pulldown
            // Panel 2 - Add Cmd_ExportDwg to Export pulldown

            // Panel 2 - Add Select pulldown
            // Panel 2 - Add Cmd_PickRooms to Select pulldown
            // Panel 2 - Add Cmd_PickWalls to Select pulldown
            // Panel 2 - Add Cmd_GetHidden to Select pulldown
            // Panel 2 - Add Cmd_GetTtbs to Select pulldown
            // Panel 2 - Add Cmd_RemoveGrouped to Select pulldown

            // Panel 2 - Add Worksets pulldown
            // Panel 2 - Add Cmd_CreateWorksets to Select pulldown

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