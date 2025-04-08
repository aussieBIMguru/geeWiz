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
        private bool MultiSelect;
        private List<FormPair> ItemsOriginal;
        private List<FormPair> ItemsShown;
        private string FilterString;

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
        public BaseListView(List<string> keys, List<object> values, string title, bool multiSelect = true)
        {
            // Initialize the form, set the icon
            InitializeComponent();
            geeWiz.Utilities.File_Utils.SetFormIcon(this);

            // Set title
            this.Text = title;

            // Create the key and value pairs
            this.ItemsOriginal = Utilities.CombineAsFormPairs(keys, values);
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

            // Call load objects function
            LoadShownItems();
        }

        #endregion

        #region Load all shown items

        /// <summary>
        /// Load all items to be shown.
        /// </summary>
        /// <returns>Void (nothing).</returns>
        private void LoadShownItems()
        {
            // Collect filter value, force to lower case
            this.FilterString = textFilter.Text.ToLower();

            // Set shown items to those that pass the filter
            this.ItemsShown = this.ItemsOriginal
                .Where(i => PassesTextFilter(i.ItemKey))
                .ToList();

            // Reset the ListView
            listView.Clear();
            listView.Columns.Add("Key", 380);

            // For each item in shown items
            foreach (var item in this.ItemsShown)
            {
                var listViewItem = new ListViewItem(item.ItemKey);
                listViewItem.Checked = false;
                this.listView.Items.Add(listViewItem);
            }
        }

        #endregion

        #region Text filtering

        /// <summary>
        /// Event handler when text filter changes.
        /// </summary>
        /// <param name="sender"">The event sender.</param>
        /// <param name="e"">The event arguments.</param>
        /// <returns>Void (nothing).</returns>
        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
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
            if (this.MultiSelect)
            {
                // If more than 0 items checked
                if (listView.CheckedItems.Count > 0)
                {
                    // Get the values related to the checked items
                    this.Tag = listView.CheckedItems
                        .Cast<ListViewItem>()
                        .Select(i => this.ItemsShown[i.Index].ItemValue)
                        .ToList();

                    // Dialog result is OK
                    this.DialogResult = DialogResult.OK;
                }
            }
            // Single selection (+ selection)
            else
            {
                // If an item is selected
                if (listView.SelectedItems.Count > 0)
                {
                    // Get the value related to the selected item
                    int ind = listView.SelectedItems[0].Index;
                    this.Tag = this.ItemsShown[ind].ItemValue;

                    // Dialog result is OK
                    this.DialogResult = DialogResult.OK;
                }
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