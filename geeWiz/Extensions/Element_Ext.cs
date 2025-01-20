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
        /// <summary>
        /// Constructs a name key based on the class the Element is.
        /// </summary>
        /// <param name="element">A Revit Element.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToNameKey(this Element element, bool includeId)
        {
            // Catch null element
            if (element is null)
            {
                if (includeId)
                {
                    return "???: ??? [-1]";
                }
                else
                {
                    return "???: ???";
                }
            }
            
            // Default state of values
            string prefix = "???";
            string suffix = "???";

            // Check if element is a sheet
            if (element is ViewSheet sheet)
            {
                prefix = "Sheet";
                suffix = $"{sheet.SheetNumber} - {sheet.Name}";
            }
            // Check if element is a viewfamilytype
            else if (element is ViewFamilyType viewFamilyType)
            {
                prefix = viewFamilyType.ViewFamily.ToString();
                suffix = viewFamilyType.Name;
            }
            // Check if element is a view
            else if (element is View view)
            {
                prefix = view.ViewType.ToString();
                if (view.IsTemplate)
                {
                    prefix += " template";
                }
                suffix = view.Name;
            }
            // Check if element is a family type
            else if (element is FamilySymbol familySymbol)
            {
                prefix = familySymbol.Family.FamilyCategory.Name;
                suffix = $"{familySymbol.Family.Name} - {familySymbol.Name}";
            }
            // Check if element is a family instance
            else if (element is FamilyInstance familyInstance)
            {
                var symbol = familyInstance.Symbol;
                prefix = symbol.Family.FamilyCategory.Name;
                suffix = $"{symbol.Family.Name} - {symbol.Name}";
            }
            // Otherwise process as an element
            else
            {
                // Try to get category name
                if (element.Category is Category category)
                {
                    prefix = category.Name;
                }
                // Otherwise try to get element type name
                else
                {
                    try
                    {
                        prefix = element.GetType().Name;
                    }
                    catch {; }
                }
                
                // Try to get element name
                try
                {
                    suffix = element.Name;
                }
                catch {; }
            }

            // If we include element id
            if (includeId && element.Id is ElementId id)
            {
                suffix += $" [{id.ToString()}]";
            }

            // Return the key
            return $"{prefix}: {suffix}";
        }

        /// <summary>
        /// Constructs a parameter helper object to get parameter values.
        /// </summary>
        /// <param name="element">A Revit Element.</param>
        /// <param name="doc">The Document to delete from (optional).</param>
        /// <returns>A ParameterHelper object.</returns>
        public static Result Ext_TryToDelete(this Element element, Document doc = null)
        {
            // Get the document if null
            if (doc is null) { doc = element.Document; }
            
            // Try to delete the element
            try
            {
                doc.Delete(element.Id);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Failed;
            }
        }

        /// <summary>
        /// Returns if an element is editable.
        /// </summary>
        /// <param name="element">The Element to check (extended).</param>
        /// <param name="doc">The Revit document (optional).</param>
        /// <returns>A boolean.</returns>
        public static bool Ext_Editable(this Element element, Document doc = null)
        {
            // Get document if not provided
            if (doc is null) { doc = element.Document; }

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
    }
}