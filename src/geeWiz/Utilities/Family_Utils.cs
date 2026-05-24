// Revit API
using Autodesk.Revit.DB;
// geeWiz
using gFam = geeWiz.Utilities.Family_Utils;

// The class belongs to the geeWiz namespace
// using gFam = geeWiz.Utilities.Family_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// An enumeration of possible outcomes of Family processing.
    /// </summary>
    public enum PROCESSING_RESULT
    {
        /// <summary>
        /// Outcome was successful.
        /// </summary>
        SUCCESS = 0,

        /// <summary>
        /// Failure of unknown/general cause.
        /// </summary>
        FAILURE_GENERAL_UNKNOWN = 1,

        /// <summary>
        /// Failure of null/general cause.
        /// </summary>
        FAILURE_GENERAL_NULL = 2,

        /// <summary>
        /// Failure due to type not found by name.
        /// </summary>
        FAILURE_TYPE_NAMENOTFOUND = 10,

        /// <summary>
        /// Failure due to type not able to be deleted.
        /// </summary>
        FAILURE_TYPE_NOTDELETED = 11,

        /// <summary>
        /// Failure due to type name already in document.
        /// </summary>
        FAILURE_TYPE_NAMEEXISTS = 12,

        /// <summary>
        /// Failure due to type not able to be renamed.
        /// </summary>
        FAILURE_TYPE_RENAME = 13,

        /// <summary>
        /// Failure due to Parameter not found by name.
        /// </summary>
        FAILURE_PARAM_NAMENOTFOUND = 20,

        /// <summary>
        /// Failure due to Parameter not able to be deleted.
        /// </summary>
        FAILURE_PARAM_NOTDELETED = 21,

        /// <summary>
        /// Failure due to Parameter name already in document.
        /// </summary>
        FAILURE_PARAM_NAMEEXISTS = 22,

        /// <summary>
        /// Failure due to Family Parameter not able to be renamed.
        /// </summary>
        FAILURE_PARAM_RENAMEFAMILY = 23,

        /// <summary>
        /// Failure due to Shared Parameter not able to be renamed.
        /// </summary>
        FAILURE_PARAM_RENAMESHARED = 24,

        /// <summary>
        /// Failure due to mismatch between SpecTypeIds.
        /// </summary>
        FAILURE_PARAM_SPECMISMATCH = 25,

        /// <summary>
        /// Failure due to new Shared parameter not being created.
        /// </summary>
        FAILURE_PARAM_NEWSHARED = 30,

        /// <summary>
        /// Failure due to new Family parameter not being created.
        /// </summary>
        FAILURE_PARAM_NEWFAMILY = 31,

        /// <summary>
        /// Failure due to parmeter replacement with Shared parameter not succeeding.
        /// </summary>
        FAILURE_PARAM_REPLACEWITHSHARED = 32,

        /// <summary>
        /// Failure due to parmeter replacement with Family parameter not succeeding.
        /// </summary>
        FAILURE_PARAM_REPLACEWITHFAMILY = 33,

        /// <summary>
        /// Failure due to document not being a Family.
        /// </summary>
        FAILURE_DOC_NOTFAMILY = 40,

        /// <summary>
        /// Failure to close document.
        /// </summary>
        FAILURE_DOC_CLOSE = 41,

        /// <summary>
        /// Failure to save document to path.
        /// </summary>
        FAILURE_DOC_SAVEAS = 42,

        /// <summary>
        /// Failure to save and close document.
        /// </summary>
        FAILURE_DOC_SAVEASANDCLOSE = 43,

        /// <summary>
        /// Failure to load document from path.
        /// </summary>
        FAILURE_DOC_LOADFROMFILE = 44,

        /// <summary>
        /// Failure to load Family document into document.
        /// </summary>
        FAILURE_DOC_LOADFROMDOC = 45,

        /// <summary>
        /// Failure to edit family from document.
        /// </summary>
        FAILURE_DOC_EDITFAMILY = 46
    }

    /// <summary>
    /// A class for handling Family processing outcomes.
    /// </summary>
    public class FamilyProccessingOutcome
    {
        #region Properties

        /// <summary>
        /// Related FamilyManager.
        /// </summary>
        public FamilyManager FamilyManager { get; set; }

        /// <summary>
        /// Related Document.
        /// </summary>
        public Document Document { get; set; }

        /// <summary>
        /// Related Family that has been edited.
        /// </summary>
        public Document EditedFamily { get; set; }

        /// <summary>
        /// Related Document title.
        /// </summary>
        public string DocumentTitle { get; set; }

        /// <summary>
        /// Related FamilyType.
        /// </summary>
        public FamilyType RelatedType { get; set; }

        /// <summary>
        /// Related FamilyParameter.
        /// </summary>
        public FamilyParameter RelatedParameter { get; set; }

        /// <summary>
        /// Related FamilyTypes.
        /// </summary>
        public List<FamilyType> RelatedTypes { get; set; }

        /// <summary>
        /// Related FamilyParameters.
        /// </summary>
        public List<FamilyParameter> RelatedParameters { get; set; }

        /// <summary>
        /// Related parameter definition.
        /// </summary>
        public ExternalDefinition RelatedDefinition { get; set; }

        /// <summary>
        /// Related Family.
        /// </summary>
        public Family RelatedFamily { get; set; }

        /// <summary>
        /// Related Parameter name.
        /// </summary>
        public string RelatedParameterName { get; set; }

        /// <summary>
        /// Related file path.
        /// </summary>
        public string RelatedFilePath { get; set; }

        /// <summary>
        /// Related name of loading family.
        /// </summary>
        public string LoadingFamilyName { get; set; }

        /// <summary>
        /// If the outcome was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The related ProcessingResult.
        /// </summary>
        public PROCESSING_RESULT ProcessingResult { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// FamilyManager based constructor.
        /// </summary>
        /// <param name="familyManager">The related FamilyManager.</param>
        /// <param name="processingResult">The default processing result to use.</param>
        public FamilyProccessingOutcome(FamilyManager familyManager, PROCESSING_RESULT processingResult = PROCESSING_RESULT.FAILURE_GENERAL_NULL)
        {
            this.FamilyManager = familyManager;
            this.ProcessingResult = processingResult;

            if (gFam.DOCUMENT_FOCUS is Document doc)
            {
                this.Document = doc;
                this.DocumentTitle = doc.Title;
            }

            this.RelatedTypes = new List<FamilyType>();
            this.RelatedParameters = new List<FamilyParameter>();
        }

        /// <summary>
        /// Document based constructor.
        /// </summary>
        /// <param name="doc">The related Document.</param>
        /// <param name="processingResult">The default processing result to use.</param>
        public FamilyProccessingOutcome(Document doc, PROCESSING_RESULT processingResult = PROCESSING_RESULT.FAILURE_DOC_NOTFAMILY)
        {
            gFam.DOCUMENT_FOCUS = doc;
            this.Document = doc;
            this.DocumentTitle = doc.Title;
            this.FamilyManager = doc.FamilyManager;
            this.ProcessingResult = processingResult;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the processing result to successful.
        /// </summary>
        public void SetSuccess()
        {
            this.Success = true;
            this.ProcessingResult = PROCESSING_RESULT.SUCCESS;
        }

        /// <summary>
        /// Set various properties, with success by default.
        /// </summary>
        /// <param name="relatedParameter">Related family parameter.</param>
        /// <param name="relatedParameters">Related family parameters.</param>
        /// <param name="relatedType">Related family type.</param>
        /// <param name="relatedTypes">Related family types.</param>
        /// <param name="relatedFamily">Related family.</param>
        /// <param name="editedFamily">Related edited family.</param>
        /// <param name="setSuccess">If we want to set success.</param>
        public void SetValues(FamilyParameter relatedParameter = null, List<FamilyParameter> relatedParameters = null,
            FamilyType relatedType = null, List<FamilyType> relatedTypes = null, Family relatedFamily = null, Document editedFamily = null,
            bool setSuccess = true)
        {
            if (relatedParameter is not null) { this.RelatedParameters = relatedParameters; }
            if (relatedParameters is not null) { this.RelatedParameters = relatedParameters; }
            if (relatedType is not null) { this.RelatedType = relatedType; }
            if (relatedTypes is not null) { this.RelatedTypes = relatedTypes; }
            if (relatedFamily is not null) { this.RelatedFamily = relatedFamily; }
            if (editedFamily is not null) { this.EditedFamily = editedFamily; }
            if (setSuccess) { this.SetSuccess(); }
        }

        #endregion
    }

    /// <summary>
    /// A class for handling Family loading behavior.
    /// </summary>
    public class FamilyLoadOptions : IFamilyLoadOptions
    {
        /// <summary>
        /// If we want to overwrite parameter values.
        /// </summary>
        private readonly bool _overwriteValues;
        
        /// <summary>
        /// If we want to overwrite nested families.
        /// </summary>
        private readonly bool _overwriteNested;

        /// <summary>
        /// Construct a FamilyLoadOptions object.
        /// </summary>
        /// <param name="overwriteValues">Overwrite parameter values.</param>
        /// <param name="overwriteNested">Overwrite shared, nested families.</param>
        public FamilyLoadOptions(bool overwriteValues = true, bool overwriteNested = false)
        {
            _overwriteValues = overwriteValues;
            _overwriteNested = overwriteNested;
        }

        /// <summary>
        /// Handle what to do if family already exists.
        /// </summary>
        /// <param name="familyInUse">If the family is in use.</param>
        /// <param name="overwriteParameterValues">If parameters will be overwritten.</param>
        /// <returns>A boolean.</returns>
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = _overwriteValues;
            return true;
        }

        /// <summary>
        /// Handle what to do if a shared, nested family exists.
        /// </summary>
        /// <param name="sharedFamily">The nested family.</param>
        /// <param name="familyInUse">If the family is in use.</param>
        /// <param name="source">The FamilySource to use.</param>
        /// <param name="overwriteParameterValues">If parameters will be overwritten.</param>
        /// <returns>A boolean.</returns>
        public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse,
            out FamilySource source, out bool overwriteParameterValues)
        {
            source = _overwriteNested ? FamilySource.Family : FamilySource.Project;
            overwriteParameterValues = _overwriteValues;
            return true;
        }
    }

    /// <summary>
    /// Static methods container related to Family Documents.
    /// </summary>
    public static class Family_Utils
    {
        /// <summary>
        /// The current Family Document held in focus.
        /// </summary>
        public static Document DOCUMENT_FOCUS = null;

        /// <summary>
        /// Convert a family processing result to a message.
        /// </summary>
        /// <param name="result">The processing result.</param>
        /// <returns>A string.</returns>
        public static string ProcessingResultToString(PROCESSING_RESULT result)
        {
            if (result == PROCESSING_RESULT.SUCCESS)
            {
                return "Success";
            }
            else
            {
                return "Still under development.";
            }
        }
    }
}