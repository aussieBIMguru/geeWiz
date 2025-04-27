// Revit API
using Autodesk.Revit.UI;
// geeWiz libraries
using gFrm = geeWiz.Forms;
using gCnv = geeWiz.Utilities.Convert_Utils;

// The class belongs to the forms namespace
// using gFrm = geeWiz.Forms (+ .Custom)
namespace geeWiz.Forms
{
    // These classes all form the front end selection forms in Revit
    public static class Custom
    {
        #region File filter constants

        // File filter constant values
        public static string FILTER_TSV = "TSV Files (*.tsv)|*.tsv";
        public static string FILTER_EXCEL = "Excel Files (*.xls;*.xlsx;*.xlsm)|*.xls;*.xlsx;*.xlsm";
        public static string FILTER_RFA = "Family Files|*.rfa";
        public static string FILTER_TXT = "Text Files (*.txt)|*.txt";

        #endregion

        #region BubbleMessage

        /// <summary>
        /// Creates and shows a bubble message.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="message">An optional message to display.</param>
        /// <param name="filePath">An optional file path to open when the form is clicked.</param>
        /// <param name="linkPath">An optional link path to open when the form is clicked.</param>
        /// <param name="success">Return a successful result.</param>
        /// <returns>A Result (based on success arg).</returns>
        public static Result BubbleMessage(string title = null, string message = null,
            string filePath = null, string linkPath = null, bool success = true)
        {
            // Process the default title conditions
            if (title is null)
            {
                if (filePath != null) { title = "File path"; }
                else if (linkPath != null) { title = "Link path"; }
                else { title = "Default title"; }
            }

            // Process the default message conditions
            if (message is null)
            {
                if (filePath != null) { message = "Click here to open file"; }
                else if (linkPath != null) { message = "Click here to open link"; }
                else { message = "Default message"; }
            }

            // Create and show the bubble message
            var bubbleMessage = new gFrm.Bases.BubbleMessage(title: title,
                message: message,
                linkPath: linkPath,
                filePath: filePath);
            bubbleMessage.Show();

            // Return the result based on intended success
            if (success) { return Result.Succeeded; }
            else { return Result.Failed; }
        }

        #endregion

        #region Message (+ variants)

        /// <summary>
        /// Processes a generic message to the user.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="message">An optional message to display.</param>
        /// <param name="yesNo">Show Yes and No options instead of OK and Cancel.</param>
        /// <param name="noCancel">Does not offer a cancel button.</param>
        /// <param name="icon">The icon type to display.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult Message(string title = null, string message = null,
            bool yesNo = false, bool noCancel = false, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            // Establish the form result to return
            var formResult = new FormResult(valid: false);

            // Default values if not provided
            title ??= "Message";
            message ??= "No description provided.";

            // Set the question icon
            if (yesNo) { icon = MessageBoxIcon.Question; }

            // Set the available buttons
            MessageBoxButtons buttons;

            if (noCancel)
            {
                buttons = MessageBoxButtons.OK;
            }
            else
            {
                if (yesNo) { buttons = MessageBoxButtons.YesNo; }
                else { buttons = MessageBoxButtons.OKCancel; }
            }

            // Create a messagebox, process its dialog result
            var dialogResult = MessageBox.Show(message, title, buttons, icon);

            // Process the outcomes
            if (dialogResult == DialogResult.Yes || dialogResult == DialogResult.OK)
            {
                formResult.Validate();
            }

            // Return the outcome
            return formResult;
        }

        /// <summary>
        /// Displays a generic completed message.
        /// </summary>
        /// <param name="message">An optional message to display.</param>
        /// <returns>Result.Succeeded.</returns>
        public static Result Completed(string message = null)
        {
            // Default message
            message ??= "Task completed.";

            // Show form to user
            Message(message: message,
                title: "Task completed",
                noCancel: true,
                icon: MessageBoxIcon.Information);

            // Return a succeeded result
            return Result.Succeeded;
        }

        /// <summary>
        /// Displays a generic cancelled message.
        /// </summary>
        /// <param name="message">An optional message to display.</param>
        /// <returns>Result.Cancelled.</returns>
        public static Result Cancelled(string message = null)
        {
            // Default message
            message ??= "Task cancelled.";

            // Show form to user
            Message(message: message,
                title: "Task cancelled",
                noCancel: true,
                icon: MessageBoxIcon.Warning);

            // Return a cancelled result
            return Result.Cancelled;
        }

