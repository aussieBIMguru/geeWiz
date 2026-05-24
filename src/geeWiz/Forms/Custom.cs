// System
using MessageBox = System.Windows.Forms.MessageBox;
// Revit API
using Autodesk.Revit.UI;
// geeWiz libraries
using gFrm = geeWiz.Forms;
using gCnv = geeWiz.Utilities.Convert_Utils;
using gDat = geeWiz.Utilities.Data_Utils;
// Microsoft
using MsDialogs = Microsoft.WindowsAPICodePack.Dialogs;

// The class belongs to the forms namespace
// using gFrm = geeWiz.Forms (+ .Custom)
namespace geeWiz.Forms
{
    /// <summary>
    /// Static wrapper methods for forms.
    /// </summary>
    public static class Custom
    {
        #region File filter constants

        /// <summary>
        /// File filter string for tsv files.
        /// </summary>
        public static string FILTER_TSV = "TSV Files (*.tsv)|*.tsv";

        /// <summary>
        /// File filter string for Excel files.
        /// </summary>
        public static string FILTER_EXCEL = "Excel Files (*.xls;*.xlsx;*.xlsm)|*.xls;*.xlsx;*.xlsm";

        /// <summary>
        /// File filter string for Revit Family files.
        /// </summary>
        public static string FILTER_RFA = "Family Files|*.rfa";

        /// <summary>
        /// File filter string for txt files.
        /// </summary>
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
        public static FormResult<bool> Message(string title = null, string message = null,
            bool yesNo = false, bool noCancel = false, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            // Establish the form result to return
            var formResult = new FormResult<bool>(valid: false);

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
        public static FormResult<string> SelectFilePaths(string title = null, string filter = null, bool multiSelect = true)
        {
            // Establish the form result to return
            var formResult = new FormResult<string>(valid: false);

            // Using a dialog object
            using (var openFileDialog = new OpenFileDialog())
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
                    List<string> filePaths = openFileDialog.FileNames.ToList();

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
        public static FormResult<string> SelectDirectory_FailSafe(string title = null)
        {
            // Establish the form result to return
            var formResult = new FormResult<string>(valid: false);

            // Default title
            title ??= "Select folder";

            // Using a dialog object
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog() { Description = title })
            {
                // Process the result
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    formResult.Validate(folderBrowserDialog.SelectedPath);
                }
            }

            // Return the outcome
            return formResult;
        }

