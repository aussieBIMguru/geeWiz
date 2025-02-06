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
            // Get document if not provided
            doc ??= element.Document;

            // If document is not workshare, element is editable naturally
            if (!doc.IsWorkshared) { return true; }

            // Get the checkout and model updates status
            CheckoutStatus checkoutStatus = WorksharingUtils.GetCheckoutStatus(doc, element.Id);
            ModelUpdatesStatus updatesStatus = WorksharingUtils.GetModelUpdatesStatus(doc, element.Id);

            // Check if owned by another user
            if (checkoutStatus == CheckoutStatus.OwnedByOtherUser) { return false; }

            // Check if it is already owned by us
            else if (checkoutStatus == CheckoutStatus.OwnedByCurrentUser) { return true; }

            // Finally, ensure element is current with central
            else { return updatesStatus == ModelUpdatesStatus.CurrentWithCentral; }
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
            ForgeTypeId forgeTypeId = ParameterUtils.GetParameterTypeId(builtInParameter);
            return element.GetParameter(forgeTypeId);
        }

        /// <summary>
        /// Constructs a parameter helper object to get parameter values.
        /// </summary>
        /// <param name="element">A Revit Element (extended).</param>
        /// <param name="parameterName">The parameter name to get.</param>
        /// <returns>A ParameterHelper object.</returns>
        public static ParameterHelper Ext_GetParameterValueByName(this Element element, string parameterName)
        {
            return new ParameterHelper(element, parameterName);
        }

        /// <summary>
        /// Gets the value of a text based parameter, if able.
        /// </summary>
        /// <param name="element">A Revit Element (extended).</param>
        /// <param name="parameterName">The parameter name to get.</param>
        /// <returns>A string.</returns>
        public static string Ext_GetParameterTextByName(this Element element, string parameterName)
        {
            // Early exit on null
            if (element is null) { return null; }

            // Try to get parameter
            Parameter parameter = element.LookupParameter(parameterName);

            // Check if parameter is valid
            if (parameter is null) { return null; }

            // Return string value if parameter is string type
            if (parameter.StorageType == StorageType.String)
            {
                return parameter.AsString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the value of a text based parameter, if able.
        /// </summary>
        /// <param name="element">A Revit Element (extended).</param>
        /// <param name="parameterName">The parameter name to set.</param>
        /// <param name="value">The string value to set.</param>
        /// <returns>A Result.</returns>
        public static Result Ext_SetParameterTextByName(this Element element, string parameterName, string value)
        {
            // Early exit on nulls
            if (element is null || value is null) { return Result.Failed; }
            
            // Try to get parameter
            Parameter parameter = element.LookupParameter(parameterName);

            // Check if parameter is valid
            if (parameter is null) { return Result.Failed; }

            // Set string value if parameter is string type
            if (parameter.StorageType == StorageType.String)
            {
                parameter.Set(value);
                return Result.Succeeded;
            }

            // Return failed if we did not succeed
            return Result.Failed;
        }

        #endregion
    }
}