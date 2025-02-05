// The base form will belong to the forms namespace
namespace geeWiz.Forms
{
    #region Class summary
    /// <summary>
    /// Standard class for showing a form for selecting from a listview.
    /// 
    /// Includes a text filter to filter keys from the list.
    /// 
    /// Form leverages index filtering to retrieve final values.
    /// 
    /// This is implemented in the Custom form, do not use this class directly.
    /// </summary>
#endregion

    public partial class BaseListView : System.Windows.Forms.Form
    {
        #region Class properties

        // Properties belonging to the form
        private List<string> keys;
        private List<object> values;
        private bool multiSelect;
        private List<int> filteredIndices;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a listview form.
        /// </summary>
        /// <param name="keys">Keys to display in the listview.</param>
        /// <param name="values">Values associated to the keys.</param>
        /// <param name="title">An optional title to display.</param>
        /// <param name="multiSelect">Allow selection of multiple keys.</param>
        /// <returns>A BaseListView form.</returns>
        public BaseListView(List<string> keys, List<object> values, string title = "", bool multiSelect = true)
        {
            // Initialize the form, set the icon
            InitializeComponent();
            geeWiz.Utilities.File_Utils.SetFormIcon(this);

            // Set the key and value properties
            this.keys = keys;
            this.values = values;

            // Establish multi selection behavior
            this.multiSelect = multiSelect;
            this.listView.MultiSelect = multiSelect;
            this.listView.CheckBoxes = multiSelect;
            this.btnCheckAll.Enabled = multiSelect;
            this.btnUncheckAll.Enabled = multiSelect;

            // Filtered indices to process with the text filter
            filteredIndices = new List<int>();
            
            // Set default title
            if (title == "")
            {
                title = multiSelect ? "Select objects from list" : "Select object from list";
            }
            this.Text = title;

            // By default, we assume cancellation occurs
            this.Tag = null;
            this.DialogResult = DialogResult.Cancel;

            // Call initial load objects function
            ResetKeys();
        }

        #endregion

        #region Reset list view

        /// <summary>
        /// Load all keys into the listview.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        private void ResetKeys()
        {
            // Clear all items and filtered indices
            listView.Clear();
            filteredIndices.Clear();

            // Re-establish the list view
            listView.Columns.Add("Key", 380);

            // For each relevant key
            for (int i = 0; i < keys.Count; i++)
            {
                // Make an unchecked listviewitem object
                var item = new ListViewItem(keys[i]);
                item.Checked = false;
                item.SubItems.Add(values[i]?.ToString() ?? "");

                // Add the item and indice
                listView.Items.Add(item);
                filteredIndices.Add(i);
            }
        }

        #endregion

        #region Text filter changed

        /// <summary>
        /// Event handler when text filter changes.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            // Collect filter value, force to lower case
            string filter = textFilter.Text.ToLower();

            // If the filter is blank, reset to all keys (do not continue further)
            if (string.IsNullOrEmpty(filter)) { ResetKeys(); return; }

            // Clear all items and filtered indices
            listView.Clear();
            filteredIndices.Clear();

            // Re-establish the list view
            listView.Columns.Add("Key", 380);

            // For each relevant key
            for (int i = 0; i < keys.Count; i++)
            {
                // If that key contains the filter string
                if (keys[i].ToLower().Contains(filter))
                {
                    // Make an unchecked listviewitem object
                    var item = new ListViewItem(keys[i]);
                    item.Checked = false;
                    item.SubItems.Add(values[i]?.ToString() ?? "");

                    // Add the item and indice
                    listView.Items.Add(item);
                    filteredIndices.Add(i);
                }
            }
        }

        #endregion

        #region Click check all button

        /// <summary>
        /// Event handler when check all button is clicked.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = true;
                item.Selected = true;
            }
        }

        #endregion

        #region Click uncheck all button

        /// <summary>
        /// Event handler when uncheck all button is clicked.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = false;
                item.Selected = false;
            }
        }

        #endregion

        #region Click OK button

        /// <summary>
        /// Event handler when OK button is clicked.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            // Multi-select (+ selection)
            if (multiSelect)
            {
                // Null list by default
                List<object> checkedValues = null;

                // Get checked items (if any, taking filtering into consideration)
                if (listView.CheckedItems.Count > 0)
                {
                    checkedValues = listView.CheckedItems.Cast<ListViewItem>()
                        .Select(item => values[filteredIndices[listView.Items.IndexOf(item)]])
                        .ToList();
                    this.DialogResult = DialogResult.OK;
                }
                
                // Apply objects to tag, close form
                this.Tag = checkedValues;
                this.Close();
            }
            // Single selection (+ selection)
            else
            {
                // Null object by default
                object selectedValue = null;

                // Get selected item (if any, taking filtering into consideration)
                if (listView.SelectedItems.Count > 0)
                {
                    var selectedItem = listView.SelectedItems[0];
                    selectedValue = values[filteredIndices[listView.Items.IndexOf(selectedItem)]];
                    this.DialogResult = DialogResult.OK;
                }

                // Apply object to tag, close form
                this.Tag = selectedValue;
                this.Close();
            }
        }

        #endregion

        #region Click Cancel button

        /// <summary>
        /// Event handler when cancel button is clicked.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Tag = null;
            this.Close();
        }

        #endregion
    }
}