// The class belongs to the extensions namespace
// Transaction t.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to Transactions.
    /// </summary>
    public static class Transaction_Ext
    {
        #region Safe handling

        /// <summary>
        /// Safely commits a transaction if it has started and not yet ended.
        /// </summary>
        /// <param name="t">The transaction.</param>
        public static void Ext_SafeCommit(this Transaction t)
        {
            // Null catch
            if (t == null)
            {
                return;
            }

            // Only commit if it has started and has not ended
            if (t.HasStarted() && !t.HasEnded())
            {
                t.Commit();
            }
        }

        /// <summary>
        /// Safely rolls back a transaction if it has started and not yet ended.
        /// </summary>
        /// <param name="t">The transaction.</param>
        public static void Ext_SafeRollBack(this Transaction t)
        {
            // Null catch
            if (t == null)
            {
                return;
            }

            // Only commit if it has started and has not ended
            if (t.HasStarted() && !t.HasEnded())
            {
                t.RollBack();
            }
        }

        #endregion
    }
}