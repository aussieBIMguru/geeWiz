// System
using System.Diagnostics;
// Autodesk
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.ApplicationServices;
// geeWiz
using gFrm = geeWiz.Forms;

// The class belongs to the root namespace
namespace geeWiz
{
    /// <summary>
    /// The sync timer informs the user of total sync times.
    /// </summary>
    public class SyncTimer
    {
        #region Constants

        // Sync time variable
        private static DateTime SYNC_START = default;

        #endregion

        #region Registration

        /// <summary>
        /// Registers the sync timer.
        /// </summary>
        /// <param name="ctlApp">The controlled application to subscribe to.</param>
        public static void Register(ControlledApplication ctlApp = null)
        {
            ctlApp ??= Globals.CtlApp;
            ctlApp.DocumentSynchronizingWithCentral += new EventHandler<DocumentSynchronizingWithCentralEventArgs>(DocumentSynchronizingWithCentral_SyncStart);
            ctlApp.DocumentSynchronizedWithCentral += new EventHandler<DocumentSynchronizedWithCentralEventArgs>(DocumentSynchronizedWithCentral_SyncEnds);
        }

        /// <summary>
        /// Deregisters the sync timer.
        /// </summary>
        /// <param name="ctlApp">The controlled application to unsubscribe from.</param>
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
        /// <param name="args">Event arguments.</param>
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
        /// <param name="args">Event arguments.</param>
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
            var elapsedTime = DateTime.Now - SYNC_START;
            var elapsedSeconds = elapsedTime.TotalSeconds;

            // Get components for time message
            var syncRating = RateSync(elapsedSeconds);
            var elapsedMinutes = (int)Math.Floor(elapsedTime.TotalMinutes);
            var excessSeconds = (int)Math.Ceiling(elapsedSeconds % 60);

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
        /// <returns>A string.</returns>
        private static string RateSync(double totalSeconds)
        {
            if (totalSeconds < 60) { return "A"; } // < 1 minute
            else if (totalSeconds < 180) { return "B"; } // < 3 minutes
            else if (totalSeconds < 300) { return "C"; } // < 5 minutes
            else if (totalSeconds < 600) { return "D"; } // < 10 minutes
            else if (totalSeconds < 900) { return "E"; } // < 15 minutes
            else { return "F"; } // > 15 minutes
        }

        #endregion
    }
}