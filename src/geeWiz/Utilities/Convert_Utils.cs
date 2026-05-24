// Revit API
using Autodesk.Revit.DB;
// geeWiz
using geeWiz.Extensions;
using gCnv = geeWiz.Utilities.Convert_Utils;
using gPar = geeWiz.Utilities.Parameter_Utils;

// The class belongs to the utility namespace
// using gCnv = geeWiz.Utilities.Convert_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// A class to attempt parseing the typical Revit parameter value inputs.
    /// </summary>
    public class DataConverter
    {
        /// <summary>
        /// String representation of value.
        /// </summary>
        public string StringValue { get; set; } = string.Empty;

        /// <summary>
        /// Integer representation of value.
        /// </summary>
        public int IntegerValue { get; set; } = 0;

        /// <summary>
        /// Double representation of value.
        /// </summary>
        public double DoubleValue { get; set; } = 0.0;

        /// <summary>
        /// ValueString representation of value in project units.
        /// </summary>
        public string ProjectStringValue { get; set; } = string.Empty;

        /// <summary>
        /// Integer representation of value in project units.
        /// </summary>
        public int ProjectIntegerValue { get; set; } = 0;

        /// <summary>
        /// Double representation of value in project units.
        /// </summary>
        public double ProjectDoubleValue { get; set; } = 0.0;

        /// <summary>
        /// ElementId representation of value.
        /// </summary>
        public ElementId ElementIdValue { get; set; } = ElementId.InvalidElementId;

        /// <summary>
        /// If the value can be a valid String.
        /// </summary>
        public bool CanBeString { get; set; } = false;

        /// <summary>
        /// If the value can be a valid Integer.
        /// </summary>
        public bool CanBeInt { get; set; } = false;

        /// <summary>
        /// If the value can be a valid Double.
        /// </summary>
        public bool CanBeDouble { get; set; } = false;

        /// <summary>
        /// If the value can be a valid ElementId.
        /// </summary>
        public bool CanBeElementId { get; set; } = false;

        /// <summary>
        /// Related UnitTypeId to the value.
        /// </summary>
        public ForgeTypeId UnitTypeId { get; set; } = null;

        /// <summary>
        /// Related Document to the context of the value.
        /// </summary>
        public Document Document { get; set; } = null;
        
        /// <summary>
        /// Intializes all possible values based on a value that is primarily a string.
        /// </summary>
        /// <param name="str">The provided value.</param>
        /// <param name="parameter">Related Parameter.</param>
        /// <param name="doc">Related Document.</param>
        /// <param name="givenAsProjectValue">If the value provided in Project Units.</param>
        public DataConverter(string str, Parameter parameter = null, Document doc = null, bool givenAsProjectValue = false)
        {
            // Store core values
            doc ??= Globals.CurrentDocument();
            this.Document = doc;

            // Check we have a valid string
            if (str is null) { return; }
            this.CanBeString = true;

            // Try to convert to double, process if we do
            var tryDouble = gCnv.StringToDouble(str);

            if (tryDouble.HasValue)
            {
                this.CanBeDouble = true;
                this.CanBeInt = true;

                // Routine if we need no parameter checking or we have 0
                if (tryDouble == 0 || parameter is null || !parameter.Ext_HasUnitType())
                {
                    SimpleDoubleRoutine(tryDouble.Value);
                }
                // Otherwise check it against the parameter
                else
                {
                    ComplexDoubleRoutine(tryDouble.Value, str, parameter, givenAsProjectValue);
                }  
            }

            // Attempt to get the ElementId
            this.ElementIdValue = str.Ext_StringToElementId(valueOnFailure: ElementId.InvalidElementId);
            this.CanBeElementId = this.ElementIdValue.Ext_IsValid();
        }

        /// <summary>
        /// Intializes all possible values based on a value that is primarily an Integer.
        /// </summary>
        /// <param name="integer">The provided value.</param>
        /// <param name="parameter">Related Parameter.</param>
        /// <param name="doc">Related Document.</param>
        /// <param name="givenAsProjectValue">If the value provided in Project Units.</param>
        public DataConverter(int integer, Parameter parameter = null, Document doc = null, bool givenAsProjectValue = false)
        {
            // Store core values
            doc ??= Globals.CurrentDocument();
            this.Document = doc;

            // Routine if we need no parameter checking or we have 0
            if (integer == 0 || parameter is null || !parameter.Ext_HasUnitType())
            {
                SimpleDoubleRoutine(integer);
            }
            // Otherwise check it against the parameter
            else
            {
                ComplexDoubleRoutine(integer, integer.ToString(), parameter, givenAsProjectValue);
            }

            this.ElementIdValue = gCnv.IntToElementId(integer, ElementId.InvalidElementId);
            this.CanBeElementId = this.ElementIdValue.Ext_IsValid();
        }

        /// <summary>
        /// Intializes all possible values based on a value that is primarily a Double.
        /// </summary>
        /// <param name="dbl">The provided value.</param>
        /// <param name="parameter">Related Parameter.</param>
        /// <param name="doc">Related Document.</param>
        /// <param name="givenAsProjectValue">If the value provided in Project Units.</param>
        public DataConverter(double dbl, Parameter parameter = null, Document doc = null, bool givenAsProjectValue = false)
        {
            // Store core values
            doc ??= Globals.CurrentDocument();
            this.Document = doc;

            // Routine if we need no parameter checking or we have 0
            if (dbl == 0.0 || parameter is null || !parameter.Ext_HasUnitType())
            {
                SimpleDoubleRoutine(dbl);
            }
            // Otherwise check it against the parameter
            else
            {
                ComplexDoubleRoutine(dbl, dbl.ToString(), parameter, givenAsProjectValue);
            }

            this.ElementIdValue = gCnv.IntToElementId(RoundToInt(dbl), ElementId.InvalidElementId);
            this.CanBeElementId = this.ElementIdValue.Ext_IsValid();
        }

        /// <summary>
        /// Intializes all possible values based on a value that is primarily an ElementId.
        /// </summary>
        /// <param name="elementId">The provided value.</param>
        /// <param name="parameter">Related Parameter.</param>
        public DataConverter(ElementId elementId, Parameter parameter = null)
        {
            this.UnitTypeId = parameter?.GetUnitTypeId();
            elementId ??= ElementId.InvalidElementId;
            
            this.CanBeString = true;
            this.StringValue = elementId.ToString();

            this.IntegerValue = elementId.Ext_AsInteger();
            this.CanBeInt = true;

            this.DoubleValue = (double)this.IntegerValue;
            this.CanBeDouble = true;

            this.ElementIdValue = elementId;
            this.CanBeElementId = elementId.Ext_IsValid();
        }

        /// <summary>
        /// A routine to process a Double value that considers project units.
        /// </summary>
        /// <param name="value">The provided value.</param>
        /// <param name="str">Related ValueString value.</param>
        /// <param name="parameter">Related Parameter.</param>
        /// <param name="givenAsProjectValue">If the value provided in Project Units.</param>
        public void ComplexDoubleRoutine(double value, string str, Parameter parameter, bool givenAsProjectValue)
        {
            // Get the spec type of the parameter
            var specTypeId = parameter.GetUnitTypeId();
            this.UnitTypeId = specTypeId;

            // If we provided in project units...
            if (givenAsProjectValue)
            {
                // Set project values as they are
                this.ProjectDoubleValue = value;
                this.ProjectIntegerValue = RoundToInt(value);
                this.ProjectStringValue = str;

                // Convert the remainder to internal
                this.DoubleValue = gCnv.ValueToInternal(value, this.Document, this.UnitTypeId);
                this.IntegerValue = RoundToInt(this.DoubleValue);
                this.StringValue = FormatDoubleAsString(this.DoubleValue);
            }
            // Otherwise handle it as internal units
            else
            {
                // Set internal values as they are
                this.DoubleValue = value;
                this.IntegerValue = RoundToInt(value);
                this.StringValue = str;

                // Convert the remainder
                this.ProjectDoubleValue = gCnv.ValueToProject(value, this.Document, this.UnitTypeId);
                this.ProjectIntegerValue = RoundToInt(this.ProjectDoubleValue);
                this.ProjectStringValue = FormatDoubleAsString(this.ProjectDoubleValue);
            }
        }

        /// <summary>
        /// A routine to process a Double value, assumed as project units.
        /// </summary>
        /// <param name="value">The provided value.</param>
        private void SimpleDoubleRoutine(double value)
        {
            this.DoubleValue = this.ProjectDoubleValue = value;
            this.IntegerValue = this.ProjectIntegerValue = RoundToInt(value);
            this.StringValue = this.ProjectStringValue = FormatDoubleAsString(value);
        }

        /// <summary>
        /// Rounds a double to the nearest integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An Integer.</returns>
        private int RoundToInt(double value)
        {
            return (int)Math.Round(value, 0);
        }

        /// <summary>
        /// Processes a double into a string, with trailing zeroes removed.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A String.</returns>
        private string FormatDoubleAsString(double value)
        {
            int intVal = (int)Math.Round(value);
            return Math.Abs(value - intVal) < 1e-6 ? intVal.ToString() : value.ToString();
        }
    }

    /// <summary>
    /// Static methods container related to Conversion.
    /// </summary>
    public static class Convert_Utils
    {
        #region Constants

        /// <summary>
        /// The Pi constant.
        /// </summary>
        public const double MATH_PI = Math.PI;

        /// <summary>
        /// The E constant.
        /// </summary>
        public const double MATH_E = Math.E;

        #endregion

        #region String => Double

        /// <summary>
        /// Convert a string to a nullable double.
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <param name="valueOnFailure">The value to use if it cannot convert.</param>
        /// <returns>A nullable Double.</returns>
        public static double? StringToDouble(string text, double? valueOnFailure = null)
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
        /// Convert a string to a double, with a double fallback value.
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <param name="valueOnFailure">The value to use if it cannot convert.</param>
        /// <returns>A Double.</returns>
        public static double StringToDouble(string text, double valueOnFailure)
        {
            // Double value
            double value;

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
        /// Convert a string to a nullable Integer.
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <param name="valueOnFailure">The value to use if it cannot convert.</param>
        /// <returns>A nullable Integer.</returns>
        public static int? StringToInt(string text, int? valueOnFailure = null)
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
        /// Convert a string to an integer, with a fallback Integer value.
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <param name="valueOnFailure">The value to use if it cannot convert.</param>
        /// <returns>An integer.</returns>
        public static int StringToInt(string text, int valueOnFailure)
        {
            // Default int value
            int value;

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
        /// <returns>A Double.</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * ((double)180 / MATH_PI);
        }

        /// <summary>
        /// Convert a value to radians from degrees.
        /// </summary>
        /// <param name="degrees">The value to convert.</param>
        /// <returns>A Double.</returns>
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
        /// <returns>A Double.</returns>
        public static double ValueToProject(double value, Document doc, ForgeTypeId unitType = null)
        {
            // Default unit type is length
            if (unitType == null) { unitType = SpecTypeId.Length; }
            
            // Get the unit type Id
            var unitTypeId = doc.GetUnits().GetFormatOptions(unitType).GetUnitTypeId();

            // Convert the unit to project
            return UnitUtils.ConvertFromInternalUnits(value, unitTypeId);
        }

        /// <summary>
        /// Converts a value to internal (from project).
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="doc">The related document.</param>
        /// <param name="unitType">The ForgeTypeId (use SpecTypeId enum).</param>
        /// <returns>A Double.</returns>
        public static double ValueToInternal(double value, Document doc, ForgeTypeId unitType = null)
        {
            // Default unit type is length
            if (unitType == null) { unitType = SpecTypeId.Length; }

            // Get the unit type Id
            var unitTypeId = doc.GetUnits().GetFormatOptions(unitType).GetUnitTypeId();

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
        /// <param name="valueOnFailure">The fallback value on failure.</param>
        /// <returns>An ElementId.</returns>
        public static ElementId IntToElementId(int integer, ElementId valueOnFailure = null)
        {
#if REVIT2020 || REVIT2021 || REVIT2022 || REVIT2023
            // Try to get ElementId using Int
            try
            {
                return new ElementId(integer);
            }
            catch{; }
#else
            try
            {
                return new ElementId((Int64)integer);
            }
            catch {; }
#endif

            // Return the elementId
            return valueOnFailure;
        }

        /// <summary>
        /// Returns the integer value of an ElementId.
        /// </summary>
        /// <param name="elementId">The ElementId.</param>
        /// <returns>An integer.</returns>
        public static int ElementIdToInt(ElementId elementId)
        {
#if REVIT2020 || REVIT2021 || REVIT2022 || REVIT2023
            // Return the IntegerValue
            return elementId.IntegerValue;
#else
            // Return the Value
            return (int)elementId.Value;
#endif
        }

        #endregion

        #region ForgeTypeIds by name

        /// <summary>
        /// Gets a GroupType with a given name ("Other" = null).
        /// </summary>
        /// <param name="name">The name to get.</param>
        /// <param name="forgeTypeIds">An optional list of ForgeTypeIds.</param>
        /// <returns>A ForgeTypeId.</returns>
        public static ForgeTypeId GroupTypeByName(string name, List<ForgeTypeId> forgeTypeIds = null)
        {
            // Catch other
            if (name == "Other" || name is null) { return null; }

            // Get ForgeTypeIds if needed
            forgeTypeIds ??= gPar.GetGroupTypeIds();

            // For each forgetypeId...
            foreach (ForgeTypeId forgeTypeId in forgeTypeIds)
            {
                // Given it might be a non group type Id, try
                try
                {
                    // If the name matches, return it
                    if (LabelUtils.GetLabelForGroup(forgeTypeId) == name)
                    {
                        return forgeTypeId;
                    }
                }
                catch
                {
                    ;
                }
            }

            // Return null (technically, "Other" group)
            return null;
        }

        /// <summary>
        /// Gets a SpecType with a given name.
        /// </summary>
        /// <param name="name">The name to get.</param>
        /// <param name="forgeTypeIds">An optional list of SpecTypeIds.</param>
        /// <returns>A ForgeTypeId.</returns>
        public static ForgeTypeId SpecTypeByName(string name, List<ForgeTypeId> forgeTypeIds = null)
        {
            // Catch null
            if (name is null) { return null; }

            // Get SpecTypeIds if needed
            forgeTypeIds ??= gPar.GetSpecTypeIds();

            // For each forgetypeId...
            foreach (ForgeTypeId forgeTypeId in forgeTypeIds)
            {
                // Given it might be a non group type Id, try
                try
                {
                    // If the name matches, return it
                    if (LabelUtils.GetLabelForSpec(forgeTypeId) == name)
                    {
                        return forgeTypeId;
                    }
                }
                catch
                {
                    ;
                }
            }

            // Return invalid
            return null;
        }

        #endregion

        #region Coordinates

        /// <summary>
        /// Returns the transform to get from internal to actual coordinates.
        /// </summary>
        /// <param name="doc">The related document.</param>
        /// <returns>A transform object.</returns>
        public static Transform InternalToActualTransform(Document doc)
        {
            return doc.ActiveProjectLocation.GetTotalTransform();
        }

        /// <summary>
        /// Returns the transform to get from actual to internal coordinates.
        /// </summary>
        /// <param name="doc">The related document.</param>
        /// <returns>A transform object.</returns>
        public static Transform ActualToInternalTransform(Document doc)
        {
            return doc.ActiveProjectLocation.GetTotalTransform().Inverse;
        }

        #endregion
    }
}