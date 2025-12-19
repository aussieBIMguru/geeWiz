// The class belongs to the extensions namespace
// FamilyInstance familyInstance.ExtensionMethod()
using Autodesk.Revit.DB;

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
            // Return the name key
            var nameKey = familyInstance.Symbol.Ext_ToFamilySymbolKey();
            return familyInstance.Ext_FinalizeKey(nameKey, includeId);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the family of a family instance.
        /// </summary>
        /// <param name="familyInstance">The familyinstance (extended).</param>
        /// <returns>A Family.</returns>
        public static Family Ext_GetFamily(this FamilyInstance familyInstance)
        {
            if (familyInstance is null) { return null; }
            return familyInstance.Symbol.Family;
        }

        /// <summary>
        /// Returns if a family instance is modelled in place.
        /// </summary>
        /// <param name="familyInstance">The familyinstance (extended).</param>
        /// <returns>A Boolean.</returns>
        public static bool Ext_IsInPlace(this FamilyInstance familyInstance)
        {
            if (familyInstance is null) { return false; }
            return familyInstance.Symbol.Family.IsInPlace;
        }

        /// <summary>
        /// Returns if a family instance is nested.
        /// </summary>
        /// <param name="familyInstance">The familyinstance (extended).</param>
        /// <returns>A Boolean.</returns>
        public static bool Ext_IsNested(this FamilyInstance familyInstance)
        {
            // Null check
            if (familyInstance is null) { return false; }

            // Return if it has a super component
            return familyInstance.SuperComponent is not null;
        }

        /// <summary>
        /// Returns the super componet of a family instance, if any.
        /// </summary>
        /// <param name="familyInstance">The familyinstance (extended).</param>
        /// <returns>A FamilyInstance.</returns>
        public static FamilyInstance Ext_GetSuperComponent(this FamilyInstance familyInstance)
        {
            // Null check
            if (familyInstance is null) { return null; }

            // If super component is a family instance, return it
            if (familyInstance.SuperComponent is FamilyInstance superComponent)
            {
                return superComponent;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}