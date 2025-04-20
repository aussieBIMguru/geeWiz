// Autodesk
using Autodesk.Revit.UI;

// The class belongs to the extensions namespace
// FamilyManager familyManager.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to the FamilyManager.
    /// </summary>
    public static class FamilyManager_Ext
    {
        #region Current type

        /// <summary>
        /// Returns the current family type.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <returns>A FamilyType.</returns>
        public static FamilyType Ext_GetCurrentType(this FamilyManager familyManager)
        {
            // Null check
            if (familyManager is null) { return null; }

            // Return current type
            return familyManager.CurrentType;
        }

        /// <summary>
        /// Sets the current family type.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <param name="familyType"></param>
        /// <returns>A Result.</returns>
        public static Result Ext_SetCurrentType(this FamilyManager familyManager, FamilyType familyType)
        {
            // Null check
            if (familyManager is null || familyType is null) { return Result.Failed; }

            // Set the current type, return success
            familyManager.CurrentType = familyType;
            return Result.Succeeded;
        }

        #endregion

        #region Family types

        /// <summary>
        /// Gets all types in the family.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <returns> A list of FamilyTypes.</returns>
        public static List<FamilyType> Ext_GetFamilyTypes(this FamilyManager familyManager)
        {
            // Empty list of types
            var familyTypes = new List<FamilyType>();
            
            // Null check
            if (familyManager is null) { return familyTypes; }

            // Get and reset the iterator
            var familyTypeSet = familyManager.Types;
            var typesIterator = familyTypeSet.ForwardIterator();
            typesIterator.Reset();

            // Get all types
            while (typesIterator.MoveNext())
            {
                familyTypes.Add(typesIterator.Current as FamilyType);
            }

            // Return all types
            return familyTypes;
        }

        #endregion

        #region Family parameters

        /// <summary>
        /// Gets all parameters in the family.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <returns>A list of FamilyParameters.</returns>
        public static List<FamilyParameter> Ext_GetFamilyParameters(this FamilyManager familyManager)
        {
            // Empty list of parameters
            var familyParameters = new List<FamilyParameter>();

            // Null check
            if (familyManager is null) { return familyParameters; }

            // Get and reset the iterator
            var familyParmeterSet = familyManager.Parameters;
            var parametersIterator = familyParmeterSet.ForwardIterator();
            parametersIterator.Reset();

            // Get all parameters
            while (parametersIterator.MoveNext())
            {
                familyParameters.Add(parametersIterator.Current as FamilyParameter);
            }

            // Return all parameters
            return familyParameters;
        }

        /// <summary>
        /// Gets a family parameter by name.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <param name="parameterName">The parameter name to find.</param>
        /// <returns>A FamilyParameter.</returns>
        public static FamilyParameter Ext_GetFamilyParameterByName(this FamilyManager familyManager, string parameterName)
        {
            // Null check
            if (familyManager is null || parameterName is null) { return null; }

            // Get and reset the iterator
            var familyParmeterSet = familyManager.Parameters;
            var parametersIterator = familyParmeterSet.ForwardIterator();
            parametersIterator.Reset();

            // Iterate until we find it
            while (parametersIterator.MoveNext())
            {
                var parameter = parametersIterator.Current as FamilyParameter;
                if (parameter.Definition.Name == parameterName)
                {
                    return parameter;
                }
            }

            // Return null if we did not find it
            return null;
        }

        #endregion
    }
}