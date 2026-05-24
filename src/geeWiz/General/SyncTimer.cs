// System
using System.Diagnostics;
// Revit API
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.ApplicationServices;
// geeWiz
using gFrm = geeWiz.Forms;

// The class belongs to the root namespace
namespace geeWiz.General
{
    /// <summary>
    /// This class is used to track Synchronization times.
    /// </summary>
    public static class SyncTimer
    {
        #region Constants

        /// <summary>
        /// The time the last sync started at.
        /// </summary>
        private static DateTime SYNC_START = default;

        #endregion

        #region Registration to events

        /// <summary>
        /// Registers the system to related Revit events.
        /// </summary>
        /// <param name="ctlApp">The ControlledApplication.</param>
        public static void Register(ControlledApplication ctlApp = null)
        {
            ctlApp ??= Globals.CtlApp;

            ctlApp.DocumentSynchronizingWithCentral += new EventHandler<DocumentSynchronizingWithCentralEventArgs>(DocumentSynchronizingWithCentral_SyncStart);
            ctlApp.DocumentSynchronizedWithCentral += new EventHandler<DocumentSynchronizedWithCentralEventArgs>(DocumentSynchronizedWithCentral_SyncEnds);
        }

        /// <summary>
        /// Deregisters the system from related Revit events.
        /// </summary>
        /// <param name="ctlApp">The ControlledApplication.</param>
        public static void DeRegister(ControlledApplication ctlApp = null)
        {
            ctlApp ??= Globals.CtlApp;

            ctlApp.DocumentSynchronizingWithCentral -= new EventHandler<DocumentSynchronizingWithCentralEventArgs>(DocumentSynchronizingWithCentral_SyncStart);
            ctlApp.DocumentSynchronizedWithCentral -= new EventHandler<DocumentSynchronizedWithCentralEventArgs>(DocumentSynchronizedWithCentral_SyncEnds);
        }

        #endregion

        #region Sync start/end

        /// <summary>
        /// Store the time when the sync begins.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Related event arguments.</param>
        private static void DocumentSynchronizingWithCentral_SyncStart(object sender, DocumentSynchronizingWithCentralEventArgs args)
        {
            // This should always work, but to be safe we try - otherwise the sync will fail
            try
            {
                SYNC_START = DateTime.Now;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SyncTimer error: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks the sync time when the sync ends.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="args">Related event arguments.</param>
        private static void DocumentSynchronizedWithCentral_SyncEnds(object sender, DocumentSynchronizedWithCentralEventArgs args)
        {
            // Catch if sync start is invalid
            if (SYNC_START == default) { return; }

            // Try to assess the sync, debug error if it fails
            // This is safer as we can block the sync if anything does!
            try
            {
                SyncAssess();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SyncTimer error: {ex.Message}");
            }
        }

        #endregion

        #region Rate sync utilities

        /// <summary>
        /// Show the sync assessment.
        /// </summary>
        private static void SyncAssess()
        {
            // Get the elapsed time and seconds
            TimeSpan elapsedTime = DateTime.Now - SYNC_START;
            double elapsedSeconds = elapsedTime.TotalSeconds;

            // Get components for time message
            string syncRating = RateSync(elapsedSeconds);
            int elapsedMinutes = (int)Math.Floor(elapsedTime.TotalMinutes);
            int excessSeconds = (int)Math.Ceiling(elapsedSeconds % 60);

            // Construct the message
            string syncTime;

            if (elapsedMinutes == 0)
            {
                syncTime = $"{excessSeconds}s";
            }
            else
            {
                syncTime = $"{elapsedMinutes}m {excessSeconds}s";
            }

            // Display the message
            gFrm.Custom.BubbleMessage(title: $"Sync rating: {syncRating}",
                message: $"Duration: {syncTime}");
        }

        /// <summary>
        /// Returns a rating for a given sync time.
        /// </summary>
        /// <param name="totalSeconds">The total sync seconds.</param>
        /// <returns>A String.</returns>
        private static string RateSync(double totalSeconds)
        {
            return totalSeconds switch
            {
                < 60 => "A", // < 1 minute
                < 180 => "B", // < 3 minutes
                < 300 => "C", // < 5 minutes
                < 600 => "D", // < 10 minutes
                < 900 => "E", // < 15 minutes
                _ => "F" // >= 15 minutes
            };
        }

        #endregion
    }
}