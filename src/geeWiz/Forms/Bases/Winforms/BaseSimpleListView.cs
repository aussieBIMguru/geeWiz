// The base form will belong to the forms namespace
namespace geeWiz.Forms.Bases
{
    /// <summary>
    /// Standard class for showing a form for selecting from a listview.
    /// This is implemented in the Custom form, do not use this class directly.
    /// </summary>
    /// <typeparam name="T">The type of object being stored.</typeparam>
    public partial class BaseSimpleListView<T> : System.Windows.Forms.Form
    {
        #region Class properties

        // Properties belonging to the form
        private bool MultiSelect;
        private List<string> Keys;
        private List<T> Values;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a listview form.
        /// </summary>
        /// <param name="keys">Keys to display in the listview.</param>
        /// <param name="values">Values associated to the keys.</param>
        /// <param name="title">A title to display.</param>
        /// <param name="multiSelect">Allow selection of multiple keys.</param>
        /// <returns>A BaseSimpleListView form.</returns>
        public BaseSimpleListView(List<string> keys, List<T> values, string title, bool multiSelect = true)
        {
            // Initialize the form, set the icon
            InitializeComponent();
            geeWiz.Utilities.File_Utils.SetFormIcon(this);

            // Set title
            this.Text = title;

            // Set the keys and values
            this.Keys = keys;
            this.Values = values;

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
            // Reset the ListView
            listView.Clear();
            listView.Columns.Add("Key", 380);

            // For each item in shown items
            foreach (var key in this.Keys)
            {
                var listViewItem = new ListViewItem(key);
                listViewItem.Checked = false;
                this.listView.Items.Add(listViewItem);
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
            // Multiple selection
            if (this.MultiSelect && listView.CheckedItems.Count > 0)
            {
                // Set the selected values
                this.Tag = listView.CheckedItems
                    .Cast<ListViewItem>()
                    .Select(i => this.Values[i.Index])
                    .ToList();

                // Dialog result is OK
                this.DialogResult = DialogResult.OK;
            }
            // Single selection
            else if (listView.SelectedItems.Count > 0)
            {
                // Set the selected value
                int ind = listView.SelectedItems[0].Index;
                this.Tag = this.Values[ind];

                // Dialog result is OK
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