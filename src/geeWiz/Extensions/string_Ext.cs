// geeWiz
using gCnv = geeWiz.Utilities.Convert_Utils;

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

        #region Conversion

        /// <summary>
        /// Attempts to convert a string to an ElementId.
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <param name="valueOnFailure">Value to return if it can't convert.</param>
        /// <returns>An ElementId.</returns>
        public static ElementId Ext_StringToElementId(this string str, ElementId valueOnFailure = null)
        {
            // Catch invalid string
            if (str.Ext_HasNoChars()) { return valueOnFailure; }

            // Try to convert to an integer
            var stringAsInt = gCnv.StringToInt(str);
            if (!stringAsInt.HasValue) { return valueOnFailure; }

            // Convert integer to elementId
            return gCnv.IntToElementId(stringAsInt.Value, valueOnFailure);
        }

        /// <summary>
        /// Attempts to convert a string to an Element.
        /// </summary>
        /// <typeparam name="T">The type of Element to return.</typeparam>
        /// <param name="str">The string to convert.</param>
        /// <param name="doc">The document to get the ElementId from.</param>
        /// <returns>An Element.</returns>
        public static T Ext_StringToElementById<T>(this string str, Document doc = null)
        {
            // Catch invalid string
            doc ??= Globals.CurrentDocument();
            if (str.Ext_HasNoChars()) { return default; }

            // Try to convert to an integer
            var stringAsInt = gCnv.StringToInt(str);
            if (!stringAsInt.HasValue) { return default; }

            // Convert integer to ElementId, get that Element
            var elementId = gCnv.IntToElementId(stringAsInt.Value);
            return elementId.Ext_GetElement<T>(doc);
        }

        /// <summary>
        /// Attempts to convert a string to an Element.
        /// </summary>
        /// <typeparam name="T">The type of Element to check for.</typeparam>
        /// <param name="str">The name of the element to get.</param>
        /// <param name="doc">The document to get the ElementId from.</param>
        /// <returns>An Element.</returns>
        public static T Ext_StringToElementByName<T>(this string str, Document doc = null, bool isElementType = false) where T : Element
        {
            // Catch invalid string
            doc ??= Globals.CurrentDocument();
            if (str.Ext_HasNoChars()) { return default; }

            // Base collector
            var collector = new FilteredElementCollector(doc).OfClass(typeof(T));

            // Get element of element type
            if (isElementType)
            {
                collector = collector.WhereElementIsElementType();
            }
            else
            {
                collector = collector.WhereElementIsNotElementType();
            }
                
            // Get the first element with that name
            return collector.FirstOrDefault(e => e.Name == str) as T;
        }

        #endregion
    }
}