// Revit API
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;
// geeWiz
using ParameterHelper = geeWiz.Utilities.ParameterHelper;


// The class belongs to the extensions namespace
// Element element.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to elements.
    /// </summary>
    public static class Element_Ext
    {
        #region Element name keys

        /// <summary>
        /// Constructs a name key, checking for common inheritances.
        /// </summary>
        /// <param name="element">A Revit Element.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToInheritedKey(this Element element, bool includeId)
        {
            // Catch null element
            if (element is null) { return "???"; }

            // Check typical inheritances in order
            if (element is ViewSheet sheet) { return sheet.Ext_ToSheetKey(includeId); }
            if (element is ViewFamilyType viewFamilyType) { return viewFamilyType.Ext_ToViewFamilyTypeKey(includeId); }
            if (element is View view) { return view.Ext_ToViewKey(includeId); }
            if (element is Family family) { return family.Ext_ToFamilyKey(includeId); }
            if (element is FamilySymbol familySymbol) { return familySymbol.Ext_ToFamilySymbolKey(includeId); }
            if (element is FamilyInstance familyInstance) { return familyInstance.Ext_ToFamilyInstanceKey(includeId); }

            // Return the element key if it did not inherit before
            return element.Ext_ToElementKey(includeId);
        }

        /// <summary>
        /// Constructs a name key for the given Element.
        /// </summary>
        /// <param name="element">A Revit Element.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToElementKey(this Element element, bool includeId)
        {
            // Catch null element
            if (element is null) { return "???"; }

            // Default state of values
            string categoryName = "???";
            string typeName = "???";
            string elementName = "???";

            // Get category name
            if (element.Category is Category category)
            {
                categoryName = category.Name;
            }

            // Get element type name
            try
            {
                typeName = element.GetType().Name;
            }
            catch {; }

            // Get element name
            try
            {
                elementName = element.Name;
            }
            catch {; }

            // Return key with Id
            if (includeId)
            {
                return $"{categoryName}: {typeName} - {elementName} [{element.Id.ToString()}]";
            }
            // Return key without Id
            else
            {
                return $"{categoryName}: {typeName} - {elementName}";
            }
        }

        #endregion

        #region Editability check

        /// <summary>
        /// Returns if an element is editable.
        /// </summary>
        /// <param name="element">The Element to check (extended).</param>
        /// <param name="doc">The Revit document (optional).</param>
        /// <returns>A boolean.</returns>
        public static bool Ext_IsEditable(this Element element, Document doc = null)
        {
            // Catch null element
            if (element is null) { return false; }
            
            // Get document if not provided
            doc ??= element.Document;

            // If document is not workshare, element is editable naturally
            if (!doc.IsWorkshared) { return true; }

            // Get the checkout and model updates status
            var checkoutStatus = WorksharingUtils.GetCheckoutStatus(doc, element.Id);
            var updatesStatus = WorksharingUtils.GetModelUpdatesStatus(doc, element.Id);

            // Check if owned by another user
            if (checkoutStatus == CheckoutStatus.OwnedByOtherUser) { return false; }

            // Check if it is already owned by us
            else if (checkoutStatus == CheckoutStatus.OwnedByCurrentUser) { return true; }

            // Finally, ensure element is current with central
            else { return updatesStatus == ModelUpdatesStatus.CurrentWithCentral; }
        }

        #endregion

        #region Element type

        /// <summary>
        /// Returns the type of an element.
        /// </summary>
        /// <param name="element">An Element (extended).</param>
        /// <returns>An Element.</returns>
        public static Element Ext_GetElementType(this Element element)
        {
            // Null check
            if (element is null) { return null; }

            // Return the element's type
            return element.Document.GetElement(element.GetTypeId());
        }

        /// <summary>
        /// Returns the type of an element.
        /// </summary>
        /// <typeparam name="T">The type to get the element type as.</typeparam>
        /// <param name="element">An Element (extended).</param>
        /// <returns>An Element as T.</returns>
        public static T Ext_GetElementType<T>(this Element element)
        {
            // Null check
            if (element is null) { return default(T); }

            // Get the element's type
            var elementType = element.Document.GetElement(element.GetTypeId());

            // If the element is of the type...
            if (elementType is T elementAsType)
            {
                // Return it
                return elementAsType;
            }
            else
            {
                // Otherwise, return default of T
                return default(T);
            }
        }

        /// <summary>
        /// Returns the type of an element.
        /// </summary>
        /// <param name="element">An Element (extended).</param>
        /// <returns>An Element.</returns>
        public static bool Ext_IsElementType(this Element element)
        {
            // Null check
            if (element is null) { return false; }

            // Return if it is an Element type
            return element is ElementType;
        }

        /// <summary>
        /// Returns all elements of type.
        /// </summary>
        /// <param name="element">An Element (extended).</param>
        /// <returns>A list of Elements.</returns>
        public static List<Element> Ext_AllElementsOfType(this Element element)
        {
            // Null check
            if (element is null) { return new List<Element>(); }

            // Get element type Id
            var elementTypeId = element.Id;

            // Return all elements of type
            return element.Document.Ext_Collector()
                .WhereElementIsNotElementType()
                .Where(e => e.GetTypeId() == elementTypeId)
                .ToList();
        }

        #endregion

        #region Element parameters

        /// <summary>
        /// Retrieve a parameter from an element via a builtin parameter.
        /// </summary>
        /// <param name="element">The Element (extended).</param>
        /// <param name="builtInParameter">A Revit BuiltInParameter.</param>
        /// <returns>A Parameter object.</returns>
        public static Parameter Ext_GetBuiltInParameter(this Element element, BuiltInParameter builtInParameter)
        {
            // Null check
            if (element is null) { return null; }
            
            // Get the parameter by its forgetypeid
            var forgeTypeId = ParameterUtils.GetParameterTypeId(builtInParameter);
            return element.GetParameter(forgeTypeId);
        }

        /// <summary>
        /// Gets the value of parameter by name, given a specified storage type.
        /// </summary>
        /// <typeparam name="T">The parameter storage type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="parameterName">The parameter name to get the value of.</param>
        /// <returns>The value of the parameter.</returns>
        public static T Ext_GetParameterValue<T>(this Element element, string parameterName)
        {
            // Default value for type if no element
            if (element is null) { return default; }

            // Get parameter, default value for type if no parameter
            var parameter = element.LookupParameter(parameterName);
            if (parameter is null) { return default; }

            // Return the value based on storage type
            return parameter.StorageType switch
            {
                StorageType.String => (T)(object)parameter.AsString(),
                StorageType.Integer => (T)(object)parameter.AsInteger(),
                StorageType.Double => (T)(object)parameter.AsDouble(),
                StorageType.ElementId => (T)(object)parameter.AsElementId(),
                _ => default
            };
        }

        /// <summary>
        /// Gets the value of type parameter by name, given a specified storage type.
        /// </summary>
        /// <typeparam name="T">The parameter storage type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="parameterName">The parameter name to get the value of.</param>
        /// <returns>The value of the parameter.</returns>
        public static T Ext_GetTypeParameterValue<T>(this Element element, string parameterName)
        {
            // Default value for type if no element
            if (element is null) { return default; }

            // Get element type
            var elementType = element.Document.GetElement(element.GetTypeId());

            // Return type parameter value
            return elementType.Ext_GetParameterValue<T>(parameterName);
        }

        /// <summary>
        /// Gets the value of a type or instance parameter by name, given a specified storage type.
        /// </summary>
        /// <typeparam name="T">The parameter storage type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="parameterName">The parameter name to get the value of.</param>
        /// <returns>The value of the parameter.</returns>
        public static T Ext_GetTypeOrInstanceParameterValue<T>(this Element element, string parameterName)
        {
            // Default value for type if no element
            if (element is null) { return default; }

            // If instance has the parameter...
            if (element.LookupParameter(parameterName) is Parameter)
            {
                // Return it for the element
                return element.Ext_GetParameterValue<T>(parameterName);
            }
            else
            {
                // Otherwise, return it for the type
                return element.Ext_GetTypeParameterValue<T>(parameterName);
            }
        }

        /// <summary>
        /// Constructs a parameter helper object to get parameter values.
        /// </summary>
        /// <param name="element">A Revit Element (extended).</param>
        /// <param name="parameterName">The parameter name to get.</param>
        /// <returns>A ParameterHelper object.</returns>
        public static ParameterHelper Ext_GetParameterHelper(this Element element, string parameterName)
        {
            return new ParameterHelper(element, parameterName);
        }

        /// <summary>
        /// Gets the value of a parameter as text (even if not a text parameter).
        /// </summary>
        /// <param name="element">A Revit Element (extended).</param>
        /// <param name="parameterName">The parameter name to get.</param>
        /// <returns>A string.</returns>
        public static string Ext_GetParameterText(this Element element, string parameterName)
        {
            // Early exit on null
            if (element is null) { return null; }

            // Try to get parameter
            var parameter = element.LookupParameter(parameterName);
            if (parameter is null) { return null; }

            // Return the value based on storage type
            return parameter.StorageType switch
            {
                StorageType.String => parameter.AsString(),
                StorageType.Integer => parameter.AsValueString(),
                StorageType.Double => parameter.AsValueString(),
                StorageType.ElementId => parameter.AsElementId().ToString(),
                _ => null
            };
        }

        /// <summary>
        /// Sets the value of a text based parameter, if able.
        /// </summary>
        /// <param name="element">A Revit Element (extended).</param>
        /// <param name="parameterName">The parameter name to set.</param>
        /// <param name="value">The string value to set.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_SetParameterValue(this Element element, string parameterName, string value)
        {
            // Early exit on no element or value
            if (element is null || value is null) { return Result.Failed; }
            
            // Try to get parameter, cancel if we can not set it
            var parameter = element.LookupParameter(parameterName);
            if (parameter is null) { return Result.Failed; }
            if (parameter.StorageType != StorageType.String) { return Result.Failed; }

            // Set parameter value
            parameter.Set(value);
            return Result.Succeeded;
        }

        /// <summary>
        /// Sets the value of an integer based parameter, if able.
        /// </summary>
        /// <param name="element">A Revit Element (extended).</param>
        /// <param name="parameterName">The parameter name to set.</param>
        /// <param name="value">The integer value to set.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_SetParameterValue(this Element element, string parameterName, int value)
        {
            // Early exit on no element or value
            if (element is null) { return Result.Failed; }

            // Try to get parameter, cancel if we can not set it
            var parameter = element.LookupParameter(parameterName);
            if (parameter is null) { return Result.Failed; }
            if (parameter.StorageType != StorageType.Integer) { return Result.Failed; }

            // Set parameter value
            parameter.Set(value);
            return Result.Succeeded;
        }

        /// <summary>
        /// Sets the value of a double based parameter, if able.
        /// </summary>
        /// <param name="element">A Revit Element (extended).</param>
        /// <param name="parameterName">The parameter name to set.</param>
        /// <param name="value">The double value to set.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_SetParameterValue(this Element element, string parameterName, double value)
        {
            // Early exit on no element or value
            if (element is null) { return Result.Failed; }

            // Try to get parameter, cancel if we can not set it
            var parameter = element.LookupParameter(parameterName);
            if (parameter is null) { return Result.Failed; }
            if (parameter.StorageType != StorageType.Double) { return Result.Failed; }

            // Set parameter value
            parameter.Set(value);
            return Result.Succeeded;
        }

        /// <summary>
        /// Sets the value of an ElementId based parameter, if able.
        /// </summary>
        /// <param name="element">A Revit Element (extended).</param>
        /// <param name="parameterName">The parameter name to set.</param>
        /// <param name="value">The ElementId value to set.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_SetParameterValue(this Element element, string parameterName, ElementId value)
        {
            // Early exit on no element or value
            if (element is null || value is null) { return Result.Failed; }

            // Try to get parameter, cancel if we can not set it
            var parameter = element.LookupParameter(parameterName);
            if (parameter is null) { return Result.Failed; }
            if (parameter.StorageType != StorageType.ElementId) { return Result.Failed; }

            // Set parameter value
            parameter.Set(value);
            return Result.Succeeded;
        }

        /// <summary>
        /// Sets the value of an ElementId based parameter, if able.
        /// </summary>
        /// <param name="element">A Revit Element (extended).</param>
        /// <param name="parameterName">The parameter name to set.</param>
        /// <param name="value">The Element for the Id value to set.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_SetParameterValue(this Element element, string parameterName, Element value)
        {
            // Early exit on no element or value
            if (element is null || value is null) { return Result.Failed; }

            // Try to get parameter, cancel if we can not set it
            var parameter = element.LookupParameter(parameterName);
            if (parameter is null) { return Result.Failed; }
            if (parameter.StorageType != StorageType.ElementId) { return Result.Failed; }

            // Set parameter value
            parameter.Set(value.Id);
            return Result.Succeeded;
        }

        #endregion
    }
}