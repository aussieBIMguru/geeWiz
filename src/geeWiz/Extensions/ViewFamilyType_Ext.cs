// The class belongs to the extensions namespace
// ViewFamilyType viewFamilyType.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to ViewFamilyTypes.
    /// </summary>
    public static class ViewFamilyType_Ext
    {
        #region Namekey

        /// <summary>
        /// Constructs a name key based on a Revit ViewFamilyType.
        /// </summary>
        /// <param name="viewFamilyType">A Revit ViewFamilyType.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToViewFamilyTypeKey(this ViewFamilyType viewFamilyType, bool includeId = false)
        {
            // Null catch
            if (viewFamilyType is null) { return "???"; }

            // Return key with Id
            if (includeId)
            {
                return $"{viewFamilyType.ViewFamily.ToString()}: {viewFamilyType.Name} [{viewFamilyType.Id.ToString()}]";
            }
            // Return key without Id
            else
            {
                return $"{viewFamilyType.ViewFamily.ToString()}: {viewFamilyType.Name}";
            }
        }

        #endregion
    }
}