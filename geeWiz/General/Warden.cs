// Autodesk
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
// geeWiz
using gFrm = geeWiz.Forms;

// The class belongs to the root namespace
namespace geeWiz
{
    /// <summary>
    /// Warden will intercept and cross check command use.
    /// </summary>
    public static class Warden
    {
        #region Register/deregister commands

        /// <summary>
        /// Registers all commands to Warden.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <returns>Void (nothing).</returns>
        public static void Register(UIControlledApplication uiApp)
        {
            WatchCommand(uiApp, commandName: "ID_INPLACE_COMPONENT");
            WatchCommand(uiApp, commandName: "ID_FILE_IMPORT");
            WatchCommand(uiApp, commandName: "ID_EDIT_PAINT");
        }

        /// <summary>
        /// Registers all commands to Warden.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <returns>Void (nothing).</returns>
        public static void DeRegister(UIControlledApplication uiApp)
        {
            IgnoreCommand(uiApp, commandName: "ID_INPLACE_COMPONENT");
            IgnoreCommand(uiApp, commandName: "ID_FILE_IMPORT");
            IgnoreCommand(uiApp, commandName: "ID_EDIT_PAINT");
        }

        #endregion

        #region Watch/ignore command

        /// <summary>
        /// Try to add a command to Warden.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="commandName">The internal name of the Command to watch.</param>
        /// <returns>Void (nothing).</returns>
        public static void WatchCommand(UIControlledApplication uiApp, string commandName)
        {
            // Look up the command Id by name
            var commandId = RevitCommandId.LookupCommandId(commandName);

            // Check if we can bind to the command
            if (commandId.CanHaveBinding && !commandId.HasBinding)
            {
                // If we can, create the binding
                uiApp.CreateAddInCommandBinding(commandId).Executed += new EventHandler<ExecutedEventArgs>(CatchCommand);
            }
        }

        /// <summary>
        /// Try to remove a command from Warden.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="commandName">The internal name of the Command to watch.</param>
        /// <returns>Void (nothing).</returns>
        public static void IgnoreCommand(UIControlledApplication uiApp, string commandName)
        {
            // Look up the command Id by name
            var commandId = RevitCommandId.LookupCommandId(commandName);

            // Check if we can bind to the command
            if (commandId.CanHaveBinding && commandId.HasBinding)
            {
                // If we can, remove the binding
                uiApp.CreateAddInCommandBinding(commandId).Executed -= new EventHandler<ExecutedEventArgs>(CatchCommand);
            }
        }

        #endregion

        #region Catch command

        /// <summary>
        /// This fires whenever the watched command is ran.
        /// </summary>
        /// <param name="sender">The event sender (command).</param>
        /// <param name="args">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        public static void CatchCommand(object sender, ExecutedEventArgs args)
        {
            // A variable as to whether we will let the command execute
            bool permit = true;

            // If Warden is active
            if (Globals.WardenActive)
            {
                // Ask the user if we want to permit the command
                var formResult = gFrm.Custom.Message(title: "Warden",
                    message: "Are you sure you want to run this command?\n\nIt is generally not good practice.",
                    yesNo: true,
                    warning: true);

                // If the user answered no or cancelled, permit becomes false
                permit = formResult.Affirmative;
            }

            // If we permit the command
            if (permit)
            {
                // Store idling and command Id
                Globals.LastCommandId = args.CommandId.Name;
                Globals.Idling = false;

                // Remove the binding, add the rebind to the idling event
                Globals.UiApp.RemoveAddInCommandBinding(args.CommandId);
                Globals.UiApp.Idling += new EventHandler<IdlingEventArgs>(RebindCommand);

                // Post the command (currently not bound)
                Globals.UiApp.PostCommand(args.CommandId);
            }
        }

        #endregion

        #region Rebind command

        /// <summary>
        /// This fires to rebind the command after being permitted.
        /// </summary>
        /// <param name="sender">The event sender (not used).</param>
        /// <param name="args">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private static void RebindCommand(object sender, IdlingEventArgs args)
        {
            // If we are idling
            if (Globals.Idling)
            {
                // Watch the command again
                WatchCommand(Globals.UiCtlApp, Globals.LastCommandId);

                // Remove this from the idling event
                Globals.UiApp.Idling -= RebindCommand;

                // End the command
                return;
            }

            // Tell the app it is idling as soon as this runs
            Globals.Idling = true;
        }

        #endregion
    }
}