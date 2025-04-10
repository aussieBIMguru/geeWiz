// The base form will belong to the forms namespace
namespace geeWiz.Forms.Bases
{
    /// <summary>
    /// Standard class for showing a form for selecting from a dropdown (combobox).
    /// 
    /// This is implemented in the Custom form, do not use this class directly.
    /// </summary>
    public partial class BaseDropdown : System.Windows.Forms.Form
    {
        #region Class properties

        // Properties belonging to the form
        private List<string> keys;
        private List<object> values;
        private int defaultIndex;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a listview form.
        /// </summary>
        /// <param name="keys">Keys to display in the listview.</param>
        /// <param name="values">Values associated to the keys.</param>
        /// <param name="title">A title to display.</param>
        /// <param name="message">A message to display.</param>
        /// <param name="defaultIndex">An optional index to initialize at.</param>
        /// <returns>A BaseDropDown form.</returns>
        public BaseDropdown(List<string> keys, List<object> values, string title, string message, int defaultIndex = -1)
        {
            // Initialize form, set the icon
            InitializeComponent();
            geeWiz.Utilities.File_Utils.SetFormIcon(this);

            // Set the key and value properties
            this.keys = keys;
            this.values = values;
            this.defaultIndex = defaultIndex;

            // Default values if not given, then set title/tooltip
            this.Text = title;
            labelTooltip.Text = message;

            // By default, we assume cancellation occurs
            this.DialogResult = DialogResult.Cancel;
            this.Tag = null;

            // Load the keys into the combobox
            InitializeKeys(defaultIndex);
        }

        #endregion

        #region Initialize keys

        /// <summary>
        /// Load the keys into the dropdown.
        /// </summary>
        /// <param name="defaultIndex">An optional index to initialize at.</param>
        /// <returns>Void (nothing).</returns>
        private void InitializeKeys(int defaultIndex)
        {
            // Clear the combobox
            comboBox.Items.Clear();

            // Add each key into the combobox
            foreach (var key in keys)
            {
                comboBox.Items.Add(key);
            }

            // Assign default index if valid
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
            // Process the outcomes
            if (comboBox.SelectedIndex != -1)
            {
                // Get the selected item by index
                object selectedValue = values[comboBox.SelectedIndex];

                // Set the value and dialog result to OK
                this.DialogResult = DialogResult.OK;
                this.Tag = selectedValue;
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
