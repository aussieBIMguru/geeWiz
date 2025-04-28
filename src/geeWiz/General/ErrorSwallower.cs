// Autodesk
using Autodesk.Revit.DB.Events;

// The class belongs to the root namespace
// using gErr = geeWiz.ErrorSwallower
namespace geeWiz
{
    /// <summary>
    /// Methods of this class generally relate to error supression.
    /// Provide a transaction to associate it to it.
    /// Note that this is generally based on the pyRevit version.
    /// </summary>
    public class ErrorSwallower : IDisposable
    {
        #region Private variables

        // Private variables for the class
        private readonly FailureSwallower _failureSwallower;
        private bool _disposed = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs an ErrorSwallower.
        /// </summary>
        /// <param name="transaction">An optional transaction to add the swallower to.</param>
        /// <returns>An ErrorSwallower object</returns>
        public ErrorSwallower(Transaction transaction = null)
        {
            // Set the failure swallower variable
            _failureSwallower = new FailureSwallower();

            // If a transaction is provided...
            if (transaction is not null)
            {
                // Get failure handling options
                var options = transaction.GetFailureHandlingOptions();

                // Set the failure processor as the failureswallower
                options.SetFailuresPreprocessor(_failureSwallower);

                // Set the failure handling options of the transaction
                transaction.SetFailureHandlingOptions(options);
            }
        }

        #endregion

        #region Process failures method

        /// <summary>
        /// Called by the ProcessingFailures event in the FailureAccessor.
        /// </summary>
        /// <param name="sender">The failure accessor.</param>
        /// <param name="args">Related event arguments.</param>
        /// <returns>Void (nothing)</returns>
        private void OnFailureProcessing(object sender, FailuresProcessingEventArgs args)
        {
            // Try to process failures
            try
            {
                // Get the failure accessor and processing result
                var failureAccessor = args.GetFailuresAccessor();
                var result = args.GetProcessingResult();

                // Process the failures, set the result
                result = _failureSwallower.PreprocessFailures(failureAccessor);
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
        /// On creation, add to failure processing event.
        /// </summary>
        /// <returns>Void (nothing)</returns>
        public void Start()
        {
            // Subscribe to failure processing event
            Globals.CtlApp.FailuresProcessing += OnFailureProcessing;
        }

        /// <summary>
        /// Override to delay the finalizer.
        /// </summary>
        /// <returns>Void (nothing)</returns>
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
        /// <returns>Void (nothing)</returns>
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
        /// Deconstructor to ensure disposal
        /// </summary>
        ~ErrorSwallower()
        {
            // Run the dispose method
            this.Dispose(false);
        }

        #endregion
    }

    /// <summary>
    /// This class deals with failures when provided to the preprocessor.
    /// </summary>
    public class FailureSwallower : IFailuresPreprocessor
    {
        #region Private variables

        // Private variable for resolvable failures
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

        // Interface method for processing failures
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failureAccessor)
        {
            // Get severity, no commit by default
            var severity = failureAccessor.GetSeverity();
            bool commitRequired = false;

            // If no severity, we can continue
            if (severity == FailureSeverity.None)
            {
                return FailureProcessingResult.Continue;
            }

            // For each failure in the messages...
            foreach (var failure in failureAccessor.GetFailureMessages())
            {
                // Get severity
                var failureSeverity = failure.GetSeverity();

                // If it's a warning...
                if (!failure.HasResolutions() && failureSeverity == FailureSeverity.Warning)
                {
                    // Delete the warning, it is skippable
                    failureAccessor.DeleteWarning(failure);
                    continue;
                }

                // Otherwise, we get the default resolution type
                var failureRegistry = Autodesk.Revit.ApplicationServices.Application.GetFailureDefinitionRegistry();
                var fid = new FailureDefinitionId(failure.GetFailureDefinitionId().Guid);
                var failureDefinitionId = failureRegistry.FindFailureDefinition(fid);
                var defaultResolution = failureDefinitionId.GetDefaultResolutionType();

                // For each type of typical resolution...
                foreach (var resolutionType in RESOLUTIONS)
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