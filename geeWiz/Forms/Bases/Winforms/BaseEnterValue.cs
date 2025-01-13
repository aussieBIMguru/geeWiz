// System
using System;
using System.Windows.Forms;

// The base form will belong to the forms namespace
namespace geeWiz.Forms
{
    /// <summary>
    /// Standard class for showing a form for entering values.
    /// 
    /// This is implemented in the Custom form, do not use this class directly.
    /// </summary>
    public partial class BaseEnterValue : System.Windows.Forms.Form
    {
        // Properties belonging to the form
        private bool numberOnly;

        /// <summary>
        /// Constructs an enter value form.
        /// </summary>
        /// <param name="title">An optional title to display.</param>
        /// <param name="message">An optional message to display.</param>
        /// <param name="defaultValue">An optinoal default value.</param>
        /// <param name="numberOnly">Enforce a number input only.</param>
        /// <returns>A BaseEnterValue form.</returns>
        public BaseEnterValue(string title = "", string message = "", string defaultValue = "", bool numberOnly = false)
        {
            // Initialize the form, set the icon
            InitializeComponent();
            geeWiz.Utilities.FileUtils.SetFormIcon(this);

            // Set default values and outcomes
            this.textBox.Text = defaultValue;
            this.DialogResult = DialogResult.Cancel;
            this.numberOnly = numberOnly;

            // Default title and message, then set in the form
            if (title == "") { title = "Enter a value"; }
            if (message == "") { message = "Enter a value below:"; }
            this.Text = title;
            labelTooltip.Text = message;
        }

        /// <summary>
        /// Event handler when OK button is clicked.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Tag = this.textBox.Text;
            this.Close();
        }

        /// <summary>
        /// Event handler when Cancel button is clicked.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Tag = null;
            this.Close();
        }

        /// <summary>
        /// Event handler when a character would be entered into the textbox.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If we are numbers only, and the character isn't numerically valid
            if (this.numberOnly && !CharIsNumericallyValid(e.KeyChar))
            {
                // Handle the key press (it doesn't happen)
                e.Handled = true;
            }
        }

        /// <summary>
        /// Return it a character is numerically acceptable.
        /// </summary>
        /// <param name="keyChar"">The entered key.</param>
        /// <returns>A boolean.</returns>
        private bool CharIsNumericallyValid(char keyChar)
        {
            // Catch if the character is a control or digit
            if (char.IsControl(keyChar) || char.IsDigit(keyChar))
            {
                return true;
            }

            // Catch if character is a decimal
            if (keyChar == '.')
            {
                // Tracker for decimals
                int count = 0;

                // Uptick for each decimal found
                foreach (char c in this.textBox.Text)
                {
                    if (c == '.') { count++; }
                }

                // Return if no decimals yet
                return count < 1;
            }

            // Otherwise not valid
            return false;
        }
    }
}
