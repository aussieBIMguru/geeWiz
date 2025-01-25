// Revit API
using Autodesk.Revit.UI;
// geeWiz libraries
using gFrm = geeWiz.Forms;

// The class belongs to the forms namespace
// using gFrm = geeWiz.Forms (+ .Custom)
namespace geeWiz.Forms
{
    // These classes all form the front end selection forms in Revit
    public static class Custom
    {
        // File filter constant values
        public static string FILTER_TSV = "TSV Files (*.tsv)|*.tsv";
        public static string FILTER_EXCEL = "Excel Files (*.xls;*.xlsx;*.xlsm)|*.xls;*.xlsx;*.xlsm";
        public static string FILTER_RFA = "Family Files|*.rfa";
        public static string FILTER_TXT = "Text Files (*.txt)|*.txt";

        /// <summary>
        /// Creates and shows a bubble message.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="message">An optional message to display.</param>
        /// <param name="filePath">An optional file path to open when the form is clicked.</param>
        /// <param name="linkPath">An optional link path to open when the form is clicked.</param>
        /// <param name="success">Return a successful result.</param>
        /// <returns>A Result (based on success arg).</returns>
        public static Result BubbleMessage(string title = "", string message = "",
            string filePath = null, string linkPath = null, bool success = true)
        {
            // Process the default title conditions
            if (title == "")
            {
                if (filePath != null) { title = "File path"; }
                else if (linkPath != null) { title = "Link path"; }
                else { title = "Default title"; }
            }
            
            // Process the default message conditions
            if (message == "")
            {
                if (filePath != null) { message = "Click here to open file"; }
                else if (linkPath != null) { message = "Click here to open link"; }
                else { message = "Default message"; }
            }

            // Create and show the bubble message
            var bubbleMessage = new gFrm.BubbleMessage(title: title,
                message: message,
                linkPath: linkPath,
                filePath: filePath);
            bubbleMessage.Show();

            // Return the result based on intended success
            if (success) { return Result.Succeeded; }
            else { return Result.Failed; }
        }

        /// <summary>
        /// Processes a generic message to the user.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="message">An optional message to display.</param>
        /// <param name="yesNo">Show Yes and No options instead of OK and Cancel.</param>
        /// <param name="noCancel">Does not offer a cancel button.</param>
        /// <param name="warning">Display a warning icon.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult Message(string title = "", string message = "",
            bool yesNo = false, bool noCancel = false, bool warning = false)
        {
            // Establish the form result to return
            FormResult formResult = new FormResult();
            formResult.Cancelled = false;
            formResult.Affirmative = false;
            formResult.Object = null;

            // Variables to set later for buttons/icon
            MessageBoxButtons buttons;
            MessageBoxIcon icon;

            // Default values if not provided
            if (title == "") { title = "Message"; }
            if (message == "") { message = "No description provided."; }

            // Set the icon
            if (warning) { icon = MessageBoxIcon.Warning; }
            else { icon = MessageBoxIcon.None; }

            // Set the available buttons
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
            DialogResult dialogResult = MessageBox.Show(message, title, buttons, icon);

            // Process the outcomes
            if (dialogResult == DialogResult.Yes || dialogResult == DialogResult.OK)
            {
                formResult.Affirmative = true;
                formResult.Object = true;
            }
            else if (dialogResult == DialogResult.None || dialogResult == DialogResult.Cancel && !noCancel)
            {
                formResult.Object = false;
                formResult.Cancelled = true;
            }
            
            // Return the outcome
            return formResult;
        }


        /// <summary>
        /// Displays a generic cancelled message.
        /// </summary>
        /// <param name="message">An optional message to display.</param>
        /// <returns>Result.Cancelled.</returns>
        public static Result Cancelled(string message = "")
        {
            // Default message
            if (message == "") { message = "Task cancelled."; }

            // Show form to user
            Message(message: message,
                title: "Task cancelled",
                noCancel: true,
                warning: true);
            
            // Return a cancelled result
            return Result.Cancelled;
        }


        /// <summary>
        /// Displays a generic completed message.
        /// </summary>
        /// <param name="message">An optional message to display.</param>
        /// <returns>Result.Succeeded.</returns>
        public static Result Completed(string message = "")
        {
            // Default message
            if (message == "") { message = "Task completed."; }

            // Show form to user
            Message(message: message,
                title: "Task completed",
                noCancel: true);

            // Return a succeeded result
            return Result.Succeeded;
        }

        /// <summary>
        /// Select file path(s) from a browser dialog.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="filter">An optional file type filter.</param>
        /// <param name="multiSelect">If we want to select more than one file path.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult SelectFilePaths(string title = "", string filter = "", bool multiSelect = true)
        {
            // Establish the form result to return
            FormResult formResult = new FormResult();
            formResult.Cancelled = false;
            formResult.Valid = false;
            formResult.Object = null;

            // Using a dialog object
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Default title and filter
                if (title == "") { title = "Select file(s)"; }
                if (filter != "") { openFileDialog.Filter = filter; }

                // Set the typical settings
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = title;
                openFileDialog.Multiselect = multiSelect;

                // Process the results
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    List<object> filePaths = openFileDialog
                        .FileNames
                        .Cast<object>()
                        .ToList();

                    if (multiSelect) { formResult.Objects = filePaths; }
                    else { formResult.Object = filePaths.First(); }
                    formResult.Valid = true;
                }
                else
                {
                    formResult.Cancelled = true;
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
        public static FormResult SelectDirectoryPath(string title = "")
        {
            // Establish the form result to return
            FormResult formResult = new FormResult();
            formResult.Cancelled = false;
            formResult.Valid = false;
            formResult.Object = null;

            // Using a dialog object
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                // Default title and filter
                if (title == "") { title = "Select folder"; }
                folderBrowserDialog.Description = title;

                // Process the result
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    formResult.Object = folderBrowserDialog.SelectedPath as object;
                    formResult.Valid = true;
                }
                else
                {
                    formResult.Cancelled = true;
                }
            }

