// The class belongs to the utility namespace
// using gCnv = geeWiz.Utilities.ConvertUtils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to converting units
    /// </summary>
    public static class ConvertUtils
    {
        /// <summary>
        /// Converts a value to project (from internal).
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="doc">The related document.</param>
        /// <param name="unitType">The ForgeTypeId (use SpecTypeId enum).</param>
        /// <returns>A double.</returns>
        public static double ValueToProject(double value, Document doc, ForgeTypeId unitType = null)
        {
            // Default unit type is length
            if (unitType == null) { unitType = SpecTypeId.Length; }
            
            // Get the unit type Id
            ForgeTypeId unitTypeId = doc.GetUnits().GetFormatOptions(unitType).GetUnitTypeId();

            // Convert the unit to project
            return UnitUtils.ConvertFromInternalUnits(value, unitTypeId);
        }

        /// <summary>
        /// Converts a value to internal (from project).
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="doc">The related document.</param>
        /// <param name="unitType">The ForgeTypeId (use SpecTypeId enum).</param>
        /// <returns>A double.</returns>
        public static double ValueToInternal(double value, Document doc, ForgeTypeId unitType = null)
        {
            // Default unit type is length
            if (unitType == null) { unitType = SpecTypeId.Length; }

            // Get the unit type Id
            ForgeTypeId unitTypeId = doc.GetUnits().GetFormatOptions(unitType).GetUnitTypeId();

            // Convert the unit to internal
            return UnitUtils.ConvertToInternalUnits(value, unitTypeId);
        }
    }
}
