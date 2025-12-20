// Revit API
using Autodesk.Revit.UI;

// The class belongs to the utilities namespace
// gPar = geeWiz.Utilities.Parameter_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// A class to get/set parameter values.
    /// </summary>
    public class ParameterHelper
    {
        #region Class Properties

        public Element Element { get; set; }
        public Parameter Parameter { get; set; }
        public StorageType StorageType { get; set; }
        public ForgeTypeId UnitTypeId { get; set; }
        public DataConverter ParameterValue { get; set; }
        public DataConverter StoredValue { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a ParameterHelper object.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parameterName"></param>
        public ParameterHelper(Element element, string parameterName)
        {
            // Store the element
            this.Element = element;
            var doc = Element.Document;

            // Store the parameter
            var parameter = element.LookupParameter(parameterName);
            this.Parameter = parameter;

            // Default values to return (assume nothing found)
            this.StorageType = StorageType.None;
            this.UnitTypeId = null;

            // Return default value if parameter is none
            if (parameter is null) { return; }
            this.StorageType = parameter.StorageType;
            this.UnitTypeId = parameter.GetUnitTypeId();

            // Work through the various storage types, storing what we can
            switch (this.StorageType)
            {
                case StorageType.String: this.ParameterValue = new DataConverter(parameter.AsString(), parameter, doc, givenAsProjectValue: false); break;
                case StorageType.Integer: this.ParameterValue = new DataConverter(parameter.AsInteger(), parameter, doc, givenAsProjectValue: false); break;
                case StorageType.Double: this.ParameterValue = new DataConverter(parameter.AsDouble(), parameter, doc, givenAsProjectValue: false); break;
                case StorageType.ElementId: this.ParameterValue = new DataConverter(parameter.AsElementId(), parameter); break;
                default: this.ParameterValue = null; break;
            }
        }

        #endregion

        #region Get parameter value

        /// <summary>
        /// Checks if the parameter stores a valid representation for the given type.
        /// </summary>
        /// <typeparam name="T">The type of format to check for.</typeparam>
        /// <returns>A boolean.</returns>
        public bool HasValueAs<T>()
        {
            if (typeof(T) == typeof(string))
            {
                return this.ParameterValue.CanBeString;
            }

            if (typeof(T) == typeof(int))
            {
                return this.ParameterValue.CanBeInt;
            }

            if (typeof(T) == typeof(double))
            {
                return this.ParameterValue.CanBeDouble;
            }

            if (typeof(T) == typeof(ElementId))
            {
                return this.ParameterValue.CanBeElementId;
            }

            return false;
        }

        /// <summary>
        /// Gets the internally stored parameter value.
        /// </summary>
        /// <typeparam name="T">The type of format to return.</typeparam>
        /// <returns>A value.</returns>
        public T GetInternalValueAs<T>()
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)this.ParameterValue.StringValue;
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(object)this.ParameterValue.IntegerValue;
            }
                
            if (typeof(T) == typeof(double))
            {
                return (T)(object)this.ParameterValue.DoubleValue;
            }
                
            if (typeof(T) == typeof(ElementId))
            {
                return (T)(object)this.ParameterValue.ElementIdValue;
            } 

            return default;
        }

        /// <summary>
        /// Gets the project stored parameter value.
        /// </summary>
        /// <typeparam name="T">The type of format to return.</typeparam>
        /// <returns>A value.</returns>
        public T GetProjectValueAs<T>()
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)this.ParameterValue.ProjectStringValue;
            }

            if (typeof(T) == typeof(int))
            {
                return (T)(object)this.ParameterValue.ProjectIntegerValue;
            }

            if (typeof(T) == typeof(double))
            {
                return (T)(object)this.ParameterValue.ProjectDoubleValue;
            }

            if (typeof(T) == typeof(ElementId))
            {
                return (T)(object)this.ParameterValue.ElementIdValue;
            }

            return default;
        }

        #endregion

        #region Store new values

        /// <summary>
        /// Stores a value to the parameter helper.
        /// </summary>
        /// <param name="value">The value to store.</param>
        /// <param name="givenAsProjectValue">Is it provided in project values vs internal.</param>
        public void Store(string value, bool givenAsProjectValue = true)
        {
            this.StoredValue = new DataConverter(value, this.Parameter, this.Element.Document, givenAsProjectValue: givenAsProjectValue);
        }

        /// <summary>
        /// Stores a value to the parameter helper.
        /// </summary>
        /// <param name="value">The value to store.</param>
        /// <param name="givenAsProjectValue">Is it provided in project values vs internal.</param>
        public void Store(int value, bool givenAsProjectValue = true)
        {
            this.StoredValue = new DataConverter(value, this.Parameter, this.Element.Document, givenAsProjectValue: givenAsProjectValue);
        }

        /// <summary>
        /// Stores a value to the parameter helper.
        /// </summary>
        /// <param name="value">The value to store.</param>
        /// <param name="givenAsProjectValue">Is it provided in project values vs internal.</param>
        public void Store(double value, bool givenAsProjectValue = true)
        {
            this.StoredValue = new DataConverter(value, this.Parameter, this.Element.Document, givenAsProjectValue: givenAsProjectValue);
        }

        /// <summary>
        /// Stores a value to the parameter helper.
        /// </summary>
        /// <param name="value">The value to store.</param>
        /// <param name="givenAsProjectValue">Is it provided in project values vs internal.</param>
        public void Store(ElementId value, bool givenAsProjectValue = true)
        {
            this.StoredValue = new DataConverter(value, this.Parameter);
        }

        #endregion

        #region Set new values

        /// <summary>
        /// Verifies if the stored value is different from the current one.
        /// </summary>
        /// <returns></returns>
        public bool ValuesAreTheSame()
        {
            // Null check
            if (this.StoredValue is null || this.ParameterValue is null)
            {
                return false;
            }

            // Check each data type and the values they contain
            switch (this.StorageType)
            {
                case StorageType.String: return this.ParameterValue.StringValue == this.StoredValue.StringValue;
                case StorageType.Integer: return this.ParameterValue.IntegerValue == this.StoredValue.IntegerValue;
                case StorageType.Double: return Math.Abs(this.ParameterValue.DoubleValue - this.StoredValue.DoubleValue) < 1e-6;
                case StorageType.ElementId: return this.ParameterValue.ElementIdValue == this.StoredValue.ElementIdValue;
                default: return false;
            }
        }

        /// <summary>
        /// Attempts to set the parameter value to the stored value.
        /// </summary>
        /// <returns></returns>
        public Result Set()
        {
            try
            {
                // Try to set the value based on the parameter storage type
                switch (this.StorageType)
                {
                    case StorageType.String: this.Parameter.Set(this.StoredValue.StringValue); break;
                    case StorageType.Integer: this.Parameter.Set(this.StoredValue.IntegerValue); break;
                    case StorageType.Double: this.Parameter.Set(this.StoredValue.DoubleValue); break;
                    case StorageType.ElementId: this.Parameter.Set(this.StoredValue.ElementIdValue); break;
                    default: return Result.Cancelled;
                }

                return Result.Succeeded;
            }
            catch
            {
                return Result.Cancelled;
            }
        }

        #endregion
    }

    /// <summary>
    /// Methods of this class generally relate to string based operations.
    /// </summary>
    public static class Parameter_Utils
    {
        #region Get Group/Spec types

        /// <summary>
        /// Gets all GroupTypeIds.
        /// </summary>
        /// <param name="sorted">Sort the grouptypes by name.</param>
        /// <returns>A list of ForgeTypeIds.</returns>
        public static List<ForgeTypeId> GetGroupTypeIds(bool sorted = false)
        {
            // Get all groupTypes
            var groupTypeIds = ParameterUtils
                .GetAllBuiltInGroups()
                .ToList();

            // Return sorted
            if (sorted)
            {
                return groupTypeIds
                    .OrderBy(g => LabelUtils.GetLabelForGroup(g))
                    .ToList();
            }
            else
            {
                return groupTypeIds;
            }
        }

        /// <summary>
        /// Gets all SpecTypeIds.
        /// </summary>
        /// <param name="sorted">Sort the spectypes by name.</param>
        /// <returns>A list of ForgeTypeIds.</returns>
        public static List<ForgeTypeId> GetSpecTypeIds(bool sorted = false)
        {
            // Get all spectypes
            var specTypeIds = SpecUtils.GetAllSpecs().ToList();

            // Return sorted
            if (sorted)
            {
                return specTypeIds
                    .OrderBy(s => LabelUtils.GetLabelForSpec(s))
                    .ToList();
            }
            else
            {
                return specTypeIds;
            }
        }

        #endregion
    }
}