            // Return the outcome
            return formResult;
        }

        /// <summary>
        /// Processes a form for entering text and/or numbers.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="message">An optional message to display.</param>
        /// <param name="defaultValue">An optinoal default value.</param>
        /// <param name="numberOnly">Enforce a number input only.</param>
        /// <param name="allowEmptyString">An empty string counts as a valid result.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult EnterValue(string title = "", string message = "",
            string defaultValue = "", bool numberOnly = false, bool allowEmptyString = false)
        {
            // Establish the form result to return
            FormResult formResult = new FormResult();
            formResult.Cancelled = false;
            formResult.Affirmative = false;
            formResult.Valid = false;

            // Returned value and number
            string inputValue = "";
            double resultDouble = 0.0;
            
            // Default values
            if (numberOnly)
            {
                if (title == "") { title = "Enter number"; }
                if (message == "") { title = "Enter a numerical input below:"; }
            }
            else
            {
                if (title == "") { title = "Enter text"; }
                if (message == "") { title = "Enter a text input below:"; }
            }

            // Using an enter value form
            using (var form = new gFrm.BaseEnterValue(title: title, message: message,
                defaultValue: defaultValue, numberOnly: numberOnly))
            {
                // Process the outcomes
                if (form.ShowDialog() == DialogResult.OK)
                {
                    formResult.Affirmative = true;
                    inputValue = form.Tag as string;
                }
                else
                {
                    formResult.Cancelled = true;
                    return formResult;
                }
            }

            // Process input as number or text
            if (numberOnly)
            {
                try
                {
                    resultDouble = double.Parse(inputValue);
                    formResult.Valid = true;
                }
                catch
                {
                    resultDouble = 0.0;
                    formResult.Valid = false;
                }
                finally
                {
                    formResult.Object = resultDouble as object;
                }
            }
            else
            {
                formResult.Object = inputValue;
                formResult.Valid = inputValue != "" || allowEmptyString;
            }
            
            // Return the form result
            return formResult;
        }

        /// <summary>
        /// Processes a generic form for showing objects in a list.
        /// </summary>
        /// <param name="keys">A list of keys to display.</param>
        /// <param name="values">A list of values to pass by key.</param>
        /// <param name="title">An optional title to display.</param>
        /// <param name="multiSelect">If we want to select more than one item.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult SelectFromList(List<string> keys, List<object> values,
            string title = "", bool multiSelect = true)
        {
            // Establish the form result to return
            FormResult formResult = new FormResult();
            formResult.Cancelled = false;
            formResult.Affirmative = false;
            formResult.Valid = false;
            List<object> selectedValues = null;
            object selectedValue = null;

            // Default title
            if (title == "")
            {
                if (multiSelect) { title = "Select objects from list:"; }
                else { title = "Select object from list:"; }
            }

            // Using a select items form
            using (var form = new gFrm.BaseListView(keys, values, title: title, multiSelect: multiSelect))
            {
                // Process the outcome
                if (form.ShowDialog() == DialogResult.OK)
                {
                    formResult.Affirmative = true;
                    formResult.Valid = true;
                    if (multiSelect) { selectedValues = form.Tag as List<object>; }
                    else { selectedValue = form.Tag as object; }
                }
                else
                {
                    formResult.Cancelled = true;
                }
            }

            // Set the object values, return the result
            formResult.Objects = selectedValues;
            formResult.Object = selectedValue;
            return formResult;
        }

        /// <summary>
        /// Processes a generic object from list.
        /// </summary>
        /// <param name="keys">A list of keys to display.</param>
        /// <param name="values">A list of values to pass.</param>
        /// <param name="title">An optional title to display.</param>
        /// <param name="message">An optional message to display.</param>
        /// <param name="defaultIndex">An optional index to initialize at.</param>
        /// <returns>A FormResult object.</returns>
        public static FormResult SelectFromDropdown(List<string> keys, List <object> values,
            string title = "", string message = "", int defaultIndex = -1)
        {
            // Establish the form result to return
            FormResult formResult = new FormResult();
            formResult.Cancelled = false;
            formResult.Affirmative = false;
            formResult.Valid = false;
            formResult.Object = null;

            // Default title and message
            if (title == "") { title = "Select object from dropdown"; }
            if (message == "") { message = "Select an object from the dropdown:"; }

            // Using a dropdown form
            using (var form = new gFrm.BaseDropdown(keys, values, title: title, message: message, defaultIndex: defaultIndex))
            {
                // Process the outcome
                if (form.ShowDialog() == DialogResult.OK)
                {
                    formResult.Affirmative = true;
                    formResult.Valid = true;
                    formResult.Object = form.Tag as object;
                }
                else
                {
                    formResult.Cancelled = true;
                }
            }

            // Return the result
            return formResult;
        }

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
    }

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
    }
}