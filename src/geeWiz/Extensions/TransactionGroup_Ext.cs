// The class belongs to the extensions namespace
// TransactionGroup tg.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to TransactionGroups.
    /// </summary>
    public static class TransactionGroup_Ext
    {
        #region Safe handling

        /// <summary>
        /// Safely assimilates a transaction group if it has started and not yet ended.
        /// </summary>
        /// <param name="tg">The TransactionGroup.</param>
        public static void Ext_SafeAssimilate(this TransactionGroup tg)
        {
            // Null catch
            if (tg == null)
            {
                return;
            }

            // Only assimilate if it has started and has not ended
            if (tg.HasStarted() && !tg.HasEnded())
            {
                tg.Assimilate();
            }
        }

        /// <summary>
        /// Safely rolls back a transaction group if it has started and not yet ended.
        /// </summary>
        /// <param name="tg">The TransactionGroup.</param>
        public static void Ext_SafeRollBack(this TransactionGroup tg)
        {
            // Null catch
            if (tg == null)
            {
                return;
            }

            // Only rollback if it has started and has not ended
            if (tg.HasStarted() && !tg.HasEnded())
            {
                tg.RollBack();
            }
        }

        #endregion
    }
}