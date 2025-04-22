// Autodesk
using Autodesk.Revit.UI;

// The class belongs to the extensions namespace
// Result result.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to Results.
    /// </summary>
    public static class Result_Ext
    {
        #region Convert result

        /// <summary>
        /// Returns an integer based on the Result.
        /// </summary>
        /// <param name="result">A Result (extended).</param>
        /// <param name="ifSuccess">If successful.</param>
        /// <param name="ifNotSuccess">If not successful.</param>
        /// <returns>An integer.</returns>
        public static int Ext_ToInteger(this Result result, int ifSuccess = 1, int ifNotSuccess = 0)
        {
            return result == Result.Succeeded ? ifSuccess : ifNotSuccess;
        }

        /// <summary>
        /// Returns a boolean based on the Result.
        /// </summary>
        /// <param name="result">A Result (extended).</param>
        /// <param name="ifSuccess">If successful.</param>
        /// <param name="ifNotSuccess">If not successful.</param>
        /// <returns>A boolean.</returns>
        public static bool Ext_ToBoolean(this Result result, bool ifSuccess = true, bool ifNotSuccess = false)
        {
            return result == Result.Succeeded ? ifSuccess : ifNotSuccess;
        }

        #endregion
    }
}