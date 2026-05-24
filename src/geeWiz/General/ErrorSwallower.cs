// Revit API
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;

// The class belongs to the root namespace
// using gErr = geeWiz.ErrorSwallower
namespace geeWiz.General
{
    /// <summary>
    /// Methods of this class generally relate to error supression.
    /// Provide a transaction to associate it to it.
    /// Note that this is generally based on the pyRevit version.
    /// </summary>
    public class ErrorSwallower : IDisposable
    {
        #region Private variables

        /// <summary>
        /// The related Failure Swallower.
        /// </summary>
        private readonly FailureSwallower _failureSwallower;

        /// <summary>
        /// A flag if the object is disposed already.
        /// </summary>
        private bool _disposed = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="transaction">The associated Transaction (optional).</param>
        public ErrorSwallower(Transaction transaction = null)
        {
            // Set the failure swallower variable
            this._failureSwallower = new FailureSwallower();

            // If a transaction is provided...
            if (transaction is not null)
            {
                // Get failure handling options
                FailureHandlingOptions options = transaction.GetFailureHandlingOptions();

                // Set the failure processor as the failureswallower
                options.SetFailuresPreprocessor(this._failureSwallower);

                // Set the failure handling options of the transaction
                transaction.SetFailureHandlingOptions(options);
            }
        }

        #endregion

        #region Process failures method

        /// <summary>
        /// Called by the ProcessingFailures event in the FailureAccessor.
        /// </summary>
        /// <param name="sender">Event sender (Failure accessor).</param>
        /// <param name="args">Event arguments.</param>
        private void OnFailureProcessing(object sender, FailuresProcessingEventArgs args)
        {
            // Try to process failures
            try
            {
                // Get the failure accessor and processing result
                FailuresAccessor failureAccessor = args.GetFailuresAccessor();
                FailureProcessingResult result = args.GetProcessingResult();

                // Process the failures, set the result
                result = this._failureSwallower.PreprocessFailures(failureAccessor);
                args.SetProcessingResult(result);
            }
            // If it fails, do nothing
            catch
            {
                {; }
            }
        }

        #endregion

        #region Start and dispose

        /// <summary>
        /// Registers the FailureProcessing event.
        /// </summary>
        public void Start()
        {
            Globals.CtlApp.FailuresProcessing += OnFailureProcessing;
        }

        /// <summary>
        /// On disposal, delays the finalizer.
        /// </summary>
        public void Dispose()
        {
            // Dispose the object
            this.Dispose(true);
            
            // Supress garbage collector
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes and unsubscribes from the event.
        /// </summary>
        /// <param name="disposing">If we should try to dispose.</param>
        protected virtual void Dispose(bool disposing)
        {
            // If we have not disposed yet...
            if (!this._disposed)
            {
                // If we are trying to dispose...
                if (disposing)
                {
                    // Unsubscribe from failure processing event
                    Globals.CtlApp.FailuresProcessing -= OnFailureProcessing;
                }

                // Set internal disposal flag
                this._disposed = true;
            }
        }

        /// <summary>
        /// Deconstructor to ensure final disposal.
        /// </summary>
        ~ErrorSwallower()
        {
            // Run the dispose method
            this.Dispose(false);
        }

        #endregion
    }

    /// <summary>
    /// This class processes common bypassable errors/failures.
    /// </summary>
    public class FailureSwallower : IFailuresPreprocessor
    {
        #region Private variables

        /// <summary>
        /// A list of common failures we can generally bypass.
        /// </summary>
        private static readonly List<FailureResolutionType> RESOLUTIONS = new List<FailureResolutionType>()
        {
            FailureResolutionType.CreateElements,
            FailureResolutionType.DeleteElements,
            FailureResolutionType.DetachElements,
            FailureResolutionType.FixElements,
            FailureResolutionType.MoveElements,
            FailureResolutionType.QuitEditMode,
            FailureResolutionType.SaveDocument,
            FailureResolutionType.SetValue,
            FailureResolutionType.SkipElements,
            FailureResolutionType.UnlockConstraints
        };

        #endregion

        #region PreprocessFailures

        /// <summary>
        /// Process failures (provided via the interface).
        /// </summary>
        /// <param name="failureAccessor">The related FailureAccessor.</param>
        /// <returns>A FailureProcessingResult.</returns>
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failureAccessor)
        {
            // Get severity, no commit by default
            FailureSeverity severity = failureAccessor.GetSeverity();
            bool commitRequired = false;

            // If no severity, we can continue
            if (severity == FailureSeverity.None)
            {
                return FailureProcessingResult.Continue;
            }

            // For each failure in the messages...
            foreach (FailureMessageAccessor failure in failureAccessor.GetFailureMessages())
            {
                // Get severity
                FailureSeverity failureSeverity = failure.GetSeverity();

                // If it's a warning...
                if (!failure.HasResolutions() && failureSeverity == FailureSeverity.Warning)
                {
                    // Delete the warning, it is skippable
                    failureAccessor.DeleteWarning(failure);
                    continue;
                }

                // Otherwise, we get the default resolution type
                FailureDefinitionRegistry failureRegistry = Autodesk.Revit.ApplicationServices.Application.GetFailureDefinitionRegistry();
                FailureDefinitionId fid = new FailureDefinitionId(failure.GetFailureDefinitionId().Guid);
                FailureDefinitionAccessor failureDefinitionId = failureRegistry.FindFailureDefinition(fid);
                FailureResolutionType defaultResolution = failureDefinitionId.GetDefaultResolutionType();

                // For each type of typical resolution...
                foreach (FailureResolutionType resolutionType in RESOLUTIONS)
                {
                    // If it is the default resolution or has a resolution of this type...
                    if (defaultResolution == resolutionType || failure.HasResolutionOfType(resolutionType))
                    {
                        // Try to resolve the failure
                        try
                        {
                            failure.SetCurrentResolutionType(resolutionType);
                            failureAccessor.ResolveFailure(failure);
                        }
                        // If we can't, pass
                        catch {; }

                        // We need to commit the failure processing
                        commitRequired = true;
                        break;
                    }
                }
            }

            // Return the failure processing result for the event
            if (commitRequired)
            {
                return FailureProcessingResult.ProceedWithCommit;
            }
            else
            {
                return FailureProcessingResult.Continue;
            }
        }

        #endregion
    }
}