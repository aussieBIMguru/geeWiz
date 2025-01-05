using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace geeWiz.Forms
{
    // Main form class for select from list of items (key/value pairs)
    public partial class SelectOneItem : System.Windows.Forms.Form
    {
        // Establish keys and values
        private List<string> keys;
        private List<object> values;
        private int defaultIndex;

        public SelectOneItem(List<string> keys, List<object> values, string title = "", string tooltip = "", int defaultIndex = -1)
        {
            // Unequal keys and values
            if (keys.Count != values.Count)
            {
                throw new ArgumentException("Keys and values must have the same count.");
            }
            // Initialize form
            InitializeComponent();
            // Set the key and value properties
            this.keys = keys;
            this.values = values;
            this.defaultIndex = defaultIndex;
            this.DialogResult = DialogResult.Cancel;
            // Default values if not given, then set title/tooltip
            if (title == "") { title = "Select item from list"; }
            if (tooltip == "") { tooltip = "Select an item from the dropdown:"; }
            this.Text = title;
            labelTooltip.Text = tooltip;
            // Load the keys into the combobox
            LoadKeysIntoComboBox(defaultIndex);
        }

        // Initial loading of combo box values
        private void LoadKeysIntoComboBox(int defaultIndex)
        {
            // Clear the combobox
            comboBox.Items.Clear();
            // Add each key into the combobox
            foreach (var key in keys)
            {
                comboBox.Items.Add(key);
            }
            // If the index is valid, assign it
            if (defaultIndex >= 0 && defaultIndex < comboBox.Items.Count)
            {
                comboBox.SelectedIndex = defaultIndex;
            }
            // Otherwise, select value 1
            else
            {
                try
                {
                    comboBox.SelectedIndex = 0;
                }
                catch
                {
                    comboBox.SelectedIndex = -1;
                }
            }
        }

        // Runs when user selects the OK/select button
        private void btnSelect_Click(object sender, EventArgs e)
        {
            // If we actually selected an object from the list...
            if (comboBox.SelectedIndex != -1)
            {
                // Get the selected item by index
                int selectedIndex = comboBox.SelectedIndex;
                object selectedValue = values[selectedIndex]; // Get value based on selected index
                // Tag the object and store the result
                this.DialogResult = DialogResult.OK;
                this.Tag = selectedValue;
            }
            else
            {
                // Flag that we cancelled, return null as tag
                this.DialogResult = DialogResult.Cancel;
                this.Tag = null;
            }
            // Close the form
            this.Close();
        }

        // Runs when user selects the cancel button
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Cancel the form and return null
            this.Tag = null;
            this.Close();
        }
    }
}
