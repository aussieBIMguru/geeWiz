// Revit API
using Autodesk.Revit.UI;
// geeWiz
using gFrm = geeWiz.Forms;

// The class belongs to the geeWiz namespace
// using gDat = geeWiz.Utilities.Data_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// A class for holding form items, with various data in parallel.
    /// </summary>
    /// <typeparam name="T">The type of object being stored.</typeparam>
    public class KeyedItem<T>
    {
        /// <summary>
        /// The itemvalue.
        /// </summary>
        public T ItemValue { get; set; }

        /// <summary>
        /// The item's grouping value.
        /// </summary>
        public T GroupValue { get; set; }

        /// <summary>
        /// The key of the item.
        /// </summary>
        public string ItemKey { get; set; }

        /// <summary>
        /// The key of the item's group.
        /// </summary>
        public string GroupKey { get; set; }

        /// <summary>
        /// The position of the item in its broader sub-set.
        /// </summary>
        public int ItemIndex { get; set; }

        /// <summary>
        /// The position of the item's group in its broader sub-set.
        /// </summary>
        public int GroupIndex { get; set; }

        /// <summary>
        /// The key of the item's index.
        /// </summary>
        public string IndexKey { get; set; }

        /// <summary>
        /// Is the item checked.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Is the item visible.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public KeyedItem()
        {
            // Initialize form behavior
            this.Checked = false;
            this.Visible = true;
        }

        /// <summary>
        /// Constructor that takes all required values directly.
        /// </summary>
        /// <param name="itemValue">The item value.</param>
        /// <param name="itemKey">The item key.</param>
        /// <param name="itemIndex">The item index.</param>
        /// <param name="groupValue">The item's group value.</param>
        /// <param name="groupKey">The item's group key.</param>
        /// <param name="groupIndex">The item's group index.</param>
        public KeyedItem(
            T itemValue, string itemKey, int itemIndex,
            T groupValue, string groupKey, int groupIndex)
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

    /// <summary>
    /// A class for holding keys aligned with a matrix of KeyedItems.
    /// </summary>
    /// <typeparam name="T">The type of object being stored.</typeparam>
    public class KeyedMatrix<T>
    {
        /// <summary>
        /// The matrix of KeyedItems.
        /// </summary>
        public List<List<KeyedItem<T>>> Matrix { get; set; }

        /// <summary>
        /// A list of keys for the matrix groups.
        /// </summary>
        public List<string> GroupKeys { get; set; }

        /// <summary>
        /// A list of items that is not properly keyed.
        /// </summary>
        public List<KeyedItem<T>> UnkeyedItems { get; set; }
        
        /// <summary>
        /// Are there any items missing necessary keys.
        /// </summary>
        public bool UnkeyedItemsFound { get; set; }

        /// <summary>
        /// Construct a keyed matrix from grouping keys and a list of KeyedItems.
        /// </summary>
        /// <param name="keys">The group keys.</param>
        /// <param name="keyedItems">The list of KeyedItems.</param>
        /// <param name="sortKeys">Sort the groups by their keys.</param>
        public KeyedMatrix(List<string> keys, List<KeyedItem<T>> keyedItems, bool sortKeys = true)
        {
            // Cancel if no keys
            if (keys.Count == 0) { return; }

            // Optional sort of keys
            if (sortKeys) { keys.Sort(); }

            // New matrix
            var matrixOut = new List<List<KeyedItem<T>>>();
            var unKeyedItems = new List<KeyedItem<T>>();

            // Add a list for each key
            for (int i = 0; i < keys.Count; i++)
            {
                matrixOut.Add(new List<KeyedItem<T>>());
            }

            // For each form item...
            foreach (var item in keyedItems)
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
        /// Refreshes the indices of all items in the matrix.
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
        /// Checks if an item is available in the matrix by index.
        /// </summary>
        /// <param name="item">The item to check using.</param>
        /// <returns>A boolean.</returns>
        public bool ItemIsAccessible(KeyedItem<T> item)
        {
            if (this.GroupKeys.Count > item.GroupIndex)
            {
                return this.Matrix[item.GroupIndex].Count > item.ItemIndex;
            }
            return false;
        }

        /// <summary>
        /// Checks if an item is available in the matrix by index.
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
        /// Updates the visibility of a contained item based on a KeyedItem's indices.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <param name="show">Show the item.</param>
        /// <returns>A Result.</returns>
        public Result SetItemVisibility(KeyedItem<T> item, bool show = true)
        {
            if (ItemIsAccessible(item))
            {
                this.Matrix[item.GroupIndex][item.ItemIndex].Visible = show;
                return Result.Succeeded;
            }
            return Result.Failed;
        }

        /// <summary>
        /// Updates the checked status of a contained item based on a KeyedItem's indices.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <param name="check">Check the item.</param>
        /// <returns>A Result</returns>
        public Result SetItemChecked(KeyedItem<T> item, bool check = true)
        {
            if (ItemIsAccessible(item))
            {
                this.Matrix[item.GroupIndex][item.ItemIndex].Checked = check;
                return Result.Succeeded;
            }
            return Result.Failed;
        }

        /// <summary>
        /// Returns the items at a specified group key.
        /// </summary>
        /// <param name="key">The key to return the group's items for.</param>
        /// <returns>A list of KeyedItems</returns>
        public List<KeyedItem<T>> GetGroupByKey(string key)
        {
            if (this.GroupKeys.Contains(key))
            {
                return this.Matrix[this.GroupKeys.IndexOf(key)];
            }
            return null;
        }
    }

    /// <summary>
    /// A class for holding a key value pair with no grouping systems.
    /// </summary>
    /// <typeparam name="T">The type of object being stored.</typeparam>
    public class KeyedValue<T>
    {
        /// <summary>
        /// The item's value.
        /// </summary>
        public T ItemValue { get; set; }

        /// <summary>
        /// The item's key.
        /// </summary>
        public string ItemKey { get; set; }

        /// <summary>
        /// The item's index.
        /// </summary>
        public int ItemIndex { get; set; }

        /// <summary>
        /// If the item is checked.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// If the item is visible.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public KeyedValue()
        {
            this.ItemValue = default;
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
        public KeyedValue(T itemValue, string itemKey)
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
        public KeyedValue(T itemValue, string itemKey, int itemIndex)
        {
            this.ItemValue = itemValue;
            this.ItemKey = itemKey;
            this.ItemIndex = itemIndex;
            this.Visible = true;
            this.Checked = false;
        }
    }

    /// <summary>
    /// Static methods container related to Data containers.
    /// </summary>
    public static class Data_Utils
    {
        #region Data utilities

        /// <summary>
        /// Combines keys and values into KeyedValues.
        /// </summary>
        /// <typeparam name="T">The type of object being stored.</typeparam>
        /// <param name="values">Objects to add to the FormPair.</param>
        /// <param name="keys">The keys to connect to the FormPair.</param>
        /// <param name="showMessages">Show error messages.</param>
        /// <returns>A list of KeyedValues.</returns>
        public static List<KeyedValue<T>> CombineAsKeyedValues<T>(List<string> keys, List<T> values, bool showMessages = false)
        {
            // Catch if invalid outcomes
            if (keys is null || values is null
                || keys.Count != values.Count || keys.Count == 0)
            {
                if (showMessages)
                {
                    gFrm.Custom.Error("Invalid key/value pairing provided.\n\n" +
                        "This is typically due to an error in the code, or no objects were provided.");
                }

                return null;
            }
            
            // Empty list of form pairs
            var formPairs = new List<KeyedValue<T>>();

            // Construct the form pairs with indices
            for (int i = 0; i < keys.Count; i++)
            {
                formPairs.Add(new KeyedValue<T>(values[i], keys[i], i));
            }

            // Return the formpairs
            return formPairs;
        }

        /// <summary>
        /// Combines keys and values into KeyedValues of the Object type.
        /// </summary>
        /// <typeparam name="T">The type of object being stored.</typeparam>
        /// <param name="values">Objects to add to the FormPair.</param>
        /// <param name="keys">The keys to connect to the FormPair.</param>
        /// <param name="showMessages">Show error messages.</param>
        /// <returns>A list of KeyedValues of the Object type.</returns>
        public static List<KeyedValue<object>> CombineAsKeyedObjects<T>(List<string> keys, List<T> values, bool showMessages = false)
        {
            // Catch if invalid outcomes
            if (keys is null || values is null
                || keys.Count != values.Count || keys.Count == 0)
            {
                if (showMessages)
                {
                    gFrm.Custom.Error("Invalid key/value pairing provided.\n\n" +
                        "This is typically due to an error in the code, or no objects were provided.");
                }

                return null;
            }

            // Empty list of form pairs
            var formPairs = new List<KeyedValue<object>>();

            // Construct the form pairs with indices
            for (int i = 0; i < keys.Count; i++)
            {
                formPairs.Add(new KeyedValue<object>(values[i] as object, keys[i], i));
            }

            // Return the formpairs
            return formPairs;
        }

        /// <summary>
        /// Replaces all negative indices in a list of integers.
        /// </summary>
        /// <param name="integers">The integers to review.</param>
        /// <param name="replaceWith">What to replace them with.</param>
        /// <returns>A list of integers.</returns>
        public static List<int> Positize(List<int> integers, int replaceWith = 0)
        {
            // Replace all negative indices
            return integers
                .Select(i => i > -1 ? i : replaceWith)
                .ToList();
        }

        /// <summary>
        /// Gets an object by a key value against a parallel list of values.
        /// </summary>
        /// <typeparam name="T">The type of object being found.</typeparam>
        /// <param name="findKey">The Key to find.</param>
        /// <param name="values">The values to search through.</param>
        /// <param name="keys">The values to search through.</param>
        /// <returns>The object (if found).</returns>
        public static T FindItemAtKey<T>(string findKey, List<T> values, List<string> keys)
        {
            // If key exists...
            if (keys.Contains(findKey))
            {
                // Make sure index is lower than value count
                int ind = keys.IndexOf(findKey);

                // Return if inside the range of values
                if (ind < values.Count)
                {
                    return values[ind];
                }
            }

            // Otherwise, return the default type
            return default;
        }

        #endregion

        #region Dictionaries

        /// <summary>
        /// Returns a value based on a key from a dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary of keys/values to search.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="defaultValue">The value to return if no key is found.</param>
        /// <returns>The related tooltip, if found.</returns>
        public static string GetDictValue(Dictionary<string, string> dictionary, string key, string defaultValue = "Value not found.")
        {
            if (dictionary.TryGetValue(key, out string value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Constructs a dictionary safely versis Linq approach.
        /// </summary>
        /// <typeparam name="TSource">The type of the objects in the list.</typeparam>
        /// <typeparam name="TKey">The dictionary key type.</typeparam>
        /// <typeparam name="TValue">The dictionary value type.</typeparam>
        /// <param name="source">The source list.</param>
        /// <param name="keySelector">The functional key selection.</param>
        /// <param name="valueSelector">The functional value selection.</param>
        /// <param name="comparer">An optional string comparer.</param>
        /// <returns>A dictionary.</returns>
        public static Dictionary<TKey, TValue> QuickDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector,
            IEqualityComparer<TKey> comparer = null)
        {
            // Produce the base dictionary
            var dict = (comparer != null)
                ? new Dictionary<TKey, TValue>(comparer)
                : new Dictionary<TKey, TValue>();

            // For each list item, functionally key it to the dictionary
            foreach (var item in source)
            {
                dict[keySelector(item)] = valueSelector(item);
            }

            // Return the dictionary
            return dict;
        }

        #endregion
    }
}