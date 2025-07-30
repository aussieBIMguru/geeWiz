// geeWiz
using gDat = geeWiz.Utilities.Data_Utils;
using gFam = geeWiz.Utilities.Family_Utils;

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
        /// Returns the current family type (via the CurrentType property).
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_GetCurrentType(this FamilyManager familyManager)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null check
            if (familyManager is null) { return processingOutcome; }

            // Set current type
            processingOutcome.SetValues(relatedType: familyManager.CurrentType);
            return processingOutcome;
        }

        /// <summary>
        /// Sets the current family type.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <param name="familyType"></param>
        /// <returns>A FamilyProcessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_SetCurrentType(this FamilyManager familyManager, FamilyType familyType)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null check
            if (familyManager is null || familyType is null) { return processingOutcome; }

            // Set the current type
            familyManager.CurrentType = familyType;
            processingOutcome.SetValues(relatedType: familyType);

            // Return processing outcome
            return processingOutcome;
        }

        #endregion

        #region Get types

        /// <summary>
        /// Gets all types in the family.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <returns> A FamilyProcessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_GetFamilyTypes(this FamilyManager familyManager)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null check
            if (familyManager is null) { return processingOutcome; }

            // Get family types
            var familyTypes = familyManager.Types
                .Cast<FamilyType>()
                .Where(t => t is not null)
                .Where(t => !t.Name.IsNullOrEmpty())
                .ToList();

            // Return successful outcome
            processingOutcome.SetValues(relatedTypes: familyTypes);
            return processingOutcome;
        }

        /// <summary>
        /// Gets a family type by name.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <param name="typeName">The type name to find.</param>
        /// <param name="types">Optional types to search through.</param>
        /// <param name="typeNames">Optional types names to search through.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_GetFamilyTypeByName(this FamilyManager familyManager, string typeName,
            List<FamilyType> types = null, List<string> typeNames = null)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null check
            if (familyManager is null || typeName is null) { return processingOutcome; }

            // Return the type if provided and found
            if (types is not null && typeNames is not null)
            {
                if (gDat.FindItemAtKey<FamilyType>(typeName, types, typeNames) is FamilyType familyType)
                {
                    processingOutcome.SetValues(relatedType: familyType);
                    return processingOutcome;
                }
            }

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
                    processingOutcome.SetValues(relatedType: familyType);
                    return processingOutcome;
                }
            }

            // Return failed to find result
            processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_TYPE_NAMENOTFOUND;
            return processingOutcome;
        }

        #endregion

        #region Family parameters

        /// <summary>
        /// Gets all parameters in the family.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <returns>A FamilyProcessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_GetFamilyParameters(this FamilyManager familyManager)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null check
            if (familyManager is null) { return processingOutcome; }

            // Get parameters
            var familyParameters = familyManager.Parameters
                .Cast<FamilyParameter>()
                .Where(p => p is not null)
                .ToList();

            // Return successful outcome
            processingOutcome.SetValues(relatedParameters: familyParameters);
            return processingOutcome;
        }

        /// <summary>
        /// Gets a family parameter by name.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <param name="parameterName">The parameter name to find.</param>
        /// <param name="parameters">Optional parameters to search through.</param>
        /// <param name="parameterNames">Optional parameter names to search through.</param>
        /// <returns>A FamilyProcessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_GetFamilyParameterByName(this FamilyManager familyManager, string parameterName,
            List<FamilyParameter> parameters = null, List<string> parameterNames = null)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null check
            if (familyManager is null || parameterName is null) { return processingOutcome; }

            // Catch if we have parameters and names provided
            if (parameters is not null && parameterName is not null)
            {
                if (gDat.FindItemAtKey<FamilyParameter>(parameterName, parameters, parameterNames) is FamilyParameter familyParameter)
                {
                    processingOutcome.SetValues(relatedParameter: familyParameter);
                    return processingOutcome;
                }
            }

            // Get and reset the iterator
            var familyParmeterSet = familyManager.Parameters;
            var parametersIterator = familyParmeterSet.ForwardIterator();
            parametersIterator.Reset();

            // Iterate until we find it
            while (parametersIterator.MoveNext())
            {
                var familyParameter = parametersIterator.Current as FamilyParameter;
                if (familyParameter.Definition.Name == parameterName)
                {
                    processingOutcome.SetValues(relatedParameter: familyParameter);
                    return processingOutcome;
                }
            }

            // Return failed to find result
            processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NAMENOTFOUND;
            return processingOutcome;
        }

        /// <summary>
        /// Renames a family parameter.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <param name="familyParameter">The parameter to rename.</param>
        /// <param name="newName">The new name to use.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_RenameFamilyParameter (this FamilyManager familyManager, FamilyParameter familyParameter, string newName)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null check
            if (familyManager is null || familyParameter is null || newName is null) { return processingOutcome; }

            // Store related properties
            processingOutcome.RelatedParameter = familyParameter;
            processingOutcome.RelatedParameterName = newName;

            // Make sure parameter is not shared
            if (familyParameter.IsShared)
            {
                processingOutcome.SetValues(relatedParameter: familyParameter, setSuccess: false);
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_RENAMESHARED;
                return processingOutcome;
            }

            // Make sure new name does not exist
            if (familyManager.Ext_GetFamilyParameterByName(newName).Success == true)
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NAMEEXISTS;
                return processingOutcome;
            }

            // Try to rename the parameter
            try
            {
                familyManager.RenameParameter(familyParameter, newName);
                processingOutcome.SetSuccess();
                return processingOutcome;
            }
            catch
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_RENAMEFAMILY;
                return processingOutcome;
            }
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
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_AddSharedParameter(this FamilyManager familyManager,
            ExternalDefinition definition, ForgeTypeId groupType, bool instance)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Catch nulls (GroupType is "Other" if null)
            if (familyManager is null || definition is null) { return processingOutcome; }

            // Set the properties
            processingOutcome.RelatedDefinition = definition;
            
            // Make sure parameter does not exist by name
            if (familyManager.Ext_GetFamilyParameterByName(definition.Name).Success == true)
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NAMEEXISTS;
                return processingOutcome;
            }

            // Try to add the parameter
            try
            {
                var newParameter = familyManager.AddParameter(definition, groupType, instance);
                processingOutcome.SetValues(relatedParameter: newParameter);
                return processingOutcome;
            }
            catch
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NEWSHARED;
                return processingOutcome;
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
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_AddFamilyParameter(this FamilyManager familyManager,
            string parameterName, ForgeTypeId groupType, ForgeTypeId specType, bool instance)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Catch nulls (GroupType is "Other" if null)
            if (familyManager is null || parameterName is null || specType is null) { return processingOutcome; }

            // Set the properties
            processingOutcome.RelatedParameterName = parameterName;

            // Make sure parameter does not exist by name
            if (familyManager.Ext_GetFamilyParameterByName(parameterName).Success == true)
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NAMEEXISTS;
                return processingOutcome;
            }

            // Try to add the parameter
            try
            {
                var newParameter = familyManager.AddParameter(parameterName, groupType, specType, instance);
                processingOutcome.SetValues(relatedParameter: newParameter);
                return processingOutcome;
            }
            catch
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NEWFAMILY;
                return processingOutcome;
            }
        }

        #endregion

        #region Replace parameters

        /// <summary>
        /// Replaces a shared parameter with a new family parameter.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended)</param>
        /// <param name="replaceParameter">The parameter to replace.</param>
        /// <param name="withName">The name of the new family parameter.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_ReplaceSharedWithFamilyParameter(this FamilyManager familyManager, FamilyParameter replaceParameter, string withName)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null check
            if (familyManager is null || replaceParameter is null || withName is null) { return processingOutcome; }

            // Set the properties
            processingOutcome.RelatedParameter = replaceParameter;
            processingOutcome.RelatedParameterName = withName;

            // Cancel if parameter is not shared
            if (!replaceParameter.IsShared)
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_REPLACEWITHFAMILY;
                return processingOutcome;
            }

            // Cancel if parameter name exists
            if (familyManager.Ext_GetFamilyParameterByName(withName).Success == true)
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NAMEEXISTS;
                return processingOutcome;
            }

            // Try to replace it
            try
            {
                var newParameter = familyManager.ReplaceParameter(replaceParameter, withName, replaceParameter.Definition.GetGroupTypeId(), replaceParameter.IsInstance);
                processingOutcome.SetValues(relatedParameter: newParameter);
                return processingOutcome;
            }
            catch
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_REPLACEWITHFAMILY;
                return processingOutcome;
            }
        }

        /// <summary>
        /// Replaces a shared parameter with a new family parameter.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended)</param>
        /// <param name="replaceParameter">The parameter to replace.</param>
        /// <param name="withDefinition">The shared parameter definition to use.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_ReplaceFamilyWithSharedParameter(this FamilyManager familyManager, FamilyParameter replaceParameter, ExternalDefinition withDefinition)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null check
            if (familyManager is null || replaceParameter is null || withDefinition is null) { return processingOutcome; }

            // Set the properties
            processingOutcome.RelatedParameter = replaceParameter;
            processingOutcome.RelatedDefinition = withDefinition;

            // Cancel if parameter is shared
            if (replaceParameter.IsShared)
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_REPLACEWITHSHARED;
                return processingOutcome;
            }

            // Cancel if spec type does not match
            if (replaceParameter.Definition.GetDataType() != withDefinition.GetDataType())
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_SPECMISMATCH;
                return processingOutcome;
            }

            // Cancel if parameter name exists, allowing for the shared and family parameter to match in name
            if (familyManager.Ext_GetFamilyParameterByName(withDefinition.Name).Success == true && replaceParameter.Definition.Name != withDefinition.Name)
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NAMEEXISTS;
                return processingOutcome;
            }

            // Try to replace it
            try
            {
                var newParameter = familyManager.ReplaceParameter(replaceParameter, withDefinition, replaceParameter.Definition.GetGroupTypeId(), replaceParameter.IsInstance);
                processingOutcome.SetValues(relatedParameter: newParameter);
                return processingOutcome;
            }
            catch
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_REPLACEWITHSHARED;
                return processingOutcome;
            }
        }

        /// <summary>
        /// Replaces a parameter with a shared parameter.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended)</param>
        /// <param name="replaceParameter">The parameter to replace.</param>
        /// <param name="withDefinition">The shared parameter definition to use.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_ReplaceParameterWithSharedParameter(this FamilyManager familyManager, FamilyParameter replaceParameter, ExternalDefinition withDefinition)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null check
            if (familyManager is null || replaceParameter is null || withDefinition is null) { return processingOutcome; }

            // Set the properties
            processingOutcome.RelatedParameter = replaceParameter;
            processingOutcome.RelatedDefinition = withDefinition;

            // Make temporary name
            var tempParameterName = $"TEMP_{withDefinition.Name}";

            // Check if parameter exists already, allowing for the shared and family parameter to match in name
            if (familyManager.Ext_GetFamilyParameterByName(withDefinition.Name).Success == true && replaceParameter.Definition.Name != withDefinition.Name)
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NAMEEXISTS;
                return processingOutcome;
            }

            // Check if temporary parameter exists already
            if (familyManager.Ext_GetFamilyParameterByName(tempParameterName).Success == true)
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NAMEEXISTS;
                return processingOutcome;
            }

            // If parameter is shared, replace with project then shared
            if (replaceParameter.IsShared)
            {
                try
                {
                    var tempParameter = familyManager.ReplaceParameter(replaceParameter, tempParameterName, replaceParameter.Definition.GetGroupTypeId(), replaceParameter.IsInstance);
                    var newParameter = familyManager.ReplaceParameter(replaceParameter, withDefinition, replaceParameter.Definition.GetGroupTypeId(), replaceParameter.IsInstance);
                    processingOutcome.SetValues(relatedParameter: newParameter);
                    return processingOutcome;
                }
                catch
                {
                    processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_REPLACEWITHSHARED;
                    return processingOutcome;
                }
            }
            // Otherwise, replace as family
            else
            {
                try
                {
                    var newParameter = familyManager.ReplaceParameter(replaceParameter, withDefinition, replaceParameter.Definition.GetGroupTypeId(), replaceParameter.IsInstance);
                    processingOutcome.SetValues(relatedParameter: newParameter);
                    return processingOutcome;
                }
                catch
                {
                    processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_REPLACEWITHSHARED;
                    return processingOutcome;
                }
            }
        }

        /// <summary>
        /// Replaces a parameter with a family parameter.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended)</param>
        /// <param name="replaceParameter">The parameter to replace.</param>
        /// <param name="withName">The new family parameter name to use.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_ReplaceParameterWithFamilyParameter(this FamilyManager familyManager, FamilyParameter replaceParameter, string withName)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null check
            if (familyManager is null || replaceParameter is null || withName is null) { return processingOutcome; }

            // Set the properties
            processingOutcome.RelatedParameter = replaceParameter;
            processingOutcome.RelatedParameterName = withName;

            // Check if parameter exists already, allowing for the shared and family parameter to match in name
            if (familyManager.Ext_GetFamilyParameterByName(withName).Success == true && replaceParameter.Definition.Name != withName)
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NAMEEXISTS;
                return processingOutcome;
            }

            // If parameter is shared, replace it
            if (replaceParameter.IsShared)
            {
                // Try to replace it
                try
                {
                    var newParameter = familyManager.ReplaceParameter(replaceParameter, withName, replaceParameter.Definition.GetGroupTypeId(), replaceParameter.IsInstance);
                    processingOutcome.SetValues(relatedParameter: newParameter);
                    return processingOutcome;
                }
                catch
                {
                    processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_REPLACEWITHFAMILY;
                    return processingOutcome;
                }
            }
            // Otherwise, rename the parameter
            else
            {
                try
                {
                    familyManager.RenameParameter(replaceParameter, withName);
                    processingOutcome.SetSuccess();
                    return processingOutcome;
                }
                catch
                {
                    processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_REPLACEWITHFAMILY;
                    return processingOutcome;
                }
            }
        }

        #endregion

        #region Parameter formulae

        // Coming soon :)

        #endregion

        #region Delete parameters and types

        /// <summary>
        /// Deletes a family type.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <param name="familyType">The family type to delete.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_DeleteFamilyType(this FamilyManager familyManager, FamilyType familyType)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null catch
            if (familyManager is null || familyType is null) { return processingOutcome; }
            
            // Store the properties
            processingOutcome.RelatedType = familyType;

            // Try to delete the type
            try
            {
                familyManager.Ext_SetCurrentType(familyType);
                familyManager.DeleteCurrentType();
                processingOutcome.SetValues(relatedType: null);
                return processingOutcome;
            }
            catch
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_TYPE_NOTDELETED;
                return processingOutcome;
            }
        }

        /// <summary>
        /// Deletes a family parameter.
        /// </summary>
        /// <param name="familyManager">The FamilyManager (extended).</param>
        /// <param name="familyParameter">The family parameter to delete.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_DeleteFamilyParameter(this FamilyManager familyManager, FamilyParameter familyParameter)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(familyManager);

            // Null catch
            if (familyManager is null || familyParameter is null) { return processingOutcome; }

            // Store the properties
            processingOutcome.RelatedParameter = familyParameter;

            // Try to delete the parameter
            try
            {
                familyManager.RemoveParameter(familyParameter);
                processingOutcome.SetValues(relatedParameter: null);
                return processingOutcome;
            }
            catch
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_PARAM_NOTDELETED;
                return processingOutcome;
            }
        }

        #endregion
    }
}