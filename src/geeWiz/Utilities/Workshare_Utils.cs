// geeWiz utilities
using gFrm = geeWiz.Forms;
using geeWiz.Extensions;

// The class belongs to the utility namespace
// using gWsh = geeWiz.Utilities.Workshare_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to checking for editability.
    /// </summary>
    public static class Workshare_Utils
    {
        #region Editable processing routine

        /// <summary>
        /// Reviews multiple elements for editability, allowing for further processing.
        /// </summary>
        /// <param name="elements">Elements to process.</param>
        /// <returns>A WorksharingResult object.</returns>
        public static WorksharingResult ProcessElements(List<Element> elements, Document doc = null)
        {
            // Variables for use later
            var worksharingResults = new WorksharingResult();
            var editable = new List<Element>();
            var nonEditable = new List<Element>();

            // Iterate over elements
            foreach (Element element in elements)
            {
                if (element.Ext_IsEditable(doc))
                {
                    editable.Add(element);
                }
                else
                {
                    nonEditable.Add(element);
                }
            }

            // Assign worksharing results
            worksharingResults.Editable = editable;
            worksharingResults.NotEditable = nonEditable;
            worksharingResults.Cancelled = false;

            // If no elements are editable
            if (editable.Count == 0)
            {
                // Message to user if we lost all elements
                if (elements.Count > 0)
                {
                    gFrm.Custom.Cancelled("Elements were found, but all of them are not editable\n\n" +
                        "The task cannot proceed, and has been cancelled.");
                }

                // Results are deemed cancelled
                worksharingResults.Cancelled = true;
            }
            // Catch if some are not editable
            else if (nonEditable.Count > 0)
            {
                // Present form to user
                gFrm.FormResult formResult = gFrm.Custom.Message(title: "Please choose an option",
                    message: "Not all elements are editable.\n\nProceed with only editable elements?",
                    yesNo: true, warning: true);

                // Catch and capture cancellation
                if (formResult.Cancelled)
                {
                    worksharingResults.Cancelled = true;
                }
            }

            // Return the worksharing outcome
            return worksharingResults;
        }

        #endregion
    }

    #region WorksharingResult class

    /// <summary>
    /// Class to store and process editable element checks.
    /// </summary>
    public class WorksharingResult
    {
        public List<Element> Editable { get; set; }
        public List<Element> NotEditable { get; set; }
        public bool Cancelled { get; set; }
    }

    #endregion
}
