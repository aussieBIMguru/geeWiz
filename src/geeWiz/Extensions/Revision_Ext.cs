// The class belongs to the extensions namespace
// Revision revision.ExtensionMethod()
namespace geeWiz.Extensions
{
    #region Namekey

    /// <summary>
    /// Methods of this class generally relate to Revisions.
    /// </summary>
    public static class Revision_Ext
    {
        /// <summary>
        /// Constructs a name key based on a Revision.
        /// </summary>
        /// <param name="revision">A Revit Revision (extended).</param>
        /// <param name="includeId">Append the ElementId to the end.</param>
        /// <returns>A string.</returns>
        public static string Ext_ToRevisionKey(this Revision revision, bool includeId = false)
        {
            // Null catch
            if (revision is null) { return "???"; }

            // Return key with Id
            if (includeId)
            {
                return $"{revision.SequenceNumber}: {revision.RevisionDate} - {revision.Description} [{revision.ToString()}]";
            }
            // Return key without Id
            else
            {
                return $"{revision.SequenceNumber}: {revision.RevisionDate} - {revision.Description}";
            }
        }
    }

    #endregion
}