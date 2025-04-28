// geeWiz
using gDat = geeWiz.Utilities.Data_Utils;

// The base form will belong to the forms namespace
namespace geeWiz.Forms.Bases
{
    /// <summary>
    /// Standard class for showing a form for selecting from a listview.
    /// Includes a text filter to filter keys from the list.
    /// Form leverages index filtering to retrieve final values.
    /// This is implemented in the Custom form, do not use this class directly.
    /// </summary>
    /// <typeparam name="T">The type of object being stored.</typeparam>
    public partial class BaseListView<T> : System.Windows.Forms.Form
    {
        #region Class properties

        // Properties belonging to the form
        private bool MultiSelect;
        private List<gDat.KeyedValue<T>> FormPairs;
        private string FilterString;
        private List<int> VisibleIndices;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a listview form.
        /// </summary>
        /// <param name="keys">Keys to display in the listview.</param>
        /// <param name="values">Values associated to the keys.</param>
        /// <param name="title">A title to display.</param>
        /// <param name="multiSelect">Allow selection of multiple keys.</param>
        /// <returns>A BaseListView form.</returns>
        public BaseListView(List<string> keys, List<T> values, string title, bool multiSelect = true)
        {
            // Initialize the form, set the icon
            InitializeComponent();
            geeWiz.Utilities.File_Utils.SetFormIcon(this);

            // Set title
            this.Text = title;

            // Create the key and value pairs
            this.FormPairs = gDat.CombineAsFormPairs(keys, values);
            this.FilterString = "";

            // Establish multi selection behavior
            this.MultiSelect = multiSelect;
            this.listView.MultiSelect = multiSelect;
            this.listView.CheckBoxes = multiSelect;
            this.btnCheckAll.Enabled = multiSelect;
            this.btnUncheckAll.Enabled = multiSelect;

            // By default, we assume cancellation occurs
            this.Tag = null;
            this.DialogResult = DialogResult.Cancel;

            // Set initial indices
            this.VisibleIndices = new List<int>();
            for (int i = 0; i < keys.Count; i++)
            {
                VisibleIndices.Add(i);
            }

            // Call load objects function
            LoadShownItems();
        }

        #endregion

        #region Load / reload items

        /// <summary>
        /// Load all items to be shown.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        private void LoadShownItems()
        {
            // Reset the ListView
            listView.Clear();
            listView.Columns.Add("Key", 380);

            // For each form pair...
            foreach (var pair in this.FormPairs)
            {
                // If visible...
                if (pair.Visible)
                {
                    // Add the key to the list view
                    var listViewItem = new ListViewItem(pair.ItemKey);
                    listViewItem.Checked = pair.Checked;
                    this.listView.Items.Add(listViewItem);
                }
            }
        }

        #endregion

        #region Visibility management

        /// <summary>
        /// Event handler when text filter changes.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            // Store the checked status
            UpdateCheckedValues();

            // Pass the filter value
            this.FilterString = textFilter.Text.ToLower();

            // Clear visible indices
            this.VisibleIndices = new List<int>();

            // For each formpair...
            foreach (var pair in this.FormPairs)
            {
                // Check if it passes the filter, set visibility
                var passesFilter = PassesTextFilter(pair.ItemKey);
                pair.Visible = passesFilter;

                // Add its index if it passes the filter
                if (passesFilter) { this.VisibleIndices.Add(pair.ItemIndex); }
            }
            
            // Call the load items method
            LoadShownItems();
        }

        /// <summary>
        /// Checks if a value passes the current text filter.
        /// </summary>
        /// <param name="text">The value to check.</param>
        /// <returns>A boolean.</returns>
        private bool PassesTextFilter(string text)
        {
            // True if filter is empty
            if (this.FilterString.IsNullOrEmpty())
            {
                return true;
            }

            // Otherwise return if it contains the string
            return text.ToLower().Contains(this.FilterString);
        }

        /// <summary>
        /// Update the checked status of visible items.
        /// </summary>
        private void UpdateCheckedValues()
        {
            // For each visible indice...
            for (int i = 0; i < this.listView.Items.Count; i++)
            {
                // Get the pair index and listviewitem
                var pairIndex = this.VisibleIndices[i];
                var listViewItem = this.listView.Items[i];

                // Set the corresponding formpair check
                this.FormPairs[pairIndex].Checked = listViewItem.Checked;
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
            // Store the checked status
            UpdateCheckedValues();

            // Multi-select (+ selection)
            if (this.MultiSelect)
            {
                // Store the checked items
                var checkedValues = FormPairs
                    .Where(f => f.Checked)
                    .Select(f => f.ItemValue)
                    .ToList();

                // If we have values, OK and tag
                if (checkedValues.Count > 0)
                {
                    this.Tag = checkedValues;
                    this.DialogResult = DialogResult.OK;
                }
            }
            // Single selection (+ selection)
            else if (listView.SelectedItems.Count > 0)
            {
                // Get the selected index, then the value index
                int selectedIndex = listView.SelectedItems[0].Index;
                int valueIndex = this.VisibleIndices[selectedIndex];

                // OK and tag
                this.Tag = this.FormPairs[valueIndex].ItemValue;
                this.DialogResult = DialogResult.OK;
            }

            // Close the form
            this.Close();
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
            this.Close();
        }

        #endregion
    }
}