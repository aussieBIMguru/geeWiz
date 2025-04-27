// geeWiz
using gFam = geeWiz.Utilities.Family_Utils;

// The class belongs to the extensions namespace
// Family family.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to Families.
    /// </summary>
    public static class Family_Ext
    {
        #region Namekey

        /// <summary>
        /// Constructs a name key based on a Revit Family.
        /// </summary>
        /// <param name="family">A Revit Family.</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToFamilyKey(this Family family, bool includeId = false)
        {
            // Null catch
            if (family is null) { return "???"; }

            // Construct the key without Id
            string familyKey = $"{family.FamilyCategory.Name}: {family.Name}";

            // Return key with Id
            if (includeId)
            {
                return $"{familyKey} [{family.Id.ToString()}]";
            }
            // Return key without Id
            else
            {
                return familyKey;
            }
        }

        #endregion

        #region Get all types

        /// <summary>
        /// Returns all types of the provided family.
        /// </summary>
        /// <param name="family">The Family (extended).</param>
        /// <returns>A list of FamilySymbols.</returns>
        public static List<FamilySymbol> Ext_GetAllTypes(this Family family)
        {
            // Null check
            if (family is null) { return new List<FamilySymbol>(); }

            // Get all instances of same symbool
            return family.Document.Ext_Collector()
                .WherePasses(new FamilySymbolFilter(family.Id))
                .Cast<FamilySymbol>()
                .ToList();
        }

        #endregion

        #region Family document actions

        /// <summary>
        /// Opens a Family from a document.
        /// </summary>
        /// <param name="family">The family (extended).</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_OpenFamilyAsDocument(this Family family)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(family.Document, processingResult: gFam.PROCESSING_RESULT.FAILURE_GENERAL_NULL);

            // Null check
            if (family is null) { return processingOutcome; }

            // Try to edit the family
            try
            {
                var editedFamily = family.Document.EditFamily(family);
                processingOutcome.SetValues(editedFamily: editedFamily);
                return processingOutcome;
            }
            catch
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_DOC_EDITFAMILY;
                return processingOutcome;
            }
        }

        #endregion
    }
}
