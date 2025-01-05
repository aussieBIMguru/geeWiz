using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using gFrm = geeWiz.Forms;

namespace geeWiz.Forms
{
    // These classes all form the front end selection forms in Revit
    public static class Custom
    {
        /// <summary>
        /// Processes a generic alert to the user.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="message">An optional message to display.</param>
        /// <param name="yes_no">Show Yes and No instead (if you cancel).</param>
        /// <param name="no_cancel">Does not offer a cancel button.</param>
        /// <param name="warn_icon">Show a warning icon in the message.</param>
        /// <returns>A true/false outcome if the selection was OK/yes.</returns>
        public static FormResults Alert(string title = "", string message = "", bool yes_no = false, bool no_cancel = false, bool warn_icon = false)
        {
            // Final result
            FormResults formResult = new FormResults();
            formResult.resultCancelled = false;
            formResult.resultAffirmative = false;
            MessageBoxButtons buttons;
            MessageBoxIcon icon;
            // Default values
            if (title == "") { title = "Alert"; }
            if (message == "") { message = "Message to user."; }
            // Set the icon
            if (warn_icon) { icon = MessageBoxIcon.Warning; } else { icon = MessageBoxIcon.None; }
            // Set the buttons
            if (no_cancel)
            {
                // No cancel = only OK
                buttons = MessageBoxButtons.OK;
            }
            else
            {
                if (yes_no)
                {
                    buttons = MessageBoxButtons.YesNo;
                }
                else
                {
                    buttons = MessageBoxButtons.OKCancel;
                }
            }
            // Create a messagebox, get result
            DialogResult result = MessageBox.Show(message, title, buttons, icon);
            // Return if the outcome is OK/Yes
            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                // True outcome, was not cancelled
                formResult.resultAffirmative = true;
                formResult.resultObject = true;
            }
            else
            {
                // Check for cancellation
                if (result == DialogResult.None || result == DialogResult.Cancel && !no_cancel)
                {
                    formResult.resultObject = false;
                    formResult.resultCancelled = true;
                }
            }
            // Return the outcome
            return formResult;
        }


        /// <summary>
        /// Processes a script cancelled alert to the user.
        /// </summary>
        /// <param name="message">A message to display.</param>
        /// <returns>A cancelled result, message UI only.</returns>
        public static Result AlertCancel(string message)
        {
            // Show form to user
            Alert(message: message, title: "Task cancelled", no_cancel: true, warn_icon: true);
            return Result.Cancelled;
        }


        /// <summary>
        /// Processes a task completed alert to the user.
        /// </summary>
        /// <param name="message">A message to display.</param>
        /// <returns>A succeeded result, message UI only.</returns>
        public static Result AlertCompleted(string message)
        {
            // Show form to user
            Alert(message: message, title: "Task completed", no_cancel: true);
            return Result.Succeeded;
        }

        /// <summary>
        /// Processes a bubble message alert.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="message">An optional message to display.</param>
        /// <param name="filePath">An optional file path to attach on click.</param>
        /// <param name="urlPath">An optional link path to attach on click.</param>
        /// <param name="success">Return a successful result.</param>
        /// <returns>A succeeded result.</returns>
        public static Result BubbleMessage(string title = "", string message = "", string filePath = null, string urlPath = null, bool success = true)
        {
            if (title == "")
            {
                if (filePath != null) { title = "File path"; }
                else if (urlPath != null) { title = "Link URL"; }
                else { title = "Default title"; }
            }
            if (message == "")
            {
                if (filePath != null) { message = "Click here to open file"; }
                else if (urlPath != null) { message = "Click here to open link"; }
                else { message = "Default message"; }
            }
            var bubbleMessage = new gFrm.BubbleMessage(title: title, message: message, urlPath: urlPath, filePath: filePath);
            bubbleMessage.Show();
            
            if (success) { return Result.Succeeded; }
            else { return Result.Failed; }
        }

