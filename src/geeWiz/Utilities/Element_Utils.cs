// geeWiz
using geeWiz.Extensions;

// The class belongs to the geeWiz namespace
// using gEle = geeWiz.Utilities.Element_Utils
namespace geeWiz.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to managing elements.
    /// </summary>
    public static class Element_Utils
    {
        /// <summary>
        /// Removes all elements on secondary design options.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="elements">A list of elements.</param>
        /// <returns>A list of elements as T.</returns>
        public static List<T> ExcludeSecondaryOptionRelated<T>(List<T> elements)
        {
            // Return all elements that are not secondary
            return elements
                .Cast<Element>()
                .Where(e => !e.Ext_IsOnSecondaryDesignOption())
                .Cast<T>()
                .ToList();
        }

        /// <summary>
        /// Replaces all null elements in a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="elements">A list of elements.</param>
        /// <param name="T">The replacement to make.</param>
        /// <returns>A list of elements as T.</returns>
        public static List<T> ReplaceNulls<T>(this List<T> elements, T replaceWith)
        {
            // Replace all null elements
            return elements
                .Select(e => e is null ? replaceWith : e)
                .ToList();
        }
    }
}