        /// <summary>
        /// Allows the user to select a directory path (uses code pack dll).
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="multiSelect">An optional title to display.</param>
        /// <returns>A directory path.</returns>
        public static FormResult<string> SelectFolder(string title = null, bool multiSelect = false)
        {
            // Make sure the platform supports this type of directory browser
            if (!MsDialogs.CommonFileDialog.IsPlatformSupported)
            {
                return SelectDirectory_FailSafe(title);
            }

            // Establish the form result to return
            var formResult = new FormResult<string>(valid: false);

            // Using a dialog object
            using (var dialog = new MsDialogs.CommonOpenFileDialog())
            {
                // Set title and default values
                title ??= multiSelect ? "Select folder(s)" : "Select a folder";
                dialog.Title = title;

                // Set properties
                dialog.RestoreDirectory = true;
                dialog.IsFolderPicker = true;
                dialog.Multiselect = multiSelect;

                // Process the result
                if (dialog.ShowDialog() == MsDialogs.CommonFileDialogResult.Ok)
                {
                    if (multiSelect)
                    {
                        formResult.Validate(dialog.FileNames.ToList());
                    }
                    else
                    {
                        formResult.Validate(dialog.FileName);
                    }
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
        public static FormResult<object> EnterValue(string title = null, string message = null,
            string defaultValue = "", bool numberOnly = false, bool allowEmptyString = false)
        {
            // Establish the form result to return
            var formResult = new FormResult<object>(valid: false);

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
                double? tryDouble = gCnv.StringToDouble(inputValue);

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
                formResult.Object = resultDouble;
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
        /// <typeparam name="T">The type of object being stored.</typeparam>
        /// <returns>A FormResult object.</returns>
        public static FormResult<T> SelectFromList<T>(List<string> keys, List<T> values,
            string title = null, bool multiSelect = true)
        {
            // Establish the form result to return
            var formResult = new FormResult<T>(valid: false);

            // Default title
            title ??= multiSelect ? "Select object(s) from list:" : "Select object from list:";

            // Using a select items form
            using (var form = new gFrm.Bases.BaseListView<T>(keys, values, title: title, multiSelect: multiSelect))
            {
                // Process the outcome
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (multiSelect) { formResult.Validate(form.Tag as List<T>); ; }
                    else { formResult.Validate((T)form.Tag); } 
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
        /// <typeparam name="T">The type of object being stored.</typeparam>
        /// <returns>A FormResult object.</returns>
        public static FormResult<T> SelectFromSimpleList<T>(List<string> keys, List<T> values,
            string title = null, bool multiSelect = true)
        {
            // Establish the form result to return
            var formResult = new FormResult<T>(valid: false);

            // Default title
            title ??= multiSelect ? "Select object(s) from list:" : "Select object from list:";

            // Using a select items form
            using (var form = new gFrm.Bases.BaseSimpleListView<T>(keys, values, title: title, multiSelect: multiSelect))
            {
                // Process the outcome
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (multiSelect) { formResult.Validate(form.Tag as List<T>); ; }
                    else { formResult.Validate((T)form.Tag); }
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
        /// <typeparam name="T">The type of object being stored.</typeparam>
        /// <returns>A FormResult object.</returns>
        public static FormResult<T> SelectFromDropdown<T>(List<string> keys, List<T> values,
            string title = null, string message = null, int defaultIndex = -1)
        {
            // Establish the form result to return
            var formResult = new FormResult<T>(valid: false);

            // Default title and message
            title ??= "Select object from dropdown";
            message ??= "Select an object from the dropdown:";

            // Using a dropdown form
            using (var form = new gFrm.Bases.BaseDropdown<T>(keys, values, title: title, message: message, defaultIndex: defaultIndex))
            {
                // Process the outcome
                if (form.ShowDialog() == DialogResult.OK)
                {
                    formResult.Validate((T)form.Tag);
                }
            }

            // Return the result
            return formResult;
        }

        #endregion
    }

    /// <summary>
    /// Static wrapper methods for Wpf forms.
    /// </summary>
    public static class CustomWpf
    {
        #region SelectFromList

        /// <summary>
        /// Processes a generic form for showing objects in a list with a text filter.
        /// </summary>
        /// <param name="keys">A list of keys to display.</param>
        /// <param name="values">A list of values to pass by key.</param>
        /// <param name="title">An optional title to display.</param>
        /// <param name="multiSelect">If we want to select more than one item.</param>
        /// <param name="allowNoSelection">The form permits the user to proceed with no chosen items.</param>
        /// <typeparam name="T">The type of object being stored.</typeparam>
        /// <returns>A FormResult object.</returns>
        public static FormResult<T> SelectFromList<T>(List<string> keys, List<T> values,
            string title = null, bool multiSelect = true, bool allowNoSelection = false)
        {
            // Establish the form result to return
            var formResult = new FormResult<T>(valid: false);

            // Default title
            title ??= multiSelect ? "Select object(s) from list:" : "Select object from list:";

            // Keyed object process
            var keyedObjects = gDat.CombineAsKeyedObjects(keys, values, showMessages: true);
            if (keyedObjects is null) { return formResult; }

            // Run the Wpf form
            var dlg = new Bases.WpfListView(keyedObjects, multiSelect, title, allowNoSelection);

            // Process the outcome if affirmative
            if (dlg.ShowDialog() == true)
            {
                List<T> chosenItems = dlg.GetChosenItems()
                    .Select(i => i.ItemValue)
                    .OfType<T>()
                    .ToList();

                if (multiSelect)
                {
                    if (chosenItems.Count > 0 || allowNoSelection)
                    {
                        formResult.Validate(objs: chosenItems);
                    }
                }
                else
                {
                    if (chosenItems.Count > 0)
                    {
                        formResult.Validate(obj: chosenItems.First());
                    }
                }
            }

            // Return the result
            return formResult;
        }

        #endregion
    }

    /// <summary>
    /// A class for holding form outcomes, used by custom forms.
    /// </summary>
    /// <typeparam name="T">The type of object being stored.</typeparam>
    public class FormResult<T>
    {
        /// <summary>
        /// Returned objects from the form.
        /// </summary>
        public List<T> Objects { get; set; }
        
        /// <summary>
        /// Returned object from the form.
        /// </summary>
        public T Object { get; set; }

        /// <summary>
        /// Was the form cancelled.
        /// </summary>
        public bool Cancelled { get; set; }

        /// <summary>
        /// Was the outcome of the form valid.
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// Was the outcome of the form affirmative.
        /// </summary>
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
            Objects = new List<T>();
            Object = default;
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
        public void Validate(T obj)
        {
            this.Validate();
            this.Object = obj;
        }

        /// <summary>
        /// Sets the dialog result to valid, passing a list of objects.
        /// </summary>
        /// <param name="objs">Objects to pass to result.</param>
        public void Validate(List<T> objs)
        {
            this.Validate();
            this.Objects = objs;
        }
    }
}