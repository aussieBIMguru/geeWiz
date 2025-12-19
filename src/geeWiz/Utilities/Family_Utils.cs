// The class belongs to the geeWiz namespace
// using gFam = geeWiz.Utilities.Family_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to managing family documents.
    /// </summary>
    public static class Family_Utils
    {
        #region Processing result management

        /// <summary>
        /// Store various family processing outcomes.
        /// </summary>
        public enum PROCESSING_RESULT
        {
            SUCCESS = 0,

            FAILURE_GENERAL_UNKNOWN = 1,
            FAILURE_GENERAL_NULL = 2,

            FAILURE_TYPE_NAMENOTFOUND = 10,
            FAILURE_TYPE_NOTDELETED = 11,
            FAILURE_TYPE_NAMEEXISTS = 12,
            FAILURE_TYPE_RENAME = 13,

            FAILURE_PARAM_NAMENOTFOUND = 20,
            FAILURE_PARAM_NOTDELETED = 21,
            FAILURE_PARAM_NAMEEXISTS = 22,
            FAILURE_PARAM_RENAMEFAMILY = 23,
            FAILURE_PARAM_RENAMESHARED = 24,
            FAILURE_PARAM_SPECMISMATCH = 25,

            FAILURE_PARAM_NEWSHARED = 30,
            FAILURE_PARAM_NEWFAMILY = 31,
            FAILURE_PARAM_REPLACEWITHSHARED = 32,
            FAILURE_PARAM_REPLACEWITHFAMILY = 33,

            FAILURE_DOC_NOTFAMILY = 40,
            FAILURE_DOC_CLOSE = 41,
            FAILURE_DOC_SAVEAS = 42,
            FAILURE_DOC_SAVEASANDCLOSE = 43,
            FAILURE_DOC_LOADFROMFILE = 44,
            FAILURE_DOC_LOADFROMDOC = 45,
            FAILURE_DOC_EDITFAMILY = 46
        }

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

        #endregion

        /// <summary>
        /// Object for handling family processing outcomes.
        /// </summary>
        public class FamilyProccessingOutcome
        {
            #region Properties

            public FamilyManager FamilyManager { get; set; }
            public Document Document { get; set; }
            public Document EditedFamily { get; set; }
            public string DocumentTitle { get; set; }
            public FamilyType RelatedType { get; set; }
            public FamilyParameter RelatedParameter { get; set; }
            public List<FamilyType> RelatedTypes { get; set; }
            public List<FamilyParameter> RelatedParameters { get; set; }
            public ExternalDefinition RelatedDefinition { get; set; }
            public Family RelatedFamily { get; set; }
            public string RelatedParameterName { get; set; }
            public string RelatedFilePath { get; set; }
            public string LoadingFamilyName { get; set; }
            public bool Success { get; set; }
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

                if (Globals.FocalDocument is Document doc)
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
                Globals.FocalDocument = doc;
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
    }
}