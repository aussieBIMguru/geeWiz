// Revit API
using Autodesk.Revit.UI;

// The class belongs to the utilities namespace
// gPar = geeWiz.Utilities.Parameter_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to string based operations.
    /// </summary>
    public static class Parameter_Utils
    {
        /// <summary>
        /// Try to set a parameter using a ParameterHelper object.
        /// </summary>
        /// <param name="helper">A ParameterHelper object.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_SetParameterValueByName(ParameterHelper helper)
        {
            // Check if we should not proceed
            if (helper.Parameter is null || helper.Element is null)
            {
                return Result.Failed;
            }

            // Set depending on stored value
            if (helper.StorageType == StorageType.String)
            {
                return ParameterHelper.SetString(helper);
            }
            else if (helper.StorageType == StorageType.Integer)
            {
                return ParameterHelper.SetInteger(helper);
            }
            else if (helper.StorageType == StorageType.Double)
            {
                return ParameterHelper.SetDouble(helper);
            }
            else if (helper.StorageType == StorageType.ElementId)
            {
                return ParameterHelper.SetElementId(helper);
            }

            // Otherwise return failed result
            return Result.Failed;
        }
    }

    #region ParameterHelper class

    /// <summary>
    /// A class to assess parameter values in various representations.
    /// </summary>
    public class ParameterHelper
    {
        #region Class Properties

        // Representations of the parameter
        public Element Element { get; set; }
        public Parameter Parameter { get; set; }
        public StorageType StorageType { get; set; }
        public int AsInteger { get; set; }
        public double AsDouble { get; set; }
        public string AsString { get; set; }
        public ElementId AsElementId { get; set; }

        #endregion

        #region Constructor - Get Value

        // Constructor for get value
        public ParameterHelper(Element element, string parameterName)
        {
            // Store the element
            this.Element = element;

            // Store the parameter
            Parameter parameter = element.LookupParameter(parameterName);
            this.Parameter = parameter;

            // Default values to return (assume nothing found)
            this.AsString = null;
            this.AsInteger = 0;
            this.AsDouble = 0.0;
            this.AsElementId = ElementId.InvalidElementId;
            this.StorageType = StorageType.None;

            // Return default value if parameter is none
            if (parameter is null) { return; }
            this.StorageType = parameter.StorageType;

            // Work through the various storage types, storing what we can
            if (this.StorageType == StorageType.String)
            {
                if (parameter.AsString() is string value)
                {
                    this.AsString = value;
                }
            }
            else if (this.StorageType == StorageType.Integer)
            {
                if (parameter.AsInteger() is int value)
                {
                    this.AsInteger = value;
                    this.AsDouble = (double)value;
                    this.AsString = value.ToString();
                }
            }
            else if (this.StorageType == StorageType.Double)
            {
                if (parameter.AsDouble() is double value)
                {
                    this.AsDouble = value;
                    this.AsInteger = (int)value;
                    this.AsString = value.ToString();
                }
            }
            else if (this.StorageType == StorageType.ElementId)
            {
                if (parameter.AsElementId() is ElementId value)
                {
                    this.AsElementId = value;
                    this.AsInteger = value.IntegerValue;
                    this.AsString = value.ToString();
                    this.AsDouble = (double)value.IntegerValue;

                }
            }
        }

        #endregion

        #region Constructor - Set String value

        // Constructor to set as string
        public ParameterHelper(Element element, string parameterName, string value)
        {
            // Store the element
            this.Element = element;

            // Store the parameter
            Parameter parameter = element.LookupParameter(parameterName);
            this.Parameter = parameter;

            // Store the type and value
            this.StorageType = StorageType.String;
            this.AsString = value;
        }

        // Method to set string value
        public static Result SetString(ParameterHelper helper)
        {
            try
            {
                helper.Parameter.Set(helper.AsString);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        #endregion

        #region Constructor - Set Integer value

        // Constructor to set as integer
        public ParameterHelper(Element element, string parameterName, int value)
        {
            // Store the element
            this.Element = element;

            // Store the parameter
            Parameter parameter = element.LookupParameter(parameterName);
            this.Parameter = parameter;

            // Store the type and value
            this.StorageType = StorageType.Integer;
            this.AsInteger = value;
        }

        // Method to set integer value
        public static Result SetInteger(ParameterHelper helper)
        {
            try
            {
                helper.Parameter.Set(helper.AsInteger);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        #endregion

        #region Constructor - Set Double value

        // Constructor to set as double
        public ParameterHelper(Element element, string parameterName, double value)
        {
            // Store the element
            this.Element = element;

            // Store the parameter
            Parameter parameter = element.LookupParameter(parameterName);
            this.Parameter = parameter;

            // Store the type and value
            this.StorageType = StorageType.Double;
            this.AsDouble = value;
        }

        // Method to set double value
        public static Result SetDouble(ParameterHelper helper)
        {
            try
            {
                helper.Parameter.Set(helper.AsDouble);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        #endregion

        #region Constructor - Set ElementId value

        // Constructor to set as element Id
        public ParameterHelper(Element element, string parameterName, ElementId value)
        {
            // Store the element
            this.Element = element;

            // Store the parameter
            Parameter parameter = element.LookupParameter(parameterName);
            this.Parameter = parameter;

            // Store the type and value
            this.StorageType = StorageType.ElementId;
            this.AsElementId = value;
        }

        // Method to set ElementId
        public static Result SetElementId(ParameterHelper helper)
        {
            try
            {
                helper.Parameter.Set(helper.AsElementId);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        #endregion
    }

    #endregion
}