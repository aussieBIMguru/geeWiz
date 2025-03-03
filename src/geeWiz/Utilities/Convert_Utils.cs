// The class belongs to the utility namespace
// using gCnv = geeWiz.Utilities.Convert_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to converting units
    /// </summary>
    public static class Convert_Utils
    {
        #region Constants

        // Mathematical constants
        public static readonly double MATH_PI = Math.PI;
        public static readonly double MATH_E = Math.E;

        #endregion

        #region String => Double

        /// <summary>
        /// Convert a string to a nullable double.
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <param name="valueOnFailure">The value to use if it cannot convert.</param>
        /// <returns>A nullable double.</returns>
        public static Nullable<double> StringToDouble(string text, Nullable<double> valueOnFailure = null)
        {
            // Default double value
            double value = 0.0;

            // If we can convert to a double, return it
            if (double.TryParse(text, out value))
            {
                return value;
            }

            // If we can't, return the value on failure
            return valueOnFailure;
        }

        /// <summary>
        /// Convert a string to a double, with a backup value if it fails.
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <param name="valueOnFailure">The value to use if it cannot convert.</param>
        /// <returns>A double.</returns>
        public static double StringToDouble(string text, double valueOnFailure)
        {
            // Default double value
            double value = 0.0;

            // If we can convert to a double, return it
            if (double.TryParse(text, out value))
            {
                return value;
            }

            // If we can't, return the value on failure
            return valueOnFailure;
        }

        #endregion

        #region String => Integer

        /// <summary>
        /// Convert a string to a nullable integer.
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <param name="valueOnFailure">The value to use if it cannot convert.</param>
        /// <returns>A nullable integer.</returns>
        public static Nullable<int> StringToInt(string text, Nullable<int> valueOnFailure = null)
        {
            // Default int value
            int value = 0;

            // If we can convert to a int, return it
            if (int.TryParse(text, out value))
            {
                return value;
            }

            // If we can't, return the value on failure
            return valueOnFailure;
        }

        /// <summary>
        /// Convert a string to an integer, with a backup value if it fails.
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <param name="valueOnFailure">The value to use if it cannot convert.</param>
        /// <returns>An integer.</returns>
        public static int StringToInt(string text, int valueOnFailure)
        {
            // Default int value
            int value = 0;

            // If we can convert to a int, return it
            if (int.TryParse(text, out value))
            {
                return value;
            }

            // If we can't, return the value on failure
            return valueOnFailure;
        }

#endregion

        #region Degrees <=> Radians

        /// <summary>
        /// Convert a value to degrees from radians.
        /// </summary>
        /// <param name="radians">The value to convert.</param>
        /// <returns>A double.</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * ((double)180 / MATH_PI);
        }

        /// <summary>
        /// Convert a value to radians from degrees.
        /// </summary>
        /// <param name="degrees">The value to convert.</param>
        /// <returns>A double.</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * (MATH_PI / (double)180);
        }

        #endregion

        #region Project <=> Internal

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

        #endregion

        #region ElementId <=> Integer

        // In Revit 2024, deprecations occured for the ElementId class
        // We handle them here using preprocessor directives

        /// <summary>
        /// Creates an ElementId from an integer (in 2024+ needs to be Int64).
        /// </summary>
        /// <param name="integer">The integer value.</param>
        /// <returns>An ElementId.</returns>
        public static ElementId IntToElementId(int integer)
        {
            // ElementId begins as null
            ElementId elementId = null;

            #if REVIT2024_OR_GREATER
            // Try to get ElementId using Int64
            try
            {
                elementId = new ElementId((Int64)integer);
            }
            catch {; }
            #else
            // Try to get ElementId using Int
            try
            {
                return new ElementId(integer);
            }
            catch{; }
            #endif

            // Return the elementId
            return elementId;
        }

        /// <summary>
        /// Returns the integer value of an ElementId (in 2024+ needs to come from Value).
        /// </summary>
        /// <param name="elementId">The ElementId.</param>
        /// <returns>An integer.</returns>
        public static int ElementIdToInt(ElementId elementId)
        {
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