// Revit API
using Autodesk.Revit.UI;

// The class belongs to the geeWiz namespace
// using gDat = geeWiz.Utilities.Data_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to managing data structures.
    /// </summary>
    public static class Data_Utils
    {
        #region KeyedItem class

        /// <summary>
        /// A class for holding form items, with various data in parallel.
        /// </summary>
        public class KeyedItem
        {
            // These properties hold an item and group object
            public object ItemValue { get; set; }
            public object GroupValue { get; set; }

            // These properties hold the key values for the objects
            public string ItemKey { get; set; }
            public string GroupKey { get; set; }

            // These properties allow for the carriage of indices in parallel
            public int ItemIndex { get; set; }
            public int GroupIndex { get; set; }

            // This is intended as a unique index string
            public string IndexKey { get; set; }

            // Behavior in forms
            public bool Checked { get; set; }
            public bool Visible { get; set; }

            /// <summary>
            /// Default constructor
            /// </summary>
            public KeyedItem()
            {
                // Initialize form behavior
                this.Checked = false;
                this.Visible = true;
            }

            /// <summary>
            /// Construct using required data.
            /// </summary>
            /// <param name="itemValue"></param>
            /// <param name="itemKey"></param>
            /// <param name="groupValue"></param>
            /// <param name="groupKey"></param>
            public KeyedItem(
                object itemValue, string itemKey, int itemIndex,
                object groupValue, string groupKey, int groupIndex)
            {
                // Pass the properties
                this.ItemValue = itemValue;
                this.ItemKey = itemKey;
                this.ItemIndex = itemIndex;
                this.GroupValue = groupValue;
                this.GroupKey = groupKey;
                this.GroupIndex = groupIndex;

                // Set the index key
                this.IndexKey = $"{groupIndex}\t{itemIndex}";

                // Initialize form behavior
                this.Checked = false;
                this.Visible = true;
            }
        }

        #endregion

        #region KeyedMatrix class

        /// <summary>
        /// A class for holding keys aligned with a matrix of FormItems.
        /// </summary>
        public class KeyedMatrix
        {
            // Properties
            public List<string> GroupKeys { get; set; }
            public List<List<KeyedItem>> Matrix { get; set; }
            public List<KeyedItem> UnkeyedItems { get; set; }
            public bool UnkeyedItemsFound { get; set; }

            /// <summary>
            /// Construct a keyed matrix from keys and a list of FormItems.
            /// </summary>
            /// <param name="keys"></param>
            /// <param name="formItems"></param>
            /// <param name="sortKeys"></param>
            public KeyedMatrix(List<string> keys, List<KeyedItem> formItems, bool sortKeys = true)
            {
                // Cancel if no keys
                if (keys.Count == 0) { return; }

                // Optional sort of keys
                if (sortKeys) { keys.Sort(); }

                // New matrix
                var matrixOut = new List<List<KeyedItem>>();
                var unKeyedItems = new List<KeyedItem>();

                // Add a list for each key
                for (int i = 0; i < keys.Count; i++)
                {
                    matrixOut.Add(new List<KeyedItem>());
                }

                // For each form item...
                foreach (var item in formItems)
                {
                    // If the group key exists...
                    if (keys.Contains(item.GroupKey))
                    {
                        // Get the group and item index
                        int groupIndex = keys.IndexOf(item.GroupKey);
                        int itemIndex = matrixOut[groupIndex].Count;

                        // Set the items, add to the matrix
                        item.ItemIndex = itemIndex;
                        item.GroupIndex = groupIndex;
                        matrixOut[groupIndex].Add(item);
                    }
                    else
                    {
                        // Otherwise, it is unkeyed
                        unKeyedItems.Add(item);
                    }
                }

                // Set the properties
                this.GroupKeys = keys;
                this.Matrix = matrixOut;
                this.UnkeyedItems = unKeyedItems;
                this.UnkeyedItemsFound = unKeyedItems.Count > 0;
            }

            /// <summary>
            /// Updates the ItemIndex property of stored items based on order.
            /// </summary>
            public void RefreshItemKeys()
            {
                // Initialize the item index
                int itemIndex;

                // For each list of items
                foreach (var items in this.Matrix)
                {
                    // Index is zero
                    itemIndex = 0;

                    // Store the index for each item again
                    foreach (var item in items)
                    {
                        item.ItemIndex++;
                        itemIndex++;
                    }
                }
            }

            /// <summary>
            /// Checks if an item is available in the matrix.
            /// </summary>
            /// <param name="item">The item to check using.</param>
            /// <returns>A boolean.</returns>
            public bool ItemIsAccessible(KeyedItem item)
            {
                if (this.GroupKeys.Count > item.GroupIndex)
                {
                    return this.Matrix[item.GroupIndex].Count > item.ItemIndex;
                }
                return false;
            }

            /// <summary>
            /// Checks if an item is available in the matrix.
            /// </summary>
            /// <param name="groupIndex">The group index to check.</param>
            /// <param name="itemIndex">The item index to check in that group.</param>
            /// <returns>A boolean.</returns>
            public bool ItemIsAccessible(int groupIndex, int itemIndex)
            {
                if (this.GroupKeys.Count > groupIndex)
                {
                    return this.Matrix[groupIndex].Count > itemIndex;
                }
                return false;
            }

            /// <summary>
            /// Updates the visibility of a contained item based on a copy.
            /// </summary>
            /// <param name="item">The item to update.</param>
            /// <param name="show">Show the item.</param>
            /// <returns>A Result.</returns>
            public Result SetItemVisibility(KeyedItem item, bool show = true)
            {
                if (ItemIsAccessible(item))
                {
                    this.Matrix[item.GroupIndex][item.ItemIndex].Visible = show;
                    return Result.Succeeded;
                }
                return Result.Failed;
            }

            /// <summary>
            /// Updates the checked status of a contained item based on a copy.
            /// </summary>
            /// <param name="item">The item to update.</param>
            /// <param name="check">Check the item.</param>
            /// <returns>A Result</returns>
            public Result SetItemChecked(KeyedItem item, bool check = true)
            {
                if (ItemIsAccessible(item))
                {
                    this.Matrix[item.GroupIndex][item.ItemIndex].Checked = check;
                    return Result.Succeeded;
                }
                return Result.Failed;
            }

            /// <summary>
            /// Returns the items at a specified key.
            /// </summary>
            /// <param name="key">The key to return the items for.</param>
            /// <returns>A list of FormItems</returns>
            public List<KeyedItem> GetGroupByKey(string key)
            {
                if (this.GroupKeys.Contains(key))
                {
                    return this.Matrix[this.GroupKeys.IndexOf(key)];
                }
                return null;
            }
        }

        #endregion

        #region KeyedValue class

        /// <summary>
        /// A class for holding a key value pair.
        /// </summary>
        public class KeyedValue
        {
            // These properties relate to the item
            public object ItemValue { get; set; }
            public string ItemKey { get; set; }
            public int ItemIndex { get; set; }

            // Form behavior
            public bool Checked { get; set; }
            public bool Visible { get; set; }

            /// <summary>
            /// Default constructor
            /// </summary>
            public KeyedValue()
            {
                this.ItemValue = null;
                this.ItemKey = null;
                this.ItemIndex = -1;
                this.Visible = true;
                this.Checked = false;
            }

            /// <summary>
            /// Construct using required data.
            /// </summary>
            /// <param name="itemValue">The object to store.</param>
            /// <param name="itemKey">The key for the item.</param>
            public KeyedValue(object itemValue, string itemKey)
            {
                this.ItemValue = itemValue;
                this.ItemKey = itemKey;
                this.ItemIndex = -1;
                this.Visible = true;
                this.Checked = false;
            }

            /// <summary>
            /// Construct using required data.
            /// </summary>
            /// <param name="itemValue">The object to store.</param>
            /// <param name="itemKey">The key for the item.</param>
            /// <param name="itemIndex">The index to store the item at.</param>
            public KeyedValue(object itemValue, string itemKey, int itemIndex)
            {
                this.ItemValue = itemValue;
                this.ItemKey = itemKey;
                this.ItemIndex = itemIndex;
                this.Visible = true;
                this.Checked = false;
            }
        }

        #endregion
    }
}