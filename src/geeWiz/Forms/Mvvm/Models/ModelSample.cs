// Nic3Point event handlers
using Nice3point.Revit.Toolkit.External.Handlers;
// Autodesk
using Autodesk.Revit.UI;
// geeWiz
using geeWiz.Utilities;
using geeWiz.Extensions;
using gFrm = geeWiz.Forms;
using gSel = geeWiz.Utilities.Select_Utils;

// Using the Mvvm Models namespace
namespace geeWiz.Forms.Mvvm.Models
{
    // Example implementation

    /*
 
    if (WindowController.Focus<View>())
    {
        return Result.Succeeded;
    }
    
    var viewModel = new Model();
    var view = new TestView(viewModel);

    WindowController.Show(view, Globals.UiApp.MainWindowHandle);

    return Result.Succeeded;

    */

    /// <summary>
    /// The code to manage the Wpf model
    /// </summary>
    public sealed partial class ModelSample : ObservableObject
    {
        // Event handlers (from Nic3Point)
        private readonly ActionEventHandler _externalHandler = new();
        private readonly AsyncEventHandler _asyncExternalHandler = new();
        private readonly AsyncEventHandler<ElementId> _asyncIdExternalHandler = new();

        // Generate properties for bound strings
        [ObservableProperty] private string _strBind_Element;
        [ObservableProperty] private string _strBind_Category;
        [ObservableProperty] private string _strBind_Status;

        // Bound command to the Wpf form - summarize
        [RelayCommand]
        private void CmdBind_ShowSummary()
        {
            // Raise event handler
            _externalHandler.Raise(application =>
            {
                // Get active UI Document
                var uiDoc = Globals.UiApp.ActiveUIDocument;

                // Select an element
                var selectedElement = uiDoc.Ext_PickWithFilter(new gSel.ISF_AnyElement(), "Select an element");

                // Update element properties
                UpdateElementProperties(selectedElement, "Element selected.");
            });
        }

        // Bound command to the Wpf form - delete
        [RelayCommand]
        private async Task CmdBind_DeleteElementAsync()
        {
            // Raise event handler
            ElementId deletedId = await _asyncIdExternalHandler.RaiseAsync(application =>
            {
                // Get active UI Document
                var uiDoc = Globals.UiApp.ActiveUIDocument;

                // Select an element
                var selectedElement = uiDoc.Ext_PickWithFilter(new gSel.ISF_AnyElement(), "Select an element");

                // Catch no element
                if (selectedElement is null)
                {
                    return ElementId.InvalidElementId;
                }

                // Get element Id and document
                ElementId id = selectedElement.Id;
                var doc = selectedElement.Document;

                // If editable, try to delete the element
                if (selectedElement.Ext_IsEditable(doc))
                {
                    using (var t = new Transaction(selectedElement.Document, "geeWiz: Delete element"))
                    {

                        t.Start();

                        this.StrBind_Status = doc.Ext_DeleteElementId(id) == Result.Succeeded ? "Element deleted" : "Element could not be deleted.";

                        t.Commit();
                    }
                }

                // Return the element Id
                return id;
            });

            // Message to user
            gFrm.Custom.BubbleMessage(title: "Element deleted", message: $"Element Id: {deletedId}");
        }

        // Bound command to the Wpf form - delayed selection
        [RelayCommand]
        private async Task CmdBind_SelectDelayedElementAsync()
        {
            // Delay for 2 seconds
            this.StrBind_Status = "Simulated delay...";
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Raise event handler
            await _asyncExternalHandler.RaiseAsync(application =>
            {
                // Hide the form
                WindowController.Hide<Views.ViewSample>();

                // Get active UI Document
                var uiDoc = Globals.UiApp.ActiveUIDocument;

                // Select an element
                var selectedElement = uiDoc.Ext_PickWithFilter(new gSel.ISF_AnyElement(), "Select an element");

                // Show the form
                WindowController.Show<Views.ViewSample>();

                // Update element properties
                UpdateElementProperties(selectedElement, "Element selected.");
            });

            // Clear the status
            this.StrBind_Status = string.Empty;
        }

        /// <summary>
        /// Updates the element properties in the form.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="status">Status message (optional).</param>
        private void UpdateElementProperties(Element element, string status = null)
        {
            // Catch no element
            if (element is null) { return; }

            // Set form properties
            this.StrBind_Element = element.Name;
            this.StrBind_Category = element.Category.Name;

            // Set status if valid
            if (status.Ext_HasChars())
            {
                this.StrBind_Status = status;
            }
        }
    }
}