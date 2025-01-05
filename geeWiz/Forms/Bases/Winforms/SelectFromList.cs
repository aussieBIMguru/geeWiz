using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace geeWiz.Forms
{
    // Main form class for select from list of items (key/value pairs)
    public partial class SelectFromList : System.Windows.Forms.Form
    {
        // Establish keys and values
        private List<string> keys;
        private List<object> values;
        private bool multiSelect;
        private List<int> filteredIndices;

        public SelectFromList(List<string> keys, List<object> values, string title = "", bool multi_select = true)
        {
            InitializeComponent();
            // Set the key and value properties
            this.keys = keys;
            this.values = values;
            this.multiSelect = multi_select;
            this.listView.MultiSelect = multi_select;
            this.listView.CheckBoxes = multi_select;
            filteredIndices = new List<int>();
            // Set title
            if (title == "")
            {
                title = multi_select ? "Select objects from list" : "Select object from list";
            }
            this.Text = title;
            // By default the form will be assumed as cancelled
            this.DialogResult = DialogResult.Cancel;
            // Specify multi select
            listView.MultiSelect = multi_select;
            this.btnCheckAll.Enabled = multi_select;
            this.btnUncheckAll.Enabled = multi_select;
            // Call load objects function
            LoadObjects();
        }

        // Load the form objects
        private void LoadObjects()
        {
            // Clear all items
            listView.Items.Clear();
            filteredIndices.Clear();
            listView.Columns.Add("Key", 380);

            // For each key...
            for (int i = 0; i < keys.Count; i++)
            {
                // Make an unchecked listviewitem object
                var item = new ListViewItem(keys[i]) { Checked = false };
                // Display its value as a string (if possible)
                item.SubItems.Add(values[i]?.ToString() ?? "");
                listView.Items.Add(item);
                filteredIndices.Add(i);
            }
        }

        // Triggers whenever the filter is changed by the user
        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            // Collect text value and force to lower
            string filter = textFilter.Text.ToLower();
            // Clear the list view and re-establish the column view
            listView.Clear();
            filteredIndices.Clear();

            // If the filter is blank, load all objects again
            if (string.IsNullOrEmpty(filter))
            {
                LoadObjects();
            }
            // Otherwise, only show the keys that satisfy the filter
            else
            {
                listView.Columns.Add("Key", 380);

                for (int i = 0; i < keys.Count; i++)
                {
                    if (keys[i].ToLower().Contains(filter)) // Check if key matches the filter
                    {
                        var item = new ListViewItem(keys[i]) { Checked = false };
                        item.SubItems.Add(values[i]?.ToString() ?? ""); // Display the value as a string
                        listView.Items.Add(item);
                        filteredIndices.Add(i);
                    }
                }
            }
        }

        // Runs when user checks all
        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = true;
                item.Selected = true;
            }
        }

        // Runs when user unchecks all
        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = false;
                item.Selected = false;
            }
        }

        // Runs when user selects the OK/select button
        private void btnSelect_Click(object sender, EventArgs e)
        {
            // Multi select outcome
            if (multiSelect)
            {
                var selectedValues = listView.CheckedItems.Cast<ListViewItem>()
                .Select(item => values[filteredIndices[listView.Items.IndexOf(item)]])
                .ToList();

                if (selectedValues.Count > 0)
                {
                    this.DialogResult = DialogResult.OK;

                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }

                this.Tag = selectedValues;
                this.Close();
            }
            // One item selected outcome
            else if (listView.SelectedItems.Count > 0)
            {
                var selectedItem = listView.SelectedItems[0];
                var selectedValue = values[filteredIndices[listView.Items.IndexOf(selectedItem)]];
                this.DialogResult = DialogResult.OK;
                this.Tag = selectedValue;
                this.Close();
            }
            // No item selected outcome
            else
            {
                this.Tag = null;
                this.Close();
            }
        }

        // Runs when the user cancels the form using the cancel button
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Cancel the form and return null
            this.Tag = null;
            this.Close();
        }

    }
}