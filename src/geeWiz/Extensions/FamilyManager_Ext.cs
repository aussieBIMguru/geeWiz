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
        #region Get/set current type

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

        #region Get types

        /// <summary>
        /// Gets all types in the family.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <returns> A list of FamilyTypes.</returns>
        public static List<FamilyType> Ext_GetFamilyTypes(this FamilyManager familyManager)
        {
            // Null check
            if (familyManager is null) { return new List<FamilyType>(); }

            // Return all types
            return familyManager.Types
                .Cast<FamilyType>()
                .Where(t => t is not null)
                .Where(t => !t.Name.IsNullOrEmpty())
                .ToList();
        }

        /// <summary>
        /// Gets a family type by name.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <param name="typeName">The type name to find.</param>
        /// <returns>A FamilyType.</returns>
        public static FamilyType Ext_GetFamilyTypeByName(this FamilyManager familyManager, string typeName)
        {
            // Null check
            if (familyManager is null || typeName is null) { return null; }

            // Get and reset the iterator
            var familyTypeSet = familyManager.Types;
            var typesIterator = familyTypeSet.ForwardIterator();
            typesIterator.Reset();

            // Iterate until we find it
            while (typesIterator.MoveNext())
            {
                var familyType = typesIterator.Current as FamilyType;
                if (familyType.Name == typeName)
                {
                    return familyType;
                }
            }

            // Return null if we did not find it
            return null;
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
            // Null check
            if (familyManager is null) { return new List<FamilyParameter>(); }

            // Return all types
            return familyManager.Parameters
                .Cast<FamilyParameter>()
                .Where(p => p is not null)
                .ToList();
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

        #region Add parameters

        /// <summary>
        /// Adds a new shared parameter to a family document.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <param name="definition">The shared parameter definition.</param>
        /// <param name="groupType">The GroupType to put the parameter under.</param>
        /// <param name="instance">If the parameter should be instance based.</param>
        /// <returns>A FamilyParameter.</returns>
        public static FamilyParameter Ext_AddSharedParameter(this FamilyManager familyManager,
            ExternalDefinition definition, ForgeTypeId groupType, bool instance)
        {
            // Catch nulls (GroupType is "Other" if null)
            if (familyManager is null || definition is null) { return null; }
            
            // Make sure parameter does not exist by name
            if (familyManager.Ext_GetFamilyParameterByName(definition.Name) is not null) { return null; }

            // Try to add the parameter
            try
            {
                // Return the new parameter if successful
                return familyManager.AddParameter(definition, groupType, instance);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Adds a new shared parameter to a family document.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <param name="parameterName">The name of the family parameter to make.</param>
        /// <param name="groupType">The GroupType to put the parameter under.</param>
        /// <param name="specType">The SpecType of the new parameter.</param>
        /// <param name="instance">If the parameter should be instance based.</param>
        /// <returns>A FamilyParameter.</returns>
        public static FamilyParameter Ext_AddFamilyParameter(this FamilyManager familyManager,
            string parameterName, ForgeTypeId groupType, ForgeTypeId specType, bool instance)
        {
            // Catch nulls (GroupType is "Other" if null)
            if (familyManager is null || parameterName is null || specType is null) { return null; }

            // Make sure parameter does not exist by name
            if (familyManager.Ext_GetFamilyParameterByName(parameterName) is not null) { return null; }

            // Try to add the parameter
            try
            {
                // Return the new parameter if successful
                return familyManager.AddParameter(parameterName, groupType, specType, instance);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}