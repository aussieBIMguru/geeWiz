// Revit API
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
// geeWiz
using gFrm = geeWiz.Forms;
using geeWiz.Extensions;
using gSel = geeWiz.Utilities.Select_Utils;
using gWin = geeWiz.Utilities.WindowController;
// Mvvm toolkit
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// Using the Mvvm Models namespace
namespace geeWiz.Forms.Mvvm.Models
{
    #region Example implementation

    /*
 
    if (gWin.Focus<View>())
    {
        return Result.Succeeded;
    }
    
    var viewModel = new Model();
    viewModel.WireExternalEvents(uiApp); < IMPORTANT, DO NOT MISS!

    var view = new TestView(viewModel);
    gWin.Show(view, Globals.UiApp.MainWindowHandle);

    return Result.Succeeded;

    */

    #endregion

    /// <summary>
    /// The code to manage the Wpf model
    /// </summary>
    public sealed partial class ModelSample : ObservableObject
    {
        #region Event wiring

        // Track if events are wired
        private bool _eventsWired;
        private readonly object _eventLock = new object();

        // Events to handle
        public IExternalEventAdapter ShowSummaryEvent { get; private set; }
        public IExternalEventAdapterAsync<ElementId> DeleteElementEvent { get; private set; }
        public IExternalEventAdapterAsync SelectDelayedEvent { get; private set; }

        /// <summary>
        /// Wires the events to the buttons. Call from Command so it's wired on Revit thread.
        /// </summary>
        /// <param name="uiApp">Revit UIApplication.</param>
        public void WireExternalEvents(UIApplication uiApp)
        {
            // Finish if already wired
            if (this._eventsWired)
            {
                return;
            }

            // Lock to ensure this only gets called once
            lock (this._eventLock)
            {
                // Finish if already wired
                if (this._eventsWired)
                {
                    return;
                }

                // Handlers belong logically to the model
                var showSummaryHandler = new ShowSummaryHandler(this);
                var deleteElementHandler = new DeleteElementHandler();
                var delayedSelectHandler = new SelectDelayedElementHandler(this);

                // ExternalEvents must be created here (inside Revit API context)
                var showSummaryEvent = ExternalEvent.Create(showSummaryHandler);
                // Async handlers create their own ExternalEvent via factory

                // Adapters stored on the ViewModel
                this.ShowSummaryEvent = new ExternalEventAdapter(showSummaryEvent);
                this.DeleteElementEvent = new ExternalEventAdapterAsync<ElementId>(deleteElementHandler);
                this.SelectDelayedEvent = new ExternalEventAdapterAsync(delayedSelectHandler);

                // Events are now wired
                this._eventsWired = true;
            }
        }

        #endregion

        #region Observable properties

        // Generate properties for bound strings
        [ObservableProperty]
        private string _strBind_Element;

        [ObservableProperty]
        private string _strBind_Category;

        [ObservableProperty]
        private string _strBind_Status;

        #endregion

        #region Command bindings

        // Bound command to the Wpf form - summarize
        [RelayCommand]
        private void ShowSummary()
        {
            // Raise the handled event
            this.ShowSummaryEvent?.Raise();
        }

        // Bound command to the Wpf form - delete
        [RelayCommand]
        private async Task DeleteElementAsync()
        {
            // Raise the handled event
            var deletedId = await DeleteElementEvent?.RaiseAsync();

            // If the Id is valid...
            if (deletedId.Ext_IsValid())
            {
                // Report the outcome to the user
                gFrm.Custom.BubbleMessage(
                    "Element deleted",
                    $"Element Id: {deletedId}");
            }
        }

        // Bound command to the Wpf form - delayed selection
        [RelayCommand]
        private async Task SelectDelayedElementAsync()
        {
            // Ensure we have an event to run
            if (this.SelectDelayedEvent == null) { return; }

            // Report delay to user
            this.StrBind_Status = "Waiting 2 seconds...";
            await Task.Delay(2000);

            // Raise the handled event
            await SelectDelayedEvent?.RaiseAsync();
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Updates the element properties in the form.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="status">Status message (optional).</param>
        public void UpdateElementProperties(Element element, string status = null)
        {
            // Catch no element
            if (element is null) { return; }

            // Set form properties
            this.StrBind_Element = element.Name;
            this.StrBind_Category = element.Category?.Name ?? "No category";

            // Set status if valid
            if (status is not null)
            {
                this.StrBind_Status = status;
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// Handler for event.
        /// </summary>
        public sealed class ShowSummaryHandler : IExternalEventHandler
        {
            private readonly ModelSample _vm;

            public ShowSummaryHandler(ModelSample vm)
            {
                this._vm = vm;
            }

            public void Execute(UIApplication app)
            {
                // Select the element
                var uiDoc = app.ActiveUIDocument;
                var element = uiDoc.Ext_PickWithFilter(new gSel.ISF_AnyElement(), "Select an element");

                // Update properties
                this._vm.UpdateElementProperties(element);
            }

            public string GetName() => nameof(ShowSummaryHandler);
        }

        /// <summary>
        /// Handler for event.
        /// </summary>
        public sealed class DeleteElementHandler : AsyncExternalEventHandler<ElementId>
        {
            protected override Task<ElementId> ExecuteAsyncCore(UIApplication app)
            {
                // Select the element
                var uiDoc = app.ActiveUIDocument;
                var element = uiDoc.Ext_PickWithFilter(new gSel.ISF_AnyElement(), "Select an element");

                // Return invalid Id if no selection
                if (element == null)
                {
                    return Task.FromResult(ElementId.InvalidElementId);
                }

                // Get the element document and Id
                var doc = element.Document;
                var id = element.Id;

                // If element is editable...
                if (element.Ext_IsEditable(doc))
                {
                    // Using a transaction...
                    using (Transaction t = new Transaction(doc, "Mvvm test"))
                    {
                        t.Start();

                        // Try to delete the element
                        doc.Ext_DeleteElementId(id);

                        t.Ext_SafeCommit();
                    }
                }

                // Return the elementId
                return Task.FromResult(id);
            }

            public override string GetName() => nameof(DeleteElementHandler);
        }

        /// <summary>
        /// Handler for event.
        /// </summary>
        public sealed class SelectDelayedElementHandler : AsyncExternalEventHandler
        {
            private readonly ModelSample _vm;

            public SelectDelayedElementHandler(ModelSample vm)
            {
                this._vm = vm;
            }

            protected override Task ExecuteAsyncCore(UIApplication app)
            {
                // Hide the view
                gWin.Hide<Mvvm.Views.ViewSample>();

                // Select the element
                var uiDoc = app.ActiveUIDocument;
                var element = uiDoc.Ext_PickWithFilter(new gSel.ISF_AnyElement(), "Select an element");

                // Update properties
                this._vm.UpdateElementProperties(element, string.Empty);

                // Show the view
                gWin.Show<Mvvm.Views.ViewSample>();

                // Task completed
                return Task.CompletedTask;
            }

            public override string GetName() => nameof(SelectDelayedElementHandler);
        }
    }

    #endregion
}