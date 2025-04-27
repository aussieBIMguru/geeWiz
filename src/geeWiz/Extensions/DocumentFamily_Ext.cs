// System
using System.IO;
// geeWiz
using gFam = geeWiz.Utilities.Family_Utils;
using gFil = geeWiz.Utilities.File_Utils;

// The class belongs to the extensions namespace (partial class)
// Document doc.ExtensionMethod()
namespace geeWiz.Extensions
{
    /// <summary>
    /// Methods of this class generally relate to family documents.
    /// </summary>
    public static partial class Document_Ext
    {
        #region Get family manager, verify family doc

        /// <summary>
        /// Gets the family manager and sets the global document.
        /// </summary>
        /// <param name="doc">The document (extended).</param>
        /// <returns>A FamilyManager.</returns>
        public static FamilyManager Ext_GetFamilyManager(this Document doc)
        {
            // Null and non-family document checks
            if (doc is null) { return null; }

            // Set the global properties
            Globals.CurrentDocument = doc;

            // Return if document is family
            return doc.FamilyManager;
        }

        /// <summary>
        /// Returns if a document is a family document.
        /// </summary>
        /// <param name="doc">The document (extended).</param>
        /// <returns>A boolean.</returns>
        public static bool Ext_IsFamilyDocument(this Document doc)
        {
            // Null and non-family document checks
            if (doc is null) { return false; }

            // Return if document is family
            return doc.IsFamilyDocument;
        }

        #endregion

        #region Saving and closing

        /// <summary>
        /// Closes a document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="save">If the document is to be saved.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_CloseFamilyDocument(this Document doc, bool save = false)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(doc);

            // Null and non-family document checks
            if (!doc.Ext_IsFamilyDocument()) { return processingOutcome; }

            // Try to close the document
            try
            {
                doc.Close(save);
                processingOutcome.SetSuccess();
                return processingOutcome;
            }
            catch
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_DOC_CLOSE;
                return processingOutcome;
            }
        }

        /// <summary>
        /// Saves a document to a file path, with the option to close afterwards.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="filePath">The filepath to save the document to.</param>
        /// <param name="options">SaveAsOptions (optional).</param>
        /// <param name="closeAfterSaving">Close afterwards (without saving).</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_SaveAsFamilyFile(this Document doc, string filePath,
            SaveAsOptions options = null, bool closeAfterSaving = false)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(doc);

            // Null and non-family document checks
            if (!doc.Ext_IsFamilyDocument()) { return processingOutcome; }

            // Default options
            options ??= new SaveAsOptions() { OverwriteExistingFile = true };

            // Try to save the document
            try
            {
                doc.SaveAs(filePath, options);
                processingOutcome.SetSuccess();
            }
            catch
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_DOC_SAVEAS;
            }

            // Optionally close the document
            if (closeAfterSaving)
            {
                try
                {
                    doc.Close();
                }
                catch
                {
                    processingOutcome.Success = false;
                    processingOutcome.ProcessingResult =
                        processingOutcome.ProcessingResult == gFam.PROCESSING_RESULT.FAILURE_DOC_SAVEAS
                        ? gFam.PROCESSING_RESULT.FAILURE_DOC_SAVEASANDCLOSE
                        : gFam.PROCESSING_RESULT.FAILURE_DOC_CLOSE;
                }
            }

            // Return the outcome
            return processingOutcome;
        }

        #endregion

        #region Loading from path/document

        /// <summary>
        /// Loads a family document into the document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="familyDoc">The family document to load.</param>
        /// <param name="options">The IFamilyLoadOptions.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_LoadFamilyDocument(this Document doc, Document familyDoc, IFamilyLoadOptions options = null)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(doc);

            // Null check, ensure document is a family
            if (doc is null || !familyDoc.Ext_IsFamilyDocument()) { return processingOutcome; }

            // Set the properties
            processingOutcome.LoadingFamilyName = familyDoc.Title;

            // Default options
            options ??= new gFil.FamilyLoadOptions(true, false);

            // Try to load the family
            try
            {
                var family = familyDoc.LoadFamily(doc, options);
                processingOutcome.SetValues(family: family);
                return processingOutcome;
            }
            catch
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_DOC_LOADFROMDOC;
                return processingOutcome;
            }
        }

        /// <summary>
        /// Loads a family into the document from a file path.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="filePath">The filepath to load from.</param>
        /// <param name="options">The IFamilyLoadOptions.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_LoadFamilyFromPath(this Document doc, string filePath, IFamilyLoadOptions options = null)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(doc);

            // Null check, ensure file exists
            if (doc is null || !File.Exists(filePath)) { return processingOutcome; }

            // Set the properties
            processingOutcome.RelatedFilePath = filePath;

            // Default options
            options ??= new gFil.FamilyLoadOptions(true, false);

            // Try to load and return the family
            try
            {
                if (doc.LoadFamily(filePath, options, out Family family))
                {
                    processingOutcome.SetValues(family: family);
                    return processingOutcome;
                }
            }
            catch
            {
                // Proceed to last step VVV
            }

            // If we got here, we failed to load
            processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_DOC_LOADFROMFILE;
            return processingOutcome;
        }

        #endregion

        #region Opening document

        /// <summary>
        /// Opens a Family from a document.
        /// </summary>
        /// <param name="doc">The Document (extended).</param>
        /// <param name="family">The family to edit.</param>
        /// <returns>A FamilyProccessingOutcome.</returns>
        public static gFam.FamilyProccessingOutcome Ext_OpenFamilyAsDocument(this Document doc, Family family)
        {
            // Processing result
            var processingOutcome = new gFam.FamilyProccessingOutcome(doc, gFam.PROCESSING_RESULT.FAILURE_GENERAL_NULL);

            // Null checks
            if (doc is null || family is null) { return processingOutcome; }

            // Make sure the family is from that document
            if (doc != family.Document)
            {
                processingOutcome.ProcessingResult = gFam.PROCESSING_RESULT.FAILURE_DOC_EDITFAMILY;
                return processingOutcome;
            }

            // Try to edit the family
            try
            {
                var editedFamily = doc.EditFamily(family);
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