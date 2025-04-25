// Autodesk
using View = Autodesk.Revit.DB.View;

// The class belongs to the extensions namespace
// DesignOption designOption.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to DesignOptions.
    /// </summary>
    public static class DesignOption_Ext
    {
        #region Namekey

        /// <summary>
        /// Constructs a name key based on a DesignOption.
        /// </summary>
        /// <param name="designOption">A DesignOption (extended).</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToFamilyKey(this DesignOption designOption, bool includeId = false)
        {
            // Null catch
            if (designOption is null) { return "???"; }

            // Construct the key without Id
            string nameKey = $"{designOption.Ext_GetDesignOptionSetName()}: {designOption.Name}";

            // Return key with Id
            if (includeId)
            {
                return $"{nameKey} [{designOption.Id.ToString()}]";
            }
            // Return key without Id
            else
            {
                return nameKey;
            }
        }

        #endregion

        #region Get elements

        /// <summary>
        /// Gets all elements related to a design option.
        /// </summary>
        /// <param name="designOption">A DesignOption (extended).</param>
        /// <param name="view">An optional view.</param>
        /// <returns>A list of Elements.</returns>
        public static List<Element> Ext_GetOptionElements(this DesignOption designOption, View view = null)
        {
            // Collect all elements on the design option
            return designOption.Document.Ext_Collector(view)
                .WherePasses(new ElementDesignOptionFilter(designOption.Id))
                .ToList();
        }

        #endregion

        #region DesignOption sets

        /// <summary>
        /// Returns a design options set element.
        /// </summary>
        /// <param name="designOption">The design option (extended).</param>
        /// <returns>An Element.</returns>
        public static Element Ext_GetDesignOptionSet(this DesignOption designOption)
        {
            // Null check
            if (designOption is null) { return null; }
            
            // Get the option set Id parameter
            var parameter = designOption.Ext_GetBuiltInParameter(BuiltInParameter.OPTION_SET_ID);
            if (parameter is null) { return null; }

            // Return the design option set as an element
            return parameter.AsElementId().Ext_GetElement(designOption.Document);
        }

        /// <summary>
        /// Returns a design options set name.
        /// </summary>
        /// <param name="designOption">The design option (extended).</param>
        /// <returns>A string.</returns>
        public static string Ext_GetDesignOptionSetName(this DesignOption designOption)
        {
            // Get the design option set
            var optionSet = designOption.Ext_GetDesignOptionSet();

            // Null check
            if (optionSet is null) { return null; }

            // Return the design option set name
            return optionSet.Name;
        }

        #endregion
    }
}