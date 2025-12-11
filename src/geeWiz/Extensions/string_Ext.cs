// The class belongs to the extensions namespace
// string string.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to DesignOptions.
    /// </summary>
    public static class string_Ext
    {
        #region Validation

        /// <summary>
        /// Returns if a string has characters.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>A boolean.</returns>
        public static bool Ext_HasChars(this string str)
        {
            return str?.Length > 0;
        }

        /// <summary>
        /// Returns if a string has no characters.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>A boolean.</returns>
        public static bool Ext_HasNoChars(this string str)
        {
            return !str.Ext_HasChars();
        }

        /// <summary>
        /// If string is null, substitutes it.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="ifNull">Value to replace if null (optional).</param>
        /// <returns>A string.</returns>
        public static string Ext_DeNull(this string str, string ifNull = "")
        {
            return str ?? ifNull;
        }

        #endregion
    }
}