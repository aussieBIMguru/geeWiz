// System
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
// geeWiz
using geeWiz.Extensions;
using KeyedValue = geeWiz.Utilities.Data_Utils.KeyedValue<object>;

// Associated to form bases namespace
namespace geeWiz.Forms.Bases
{
    /// <summary>
    /// Select items from list (Wpf)
    /// </summary>
    public partial class WpfListView : Window
    {
        #region Properties

        // Properties of form class
        private readonly List<KeyedValue> _objects = new List<KeyedValue>();
        private readonly ICollectionView _view;
        private readonly bool _multiSelect;
        private readonly bool _allowNoSelection;
        private bool _bulkUpdating = false;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="multiSelect"></param>
        /// <param name="title"></param>
        /// <param name="allowNoSelection"></param>
        public WpfListView(List<KeyedValue> objects, bool multiSelect = true, string title = null, bool allowNoSelection = false)
        {
            // Initialize the form
            InitializeComponent();

            // Set the objects and behaviors
            this._objects = objects;
            this._multiSelect = multiSelect;
            this._allowNoSelection = allowNoSelection;
            if (title.Ext_HasChars()) { this.Title = title; }

            // Set the view for the objects to allow text filtering
            this.ListBox.ItemsSource = this._objects;
            this._view = CollectionViewSource.GetDefaultView(this._objects);
            this._view.Filter = FilterByText;

            // Configure the behavior for multi or single select
            var templateName = Utilities.Wpf_SetListBoxMode(this._multiSelect, this.ListBox, this.CheckAllButton, this.UncheckAllButton);

            // Apply the related item template from shared styles
            if ((DataTemplate)FindResource(templateName) is DataTemplate template)
            {
                this.ListBox.ItemTemplate = template;
            }
        }

        #endregion

        #region Shift select / text filtering

        /// <summary>
        /// Runs whenever a checkbox is clicked on/off.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Make sure this wasn't triggered during an check or uncheck all
            if (this._bulkUpdating) { return; }

            // Run a shift click check
            Utilities.Wpf_ShiftClickProcess<object>(sender, this._multiSelect, this.ListBox);
        }

        /// <summary>
        /// Filter an object based on the text filter.
        /// </summary>
        /// <param name="obj">Object to check.</param>
        /// <returns></returns>
        private bool FilterByText(object obj)
        {
            // Get the filter value
            string filter = this.FilterTextBox.Text;

            // Empty filter = passes
            if (filter.Ext_HasNoChars())
            {
                return true;
            }

            // If there is a valid object, check it against the filter
            if (obj is KeyedValue keyedObject && keyedObject.ItemKey is not null)
            {
                return keyedObject.ItemKey.Contains(filter, StringComparison.OrdinalIgnoreCase);
            }

            // It fails if it can't be compared
            return false;
        }

        /// <summary>
        /// Runs when the text filter changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Refresh the view (applies the filter)
            this._view?.Refresh();
        }

        #endregion

        #region Check / uncheck all

        /// <summary>
        /// Check all visible objects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {
            // Flag that we are bulk updating (avoids checkbox trigger)
            this._bulkUpdating = true;

            // Check each underlying object
            foreach (var obj in this._view)
            {
                if (obj is KeyedValue keyedObject)
                {
                    keyedObject.Checked = true;
                }
            }

            // Refresh items and flag that bulk update is finished
            this.ListBox.Items.Refresh();
            this._bulkUpdating = false;
        }

        /// <summary>
        /// Unchecks all visible objects.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UncheckAll_Click(object sender, RoutedEventArgs e)
        {
            // Flag that we are bulk updating (avoids checkbox trigger)
            this._bulkUpdating = true;

            // Uncheck each underlying object
            foreach (var obj in this._view)
            {
                if (obj is KeyedValue keyedObject)
                {
                    keyedObject.Checked = false;
                }
            }

            // Refresh items and flag that bulk update is finished
            this.ListBox.Items.Refresh();
            this._bulkUpdating = false;
        }

        #endregion

        #region OK, Cancel, get items

        /// <summary>
        /// Gets the checked items or selected item.
        /// </summary>
        /// <returns></returns>
        public List<KeyedValue> GetChosenItems()
        {
            // Multiselect, return all checked
            if (this._multiSelect)
            {
                return this._objects
                    .Where(i => i.Checked)
                    .ToList();
            }
            // Otherwise, return selected item if available
            else
            {
                if (this.ListBox.SelectedItem is KeyedValue keyedObject)
                {
                    return new List<KeyedValue> { keyedObject };
                }

                return new List<KeyedValue>();
            }
        }

        /// <summary>
        /// Runs when OK is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure we have required output before finishing
            if (!this._allowNoSelection && GetChosenItems().Count == 0)
            {
                Custom.Error("No elements are selected.");
                return;
            }

            // Close the form with a true outcome
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Runs when Cancel is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the form with a false outcome
            this.DialogResult = false;
            this.Close();
        }

        #endregion
    }
}