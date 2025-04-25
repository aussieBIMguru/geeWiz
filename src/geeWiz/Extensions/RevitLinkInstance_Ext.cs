// The class belongs to the extensions namespace
// RevitLinkInstance revitLinkInstance.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to Revit link instances.
    /// </summary>
    public static class RevitLinkInstance_Ext
    {
        /// <summary>
        /// Returns a linked instance's transform.
        /// </summary>
        /// <param name="linkInstance">The linked instance (extended).</param>
        /// <returns>A Transform.</returns>
        public static Transform Ext_GetLinkTransform(this RevitLinkInstance linkInstance, bool invert = false)
        {
            // Null check
            if (linkInstance is null) { return null; }

            // Return the transform
            if (invert)
            {
                return linkInstance.GetTransform().Inverse;
            }
            else
            {
                return linkInstance.GetTransform();
            }
        }

        /// <summary>
        /// Returns a linked instance's type.
        /// </summary>
        /// <param name="linkInstance">The linked instance.</param>
        /// <returns>A RevitLinkType.</returns>
        public static RevitLinkType Ext_GetRevitLinkType(this RevitLinkInstance linkInstance)
        {
            // Null check
            if (linkInstance is null) { return null; }

            // Return the type
            return (linkInstance as Element).Ext_GetElementType<RevitLinkType>();
        }
    }
}
