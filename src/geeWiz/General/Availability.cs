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

        // Limit the options we can choose as availabilities using a class
        public static class AvailabilityNames
        {
            public static readonly string Disabled = typeof(Disabled).FullName;
            public static readonly string ZeroDoc = typeof(ZeroDoc).FullName;
            public static readonly string Document = typeof(Document).FullName;
            public static readonly string Project = typeof(Project).FullName;
            public static readonly string Family = typeof(Family).FullName;
            public static readonly string Workshared = typeof(Workshared).FullName;
            public static readonly string Selection = typeof(Selection).FullName;
            public static readonly string ActiveViewSchedule = typeof(ActiveViewSchedule).FullName;
            public static readonly string SelectionIncludesSheets = typeof(SelectionIncludesSheets).FullName;
            public static readonly string SelectionOnlySheets = typeof(SelectionOnlySheets).FullName;
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