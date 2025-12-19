// The class belongs to the extensions namespace
// FamilySymbol familySymbol.ExtensionMethod()
using Autodesk.Revit.DB;

namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to FamilySymbols (types).
    /// </summary>
    public static class FamilySymbol_Ext
    {
        #region Namekey

        /// <summary>
        /// Constructs a name key based on a Revit FamilySymbol (type).
        /// </summary>
        /// <param name="familySymbol">A Revit FamilySymbol (type).</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToFamilySymbolKey(this FamilySymbol familySymbol, bool includeId = false)
        {
            // Null catch
            if (familySymbol is null) { return "???"; }

            // Return the name key
            var nameKey = $"{familySymbol.Family.FamilyCategory.Name}: {familySymbol.Family.Name} - {familySymbol.Name}";
            return familySymbol.Ext_FinalizeKey(nameKey, includeId);
        }

        #endregion

        #region Get all instances

        /// <summary>
        /// Returns all instances of the provided family type.
        /// </summary>
        /// <param name="familySymbol">The FamilySymbol (extended).</param>
        /// <returns>A list of FamilyInstances.</returns>
        public static List<FamilyInstance> Ext_GetAllInstances(this FamilySymbol familySymbol)
        {
            // Get family symbol Id
            var familySymbolId = familySymbol.Id;
            
            // Get all instances of same symbool
            return familySymbol.Document.Ext_Collector()
                .OfClass(typeof(FamilyInstance))
                .Cast<FamilyInstance>()
                .Where(f => f.Symbol.Id == familySymbolId)
                .ToList();
        }

        #endregion
    }
}