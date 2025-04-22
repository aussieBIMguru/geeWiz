// The class belongs to the extensions namespace
// ElementId elementId.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to ElementIds.
    /// </summary>
    public static class ElementId_Ext
    {
        #region Get Element

        /// <summary>
        /// Returns if an ElementId is null or invalid.
        /// </summary>
        /// <param name="elementId">The ElementId (extended).</param>
        /// <returns>A boolean.</returns>
        public static bool Ext_IsNullOrInvalid(this ElementId elementId)
        {
            return elementId is null || elementId == ElementId.InvalidElementId;
        }

        /// <summary>
        /// Returns if an ElementId is valid.
        /// </summary>
        /// <param name="elementId">The ElementId (extended).</param>
        /// <returns>A boolean.</returns>
        public static bool Ext_IsValid(this ElementId elementId)
        {
            return elementId is not null && elementId != ElementId.InvalidElementId;
        }

        /// <summary>
        /// Gets an element from an ElementId and a Document.
        /// </summary>
        /// <param name="elementId">The ElementId (extended).</param>
        /// <param name="doc">The document to get the element from.</param>
        /// <returns>An Element.</returns>
        public static Element Ext_GetElement(this ElementId elementId, Document doc)
        {
            // Catch invalid elementId or null
            if (elementId.Ext_IsNullOrInvalid() || doc is null)
            {
                return null;
            }

            // Get element
            return doc.GetElement(elementId);
        }

        /// <summary>
        /// Gets an element and returns it as the given type if possible.
        /// </summary>
        /// <typeparam name="T">The type to return as.</typeparam>
        /// <param name="elementId">The ElementId (extended).</param>
        /// <param name="doc">The document to get the element from.</param>
        /// <returns>An Element as T.</returns>
        public static T Ext_GetElement<T>(this ElementId elementId, Document doc)
        {
            // Catch invalid elementId or null
            if (elementId.Ext_IsNullOrInvalid() || doc is null)
            {
                return default(T);
            }

            // Get element
            var element = doc.GetElement(elementId);

            // Try to return as that type
            if (element is T elementAsType)
            {
                return elementAsType;
            }
            else
            {
                return default(T);
            }
        }

        #endregion

        #region To integer value

        /// <summary>
        /// Returns the integer value of an ElementId.
        /// </summary>
        /// <param name="elementId">The ElementId (extended).</param>
        /// <returns>An integer.</returns>
        public static int Ext_AsInteger(this ElementId elementId)
        {
            // Null catch = invalid
            if (elementId is null) { return -1; }

            #if REVIT2024_OR_GREATER
            // Return the Value
            return (int)elementId.Value;

            #else
            // Return the IntegerValue
            return elementId.IntegerValue;

            #endif
        }

        #endregion
    }
}