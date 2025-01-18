// The class belongs to the root namespace
// using gErr = geeWiz.ErrorSwallower
namespace geeWiz
{
    /// <summary>
    /// Methods of this class generally relate to error supression.
    /// Run the AddToTransaction method to associate the swallower.
    /// </summary>
    public class ErrorSwallower : IFailuresPreprocessor
    {
        /// <summary>
        /// Part of the interface, suppresses warnings.
        /// </summary>
        /// <param name="a"">The failure accessor.</param>
        public FailureProcessingResult PreprocessFailures(FailuresAccessor a)
        {
            // Get related failures
            IList<FailureMessageAccessor> failures = a.GetFailureMessages();

            // Process the failures
            foreach (FailureMessageAccessor f in failures)
            {
                // Supress warning
                if (a.GetSeverity() == FailureSeverity.Warning)
                {
                    a.DeleteWarning(f);
                }
                // Otherwise, resolve as a failure
                else
                {
                    a.ResolveFailure(f);
                    return FailureProcessingResult.ProceedWithCommit;
                }
            }
            
            // Continue if no failure to resolve
            return FailureProcessingResult.Continue;
        }

        /// <summary>
        /// Attach a new error swallower to an open transaction.
        /// </summary>
        /// <param name="transaction"">A Revit transaction.</param>
        /// <returns>Void (nothing).</returns>
        public static void AddToTransaction(Transaction transaction)
        {
            // Honestly not sure how this part works, thanks chatGPT!
            FailureHandlingOptions options = transaction.GetFailureHandlingOptions();
            options.SetFailuresPreprocessor(new ErrorSwallower());
            transaction.SetFailureHandlingOptions(options);
        }
    }
}