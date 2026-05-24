// Revit API
using Autodesk.Revit.DB;

namespace geeWiz.Extensions
{
    /// <summary>
    /// Extension methods to the Parameter class.
    /// </summary>
    public static class Parameter_Ext
    {
        /// <summary>
        /// Check if a parameter has a UnitTypeId.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <returns>A Boolean.</returns>
        public static bool Ext_HasUnitType(this Parameter parameter)
        {
            if (parameter == null) { return false; }

            ForgeTypeId spec = parameter.GetUnitTypeId();

            return parameter.StorageType == StorageType.Double
                && spec is not null
                && spec.IsValidObject;
        }
    }
}
