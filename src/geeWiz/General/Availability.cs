// Revit API
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

// The class belongs to the root namespace
// using gAva = geeWiz.Availability.AvailabilityNames;
namespace geeWiz.General.Availability
{
    /// <summary>
    /// Provides a shorter means of referencing availability class names.
    /// </summary>
    public static class AvailabilityNames
    {
        /// <summary>
        /// The full name of the availability class.
        /// </summary>
        public static readonly string Disabled = typeof(Availability_Disabled).FullName;

        /// <summary>
        /// The full name of the availability class.
        /// </summary>
        public static readonly string ZeroDoc = typeof(Availability_ZeroDoc).FullName;

        /// <summary>
        /// The full name of the availability class.
        /// </summary>
        public static readonly string Document = typeof(Availability_Document).FullName;

        /// <summary>
        /// The full name of the availability class.
        /// </summary>
        public static readonly string Project = typeof(Availability_Project).FullName;

        /// <summary>
        /// The full name of the availability class.
        /// </summary>
        public static readonly string Family = typeof(Availability_Family).FullName;

        /// <summary>
        /// The full name of the availability class.
        /// </summary>
        public static readonly string Workshared = typeof(Availability_Workshared).FullName;

        /// <summary>
        /// The full name of the availability class.
        /// </summary>
        public static readonly string Selection = typeof(Availability_Selection).FullName;

        /// <summary>
        /// The full name of the availability class.
        /// </summary>
        public static readonly string ActiveViewSchedule = typeof(Availability_ActiveViewSchedule).FullName;

        /// <summary>
        /// The full name of the availability class.
        /// </summary>
        public static readonly string SelectionIncludesSheets = typeof(Availability_SelectionIncludesSheets).FullName;

        /// <summary>
        /// The full name of the availability class.
        /// </summary>
        public static readonly string SelectionOnlySheets = typeof(Availability_SelectionOnlySheets).FullName;
    }

    /// <summary>
    /// This class is used by commands to check if they are available in the current context.
    /// This availability tells a command to always be unavailable.
    /// </summary>
    public class Availability_Disabled : IExternalCommandAvailability
    {
        /// <summary>
        /// Returns if a Command using this is available.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="categories">Categories of selected elements.</param>
        /// <returns>A Boolean indicating if the command is available.</returns>
        public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
        {
            return false;
        }
    }

    /// <summary>
    /// This class is used by commands to check if they are available in the current context.
    /// This availability tells a command to always be available.
    /// </summary>
    public class Availability_ZeroDoc : IExternalCommandAvailability
    {
        /// <summary>
        /// Returns if a Command using this is available.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="categories">Categories of selected elements.</param>
        /// <returns>A Boolean indicating if the command is available.</returns>
        public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
        {
            return true;
        }
    }

    /// <summary>
    /// This class is used by commands to check if they are available in the current context.
    /// This availability tells a command to be available if there is an active Document.
    /// </summary>
    public class Availability_Document : IExternalCommandAvailability
    {
        /// <summary>
        /// Returns if a Command using this is available.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="categories">Categories of selected elements.</param>
        /// <returns>A Boolean indicating if the command is available.</returns>
        public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
        {
            return uiApp.ActiveUIDocument is not null;
        }
    }

    /// <summary>
    /// This class is used by commands to check if they are available in the current context.
    /// This availability tells a command to be available if there is an active Project Document.
    /// </summary>
    public class Availability_Project : IExternalCommandAvailability
    {
        /// <summary>
        /// Returns if a Command using this is available.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="categories">Categories of selected elements.</param>
        /// <returns>A Boolean indicating if the command is available.</returns>
        public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
        {
            if (uiApp.ActiveUIDocument is UIDocument uiDoc)
            {
                return !uiDoc.Document.IsFamilyDocument;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// This class is used by commands to check if they are available in the current context.
    /// This availability tells a command to be available if there is an active Family Document.
    /// </summary>
    public class Availability_Family : IExternalCommandAvailability
    {
        /// <summary>
        /// Returns if a Command using this is available.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="categories">Categories of selected elements.</param>
        /// <returns>A Boolean indicating if the command is available.</returns>
        public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
        {
            if (uiApp.ActiveUIDocument is UIDocument uiDoc)
            {
                return uiDoc.Document.IsFamilyDocument;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// This class is used by commands to check if they are available in the current context.
    /// This availability tells a command to be available if there is an active Workshared Document.
    /// </summary>
    public class Availability_Workshared : IExternalCommandAvailability
    {
        /// <summary>
        /// Returns if a Command using this is available.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="categories">Categories of selected elements.</param>
        /// <returns>A Boolean indicating if the command is available.</returns>
        public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
        {
            if (uiApp.ActiveUIDocument is UIDocument uiDoc)
            {
                return uiDoc.Document.IsWorkshared;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// This class is used by commands to check if they are available in the current context.
    /// This availability tells a command to be available if there are Elements in the current selection.
    /// </summary>
    public class Availability_Selection : IExternalCommandAvailability
    {
        /// <summary>
        /// Returns if a Command using this is available.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="categories">Categories of selected elements.</param>
        /// <returns>A Boolean indicating if the command is available.</returns>
        public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
        {
            if (uiApp.ActiveUIDocument is not null)
            {
                return categories.Size > 0;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// This class is used by commands to check if they are available in the current context.
    /// This availability tells a command to be available if the Active View is a Schedule.
    /// </summary>
    public class Availability_ActiveViewSchedule : IExternalCommandAvailability
    {
        /// <summary>
        /// Returns if a Command using this is available.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="categories">Categories of selected elements.</param>
        /// <returns>A Boolean indicating if the command is available.</returns>
        public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
        {
            if (uiApp.ActiveUIDocument is UIDocument uiDoc)
            {
                return uiDoc.ActiveGraphicalView is ViewSchedule;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// This class is used by commands to check if they are available in the current context.
    /// This availability tells a command to be available if Sheets are in the current selection.
    /// </summary>
    public class Availability_SelectionIncludesSheets : IExternalCommandAvailability
    {
        /// <summary>
        /// Returns if a Command using this is available.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="categories">Categories of selected elements.</param>
        /// <returns>A Boolean indicating if the command is available.</returns>
        public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
        {
            if (uiApp.ActiveUIDocument is UIDocument uiDoc)
            {
                return categories.Contains(Category.GetCategory(uiDoc.Document, BuiltInCategory.OST_Sheets));
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// This class is used by commands to check if they are available in the current context.
    /// This availability tells a command to be available if only Sheets are in the current selection.
    /// </summary>
    public class Availability_SelectionOnlySheets : IExternalCommandAvailability
    {
        /// <summary>
        /// Returns if a Command using this is available.
        /// </summary>
        /// <param name="uiApp">The UIApplication.</param>
        /// <param name="categories">Categories of selected elements.</param>
        /// <returns>A Boolean indicating if the command is available.</returns>
        public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
        {
            if (uiApp.ActiveUIDocument is UIDocument uiDoc)
            {
                if (categories.Size > 1) { return false; } // More than one category not permitted
                return categories.Contains(Category.GetCategory(uiDoc.Document, BuiltInCategory.OST_Sheets));
            }
            else
            {
                return false;
            }
        }
    }
}