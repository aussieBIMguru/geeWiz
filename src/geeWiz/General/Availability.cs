// Revit API
using Autodesk.Revit.UI;

// The class belongs to the root namespace
// using gAva = geeWiz.Availability.AvailabilityNames;
namespace geeWiz
{
    /// <summary>
    /// Methods of this class generally relate to availability/context.
    /// This is assigned to commands on startup.
    /// </summary>
    public static class Availability
    {
        #region AvailabilityNames class

        // Availability class prefix
        internal static string PATH_AVAILABILITY = "geeWiz.Availability";

        // Limit the options we can choose as availabilities using a class
        public static class AvailabilityNames
        {
            public static string Disabled {get { return $"{PATH_AVAILABILITY}+Disabled";} }
            public static string ZeroDoc { get { return $"{PATH_AVAILABILITY}+ZeroDoc"; } }
            public static string Document { get { return $"{PATH_AVAILABILITY}+Document"; } }
            public static string Project { get { return $"{PATH_AVAILABILITY}+Project"; } }
            public static string Family { get { return $"{PATH_AVAILABILITY}+Family"; } }
            public static string Workshared { get { return $"{PATH_AVAILABILITY}+Workshared"; } }
            public static string Selection { get { return $"{PATH_AVAILABILITY}+Selection"; } }
            public static string ActiveViewSchedule { get { return $"{PATH_AVAILABILITY}+ActiveViewSchedule"; } }
            public static string SelectionIncludesSheets { get { return $"{PATH_AVAILABILITY}+SelectionIncludesSheets"; } }
            public static string SelectionOnlySheets { get { return $"{PATH_AVAILABILITY}+SelectionOnlySheets"; } }
        }

        #endregion

        #region Availability classes

        // Command is disabled
        private class Disabled : IExternalCommandAvailability
        {
            public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
            {
                return false;
            }
        }

        // Command can only be ran even if a document is not opened
        private class ZeroDoc : IExternalCommandAvailability
        {
            public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
            {
                return true;
            }
        }

        // Command can only be ran in a document
        private class Document : IExternalCommandAvailability
        {
            public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
            {
                return uiApp.ActiveUIDocument is not null;
            }
        }

        // Command can only be ran in a project (non-family) document
        private class Project : IExternalCommandAvailability
        {
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

        // Command can only be ran in a family document
        private class Family : IExternalCommandAvailability
        {
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

        // Command can only be ran in a workshared document
        private class Workshared : IExternalCommandAvailability
        {
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

        // Command can only be ran when elements are selected (even if in family document)
        private class Selection : IExternalCommandAvailability
        {
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

        // Command can only be ran when active view is a schedule
        private class ActiveViewSchedule : IExternalCommandAvailability
        {
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

        // Command can only be ran when sheets are in selection
        private class SelectionIncludesSheets : IExternalCommandAvailability
        {
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

        // Command can only be ran when only sheets are in selection
        private class SelectionOnlySheets : IExternalCommandAvailability
        {
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

        #endregion
    }
}