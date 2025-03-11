// The class belongs to the extensions namespace
// FamilyInstance familyInstance.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to FamilyInstances.
    /// </summary>
    public static class FamilyInstance_Ext
    {
        #region Namekey

        /// <summary>
        /// Constructs a name key based on a Revit FamilySymbol (type).
        /// </summary>
        /// <param name="familyInstance">A Revit FamilyInstance.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToFamilyInstanceKey(this FamilyInstance familyInstance, bool includeId = false)
        {
            // Return its key using the FamilySymbol key
            return familyInstance.Symbol.Ext_ToFamilySymbolKey(includeId, instanceId: familyInstance.Id);
        }

        #endregion
    }
}