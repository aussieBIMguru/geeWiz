using System;
using System.Windows.Forms;

// Forms namespace
namespace geeWiz.Forms
{
    public partial class EnterValue : System.Windows.Forms.Form
    {
        // Property if we want numbers only
        private bool numberOnly;

        // Base form class
        public EnterValue(string title = "", string tooltip = "", string defaultValue = "", bool numberOnly = false)
        {
            // Initialize the form
            InitializeComponent();
            // Set default values and outcomes
            this.textBox.Text = defaultValue;
            this.DialogResult = DialogResult.Cancel;
            this.numberOnly = numberOnly;
            // Default values if not given, then set title/tooltip
            if (title == "") { title = "Enter a value"; }
            if (tooltip == "") { tooltip = "Enter a value below:"; }
            this.Text = title;
            labelTooltip.Text = tooltip;
        }

        // Event for when OK button is clicked
        private void btnSelect_Click(object sender, EventArgs e)
        {
            // Tag the object and store the result
            this.DialogResult = DialogResult.OK;
            this.Tag = this.textBox.Text;

            // Close the form
            this.Close();
        }

        // Event for when cancel button is clicked
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Cancel the form and return null
            this.Tag = null;
            this.Close();
        }

        // Event for when a character is entered
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control characters (like backspace) if numbers only
            if (this.numberOnly && !Char_Invalid(e.KeyChar, this.textBox.Text))
            {
                e.Handled = true; // Suppress the key press
            }
        }

        // Helper function to identify char suitable for text as numbers
        private bool Char_Invalid(char keyChar, string input)
        {
            // Count the dots
            int count = 0;
            foreach (char c in input)
            {
                if (c == '.') { count++; }
            }
            // If a control or digit character
            if (char.IsControl(keyChar) || char.IsDigit(keyChar))
            {
                return true;
            }
            // If the first decimal
            else if (keyChar == '.' && count < 1)
            {
                return true;
            }
            // Otherwise not valid
            else
            {
                return false;
            }
        }
    }
}
