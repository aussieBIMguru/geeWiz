// Revit API
using Autodesk.Revit.UI;

// The class belongs to the utility namespace
// using gAva = geeWiz.Utilities.Availability
namespace gSharp.Utilities
{
    /// <summary>
    /// Methods of this class generally relate to availability/context.
    /// This is assigned to commands on startup.
    /// </summary>
    public static class AvailabilityUtils
    {
        // Command is disabled
        public class Disabled : IExternalCommandAvailability
        {
            public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
            {
                return false;
            }
        }

        // Command can only be ran even if a document is not opened
        public class ZeroDoc : IExternalCommandAvailability
        {
            public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
            {
                return true;
            }
        }

        // Command can only be ran in a project (non-family) document
        public class Project : IExternalCommandAvailability
        {
            public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
            {
                if (uiApp.ActiveUIDocument != null)
                {
                    Document doc = uiApp.ActiveUIDocument.Document;
                    return !doc.IsFamilyDocument;
                }
                else
                {
                    return false;
                }
            }
        }

        // Command can only be ran in a family document
        public class Family : IExternalCommandAvailability
        {
            public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
            {
                if (uiApp.ActiveUIDocument != null)
                {
                    Document doc = uiApp.ActiveUIDocument.Document;
                    return doc.IsFamilyDocument;
                }
                else
                {
                    return false;
                }
            }
        }

        // Command can only be ran in a workshared document
        public class Workshared : IExternalCommandAvailability
        {
            public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
            {
                if (uiApp.ActiveUIDocument != null)
                {
                    Document doc = uiApp.ActiveUIDocument.Document;
                    return doc.IsWorkshared;
                }
                else
                {
                    return false;
                }
            }
        }

        // Command can only be ran when elements are selected and in a project
        public class ProjectSelection : IExternalCommandAvailability
        {
            public bool IsCommandAvailable(UIApplication uiApp, CategorySet categories)
            {
                if (uiApp.ActiveUIDocument != null)
                {
                    Document doc = uiApp.ActiveUIDocument.Document;
                    if (doc.IsFamilyDocument) { return false; }

                    // Return if at least 1 category is in selection
                    return categories.Size > 0;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}