        /// <summary>
        /// Allows the user to select file paths.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="filter">An optional file type filter.</param>
        /// <returns>A list of file paths.</returns>
        public static List<string> SelectFiles(string title = "", string filter = "")
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Settings for dialog
                if (filter != "") { openFileDialog.Filter = filter; }
                openFileDialog.RestoreDirectory = true;
                if (title == "") { title = "Select file(s)"; }
                openFileDialog.Title = title;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileNames.ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Allows the user to select a file path.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="filter">An optional file type filter.</param>
        /// <returns>A file path.</returns>
        public static string SelectFile(string title = "", string filter = "")
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Settings for dialog
                if (filter != "") { openFileDialog.Filter = filter; }
                openFileDialog.RestoreDirectory = true;
                if (title == "") { title = "Select a file"; }
                openFileDialog.Title = title;
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileNames.First();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Allows the user to select a directory path.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <returns>A directory path.</returns>
        public static string SelectDirectory(string title = "")
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                // Settings for dialog
                if (title == "") { title = "Select a folder"; }
                folderBrowserDialog.Description = title;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    return folderBrowserDialog.SelectedPath;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Processes a generic form for entering text.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="tooltip">An optional tooltip to display.</param>
        /// <param name="defaultValue">A default value to display.</param>
        /// <param name="numberOnly">Accept numerical input only.</param>
        /// <returns>The entered text.</returns>
        public static FormResults EnterText(string title = "", string tooltip = "", string defaultValue = "", bool numberOnly = false)
        {
            // Final result
            FormResults formResult = new FormResults();
            formResult.resultCancelled = false;
            formResult.resultAffirmative = false;
            string inputValue = "";
            double resultDouble = 0.0;
            // Default values
            if (numberOnly)
            {
                if (title == "") { title = "Enter text"; }
                if (tooltip == "") { title = "Enter a text input below:"; }
            }
            else
            {
                if (title == "") { title = "Enter number"; }
                if (tooltip == "") { title = "Enter a numerical input below:"; }
            }
            // Run the form
            try
            {
                using (var form = new gFrm.EnterValue(title: title, tooltip: tooltip, defaultValue: defaultValue, numberOnly: numberOnly))
                {
                    // If we encountered an OK outcome...
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Get the text input from the form
                        formResult.resultAffirmative = true;
                        inputValue = form.Tag as string;
                    }
                    // Otherwise if the form ran, we cancelled it
                    else
                    {
                        formResult.resultCancelled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // If we couldn't use it, we failed
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                formResult.resultCancelled = true;
            }
            // Process numerical input
            if (numberOnly)
            {
                try
                {
                    resultDouble = double.Parse(inputValue);
                    formResult.resultValid = true;
                }
                catch
                {
                    resultDouble = 0.0;
                    formResult.resultValid = false;
                }
                finally
                {
                    formResult.resultObject = resultDouble;
                }
            }
            // Process text input
            else
            {
                formResult.resultObject = inputValue;
                formResult.resultValid = inputValue != "";
            }
            // Collect objects, return result
            return formResult;
        }

        /// <summary>
        /// Processes a generic form for showing objects.
        /// </summary>
        /// <param name="keys">A list of keys to display.</param>
        /// <param name="values">A list of values to pass.</param>
        /// <param name="title">An optional title to display.</param>
        /// <param name="multi_select">Select more than one item.</param>
        /// <returns>Object(s) that were selected.</returns>
        public static FormResults SelectItemsFromList(List<string> keys, List<object> values, string title = "", bool multi_select = true)
        {
            // Final result
            FormResults formResults = new FormResults();
            formResults.resultCancelled = false;
            formResults.resultAffirmative = false;
            List<object> selectedValues = null;
            object selectedValue = null;

            // Default title
            if (title == "")
            {
                if (multi_select) { title = "Select objects:"; }
                else { title = "Select object:"; }
            }

            // Run the form
            try
            {

                using (var form = new gFrm.SelectFromList(keys, values, title: title, multi_select: multi_select))
                {
                    // If we encountered an OK outcome...
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Get the list of objects from the form tag
                        formResults.resultAffirmative = true;
                        if (multi_select)
                        {
                            selectedValues = form.Tag as List<object>;
                        }
                        else
                        {
                            selectedValue = form.Tag as object;
                        }
                    }
                    // Otherwise if the form ran, we cancelled it
                    else
                    {
                        formResults.resultCancelled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // If we couldn't use it, we failed
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                formResults.resultCancelled = true;
            }

            // Collect objects, return result
            formResults.resultObjects = selectedValues;
            formResults.resultObject = selectedValue;
            return formResults;
        }

        /// <summary>
        /// Processes a generic object from list.
        /// </summary>
        /// <param name="keys">A list of keys to display.</param>
        /// <param name="values">A list of values to pass.</param>
        /// <param name="title">An optional title to display.</param>
        /// <param name="tooltip">An optional tooltip to display.</param>
        /// <returns>The selected object.</returns>
        public static FormResults SelectOneItem(List<string> keys, List <object> values, string title = "", string tooltip = "")
        {
            // Final result
            FormResults formResults = new FormResults();
            formResults.resultCancelled = false;
            formResults.resultAffirmative = false;
            object selectedValue = new object();

            // Default values
            if (title == "") { title = "Select item from list"; }
            if (tooltip == "") { tooltip = "Select an item from the dropdown:"; }

            // Run the form
            try
            {

                using (var form = new gFrm.SelectOneItem(keys, values, title: title, tooltip: tooltip))
                {
                    // If we encountered an OK outcome...
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Get the list of objects from the form tag
                        formResults.resultAffirmative = true;
                        selectedValue = form.Tag as object;
                    }
                    // Otherwise if the form ran, we cancelled it
                    else
                    {
                        formResults.resultCancelled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // If we couldn't use it, we failed
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                formResults.resultCancelled = true;
            }

            // Collect sheets, return result
            formResults.resultObject = selectedValue;
            return formResults;
        }

        /// <summary>
        /// Calculates ideal sleep delay.
        /// </summary>
        /// <param name="count">Object/step count.</param>
        /// <param name="duration">How long you want the process to take.</param>
        /// <returns>The selected revision.</returns>
        public static int ProgressStep(int count, int duration = 3000, bool limit = true)
        {
            // Catch zero
            if (count < 1) { return duration; }
            
            // Calculate the step
            int step = duration / count;

            // Ensure no less than 50
            if (step < 1 && limit) { return 1; }
            else if (step > 200 && limit) { return 200; }
            else { return step; }
        }
    }

    /// <summary>
    /// Classes for holding multiple values from form processing outcomes.
    /// </summary>

    // Class to return result and objects
    public class FormResults
    {
        public List<Object> resultObjects { get; set; }
        public object resultObject { get; set; }
        public bool resultCancelled { get; set; }
        public bool resultValid { get; set; }
        public bool resultAffirmative { get; set; }
    }
}