// The class belongs to the extensions namespace
// ForgeTypeId forgeTypeId.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to ForgeTypeIds.
    /// </summary>
    public static class ForgeTypeId_Ext
    {
        /// <summary>
        /// Gets the name for a GroupTypeId.
        /// </summary>
        /// <param name="forgeTypeId">A ForgeTypeId (extended).</param>
        /// <param name="catchOther">Assume null means "Other".</param>
        /// <returns>A string.</returns>
        public static string Ext_GetGroupTypeName(this ForgeTypeId forgeTypeId, bool catchOther = true)
        {
            try
            {
                var name = LabelUtils.GetLabelForGroup(forgeTypeId);
                if (catchOther) { name ??= "Other"; }
                return name;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the name for a SpecTypeId.
        /// </summary>
        /// <param name="forgeTypeId">A ForgeTypeId (extended).</param>
        /// <returns>A string.</returns>
        public static string Ext_GetSpecTypeName(this ForgeTypeId forgeTypeId)
        {
            try
            {
                return LabelUtils.GetLabelForSpec(forgeTypeId);
            }
            catch
            {
                return null;
            }
        }
    }
}