        /// <summary>
        /// Displays a generic error message.
        /// </summary>
        /// <param name="message">An optional message to display.</param>
        /// <returns>Result.Failed.</returns>
        public static Result Error(string message = null)
        {
            // Default message
            message ??= "Error encountered.";

            // Show form to user
            Message(message: message,
                title: "Error",
                noCancel: true,
                icon: MessageBoxIcon.Error);

            // Return a cancelled result
            return Result.Failed;
        }

        #endregion

        #region Select files / directory

        /// <summary>
        /// Select file path(s) from a browser dialog.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="filter">An optional file type filter.</param>
        /// <param name="multiSelect">If we want to select more than one file path.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult SelectFilePaths(string title = null, string filter = null, bool multiSelect = true)
        {
            // Establish the form result to return
            var formResult = new FormResult(valid: false);

            // Using a dialog object
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Default title and filter
                title ??= multiSelect ? "Select file(s)" : "Select a file";
                if (filter is not null) { openFileDialog.Filter = filter; }

                // Set the typical settings
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = title;
                openFileDialog.Multiselect = multiSelect;

                // Process the results
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePaths = openFileDialog
                        .FileNames
                        .Cast<object>()
                        .ToList();

                    if (multiSelect) { formResult.Validate(filePaths); }
                    else { formResult.Validate(filePaths.First()); }
                }
            }

            // Return the outcome
            return formResult;
        }

        /// <summary>
        /// Select a directory path from a browser dialog.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult SelectDirectoryPath(string title = null)
        {
            // Establish the form result to return
            var formResult = new FormResult(valid: false);

            // Default title
            title ??= "Select folder";

            // Using a dialog object
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog() { Description = title })
            {
                // Process the result
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    formResult.Validate(folderBrowserDialog.SelectedPath as object);
                }
            }

            // Return the outcome
            return formResult;
        }

        #endregion

        #region EnterValue

        /// <summary>
        /// Processes a form for entering text and/or numbers.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="message">An optional message to display.</param>
        /// <param name="defaultValue">An optinoal default value.</param>
        /// <param name="numberOnly">Enforce a number input only.</param>
        /// <param name="allowEmptyString">An empty string counts as a valid result.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult EnterValue(string title = null, string message = null,
            string defaultValue = "", bool numberOnly = false, bool allowEmptyString = false)
        {
            // Establish the form result to return
            var formResult = new FormResult(valid: false);

            // Returned value and number
            string inputValue = "";
            double resultDouble = 0.0;

            // Default values
            if (numberOnly)
            {
                title ??= "Enter number";
                title ??= "Enter a numerical input below:";
            }
            else
            {
                title ??= "Enter text";
                title ??= "Enter a text input below:";
            }

            // Using an enter value form
            using (var form = new gFrm.Bases.BaseEnterValue(title: title, message: message,
                defaultValue: defaultValue, numberOnly: numberOnly))
            {
                // Process the outcomes
                if (form.ShowDialog() == DialogResult.OK)
                {
                    inputValue = form.Tag as string;
                    formResult.Validate();
                }
                else
                {
                    // Early return
                    return formResult;
                }
            }

            // Process input as number
            if (numberOnly)
            {
                // Try to parse as double
                var tryDouble = gCnv.StringToDouble(inputValue);

                // Process the outcome
                if (tryDouble.HasValue)
                {
                    resultDouble = tryDouble.Value;
                }
                else
                {
                    formResult.Valid = false;
                }

                // Set form object anyway
                formResult.Object = resultDouble as object;
            }
            // Otherwise, process as text
            else
            {
                formResult.Object = inputValue;
                formResult.Valid = inputValue != "" || allowEmptyString;
            }

            // Return the form result
            return formResult;
        }

        #endregion

        #region SelectFromList

        /// <summary>
        /// Processes a generic form for showing objects in a list with a text filter.
        /// </summary>
        /// <param name="keys">A list of keys to display.</param>
        /// <param name="values">A list of values to pass by key.</param>
        /// <param name="title">An optional title to display.</param>
        /// <param name="multiSelect">If we want to select more than one item.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult SelectFromList(List<string> keys, List<object> values,
            string title = null, bool multiSelect = true)
        {
            // Establish the form result to return
            var formResult = new FormResult(valid: false);

            // Default title
            title ??= multiSelect ? "Select object(s) from list:" : "Select object from list:";

            // Using a select items form
            using (var form = new gFrm.Bases.BaseListView(keys, values, title: title, multiSelect: multiSelect))
            {
                // Process the outcome
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (multiSelect) { formResult.Validate(form.Tag as List<object>); ; }
                    else { formResult.Validate(form.Tag as object); } 
                }
            }

            // Return the result
            return formResult;
        }

        /// <summary>
        /// Processes a generic form for showing objects in a simple list.
        /// </summary>
        /// <param name="keys">A list of keys to display.</param>
        /// <param name="values">A list of values to pass by key.</param>
        /// <param name="title">An optional title to display.</param>
        /// <param name="multiSelect">If we want to select more than one item.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult SelectFromSimpleList(List<string> keys, List<object> values,
            string title = null, bool multiSelect = true)
        {
            // Establish the form result to return
            var formResult = new FormResult(valid: false);

            // Default title
            title ??= multiSelect ? "Select object(s) from list:" : "Select object from list:";

            // Using a select items form
            using (var form = new gFrm.Bases.BaseSimpleListView(keys, values, title: title, multiSelect: multiSelect))
            {
                // Process the outcome
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (multiSelect) { formResult.Validate(form.Tag as List<object>); ; }
                    else { formResult.Validate(form.Tag as object); }
                }
            }

            // Return the result
            return formResult;
        }

        #endregion

        #region SelectFromDropdown

        /// <summary>
        /// Processes a generic object from list.
        /// </summary>
        /// <param name="keys">A list of keys to display.</param>
        /// <param name="values">A list of values to pass.</param>
        /// <param name="title">An optional title to display.</param>
        /// <param name="message">An optional message to display.</param>
        /// <param name="defaultIndex">An optional index to initialize at.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult SelectFromDropdown(List<string> keys, List<object> values,
            string title = null, string message = null, int defaultIndex = -1)
        {
            // Establish the form result to return
            var formResult = new FormResult(valid: false);

            // Default title and message
            title ??= "Select object from dropdown";
            message ??= "Select an object from the dropdown:";

            // Using a dropdown form
            using (var form = new gFrm.Bases.BaseDropdown(keys, values, title: title, message: message, defaultIndex: defaultIndex))
            {
                // Process the outcome
                if (form.ShowDialog() == DialogResult.OK)
                {
                    formResult.Validate(form.Tag as object);
                }
            }

            // Return the result
            return formResult;
        }

        #endregion
    }

    // These classes provide form utility
    public static class Utilities
    {
        #region Progress bar delay

        /// <summary>
        /// Calculates ideal sleep delay for a progress form.
        /// </summary>
        /// <param name="steps">Steps to take.</param>
        /// <param name="duration">The desired overall progress time (in ms).</param>
        /// <param name="imposeLimit">Keep between realistic min/max of 1 and 200ms.</param>
        /// <returns>The delay (in ms).</returns>
        public static int ProgressDelay(int steps, int duration = 3000, bool imposeLimit = true)
        {
            // Catch one step or less
            if (steps < 2) { return duration; }

            // Calculate the step
            int step = duration / steps;

            // Catch limit imposed
            if (imposeLimit)
            {
                if (step < 1) { step = 1; }
                else if (step > 200) { step = 200; }
            }

            // Return the step
            return step;
        }

        #endregion
    }

    #region FormResult class

    /// <summary>
    /// A class for holding form outcomes, used by custom forms.
    /// </summary>
    public class FormResult
    {
        // These properties hold the resulting object or objects from the form
        public List<object> Objects { get; set; }
        public object Object { get; set; }

        // These properties allow us to verify the outcome of the form
        public bool Cancelled { get; set; }
        public bool Valid { get; set; }
        public bool Affirmative { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public FormResult() { }

        /// <summary>
        /// Constructor to begin a FormResult.
        /// </summary>
        /// <param name="valid">Should the result begin as valid.</param>
        public FormResult(bool valid)
        {
            Objects = new List<object>();
            Object = null;
            Cancelled = !valid;
            Valid = valid;
            Affirmative = valid;
        }

        /// <summary>
        /// Sets all dialog related results to valid.
        /// </summary>
        public void Validate()
        {
            Cancelled = false;
            Valid = true;
            Affirmative = true;
        }

        /// <summary>
        /// Sets the dialog result to valid, passing an object.
        /// </summary>
        /// <param name="obj">Object to pass to result.</param>
        public void Validate(object obj)
        {
            this.Validate();
            this.Object = obj;
        }

        /// <summary>
        /// Sets the dialog result to valid, passing a list of objects.
        /// </summary>
        /// <param name="objs">Objects to pass to result.</param>
        public void Validate(List<object> objs)
        {
            this.Validate();
            this.Objects = objs;
        }
    }

    #endregion
}