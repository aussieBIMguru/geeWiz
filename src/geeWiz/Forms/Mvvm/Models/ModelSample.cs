// Revit API
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
// geeWiz
using gFrm = geeWiz.Forms;
using geeWiz.Extensions;
using gWin = geeWiz.Utilities.WindowController;
// Mvvm toolkit
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// Using the Mvvm Models namespace
namespace geeWiz.Forms.Mvvm.Models
{
    /// <summary>
    /// The Model of the MVVM Sample system.
    /// </summary>
    public sealed partial class ModelSample : ObservableObject
    {
        #region Event wiring

        /// <summary>
        /// Have events been wired yet.
        /// </summary>
        private bool _eventsWired;

        /// <summary>
        /// Prevents wiring more than once.
        /// </summary>
        private readonly object _eventLock = new object();

        /// <summary>
        /// Event to show element summary.
        /// </summary>
        public IExternalEventAdapter ShowSummaryEvent { get; private set; }

        /// <summary>
        /// Event to delete element.
        /// </summary>
        public IExternalEventAdapterAsync<ElementId> DeleteElementEvent { get; private set; }

        /// <summary>
        /// Event to select element with a delay.
        /// </summary>
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
                var deleteElementHandler = new DeleteElementHandler(this);
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

        /// <summary>
        /// Binding to form for element name.
        /// </summary>
        [ObservableProperty]
        private string _strBind_Element;

        /// <summary>
        /// Binding to form for category name.
        /// </summary>
        [ObservableProperty]
        private string _strBind_Category;

        /// <summary>
        /// Binding to form for status of form.
        /// </summary>
        [ObservableProperty]
        private string _strBind_Status;

        #endregion

        #region Command bindings

        /// <summary>
        /// Relayed command to show element summary.
        /// </summary>
        [RelayCommand]
        private void ShowSummary()
        {
            // Raise the handled event
            this.ShowSummaryEvent?.Raise();
        }

        /// <summary>
        /// Relayed command to delete element.
        /// </summary>
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

        /// <summary>
        /// Relayed command to select element with delay.
        /// </summary>
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
            /// <summary>
            /// The related Model.
            /// </summary>
            private readonly ModelSample _vm;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="vm">Related Model.</param>
            public ShowSummaryHandler(ModelSample vm)
            {
                this._vm = vm;
            }

            /// <summary>
            /// Execute the event.
            /// </summary>
            /// <param name="app">The UIApplication.</param>
            public void Execute(UIApplication app)
            {
                // Select the element
                UIDocument uiDoc = app.ActiveUIDocument;
                Element element = uiDoc.Ext_PickWithFilter(new ISF.AnyElement(), "Select an element");

                // Update properties
                this._vm.UpdateElementProperties(element);
            }

            /// <summary>
            /// Return name of handler.
            /// </summary>
            /// <returns>A string.</returns>
            public string GetName() => nameof(ShowSummaryHandler);
        }

        /// <summary>
        /// Handler for event.
        /// </summary>
        public sealed class DeleteElementHandler : AsyncExternalEventHandler<ElementId>
        {
            /// <summary>
            /// The related Model.
            /// </summary>
            private readonly ModelSample _vm;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="vm">Related Model.</param>
            public DeleteElementHandler(ModelSample vm)
            {
                this._vm = vm;
            }

            /// <summary>
            /// Execute the event.
            /// </summary>
            /// <param name="app">The UIApplication.</param>
            protected override Task<ElementId> ExecuteAsyncCore(UIApplication app)
            {
                // Select the element
                UIDocument uiDoc = app.ActiveUIDocument;
                Element element = uiDoc.Ext_PickWithFilter(new ISF.AnyElement(), "Select an element");

                // Return invalid Id if no selection
                if (element is null)
                {
                    return Task.FromResult(ElementId.InvalidElementId);
                }

                // Get the element document and Id
                Document doc = element.Document;
                ElementId id = element.Id;

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

            /// <summary>
            /// Return name of handler.
            /// </summary>
            /// <returns>A string.</returns>
            public override string GetName() => nameof(DeleteElementHandler);
        }

        /// <summary>
        /// Handler for event.
        /// </summary>
        public sealed class SelectDelayedElementHandler : AsyncExternalEventHandler
        {
            /// <summary>
            /// The related Model.
            /// </summary>
            private readonly ModelSample _vm;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="vm">Related Model.</param>
            public SelectDelayedElementHandler(ModelSample vm)
            {
                this._vm = vm;
            }

            /// <summary>
            /// Execute the event.
            /// </summary>
            /// <param name="app">The UIApplication.</param>
            protected override Task ExecuteAsyncCore(UIApplication app)
            {
                // Hide the view
                gWin.Hide<Mvvm.Views.ViewSample>();

                // Select the element
                UIDocument uiDoc = app.ActiveUIDocument;
                Element element = uiDoc.Ext_PickWithFilter(new ISF.AnyElement(), "Select an element");

                // Update properties
                this._vm.UpdateElementProperties(element, string.Empty);

                // Show the view
                gWin.Show<Mvvm.Views.ViewSample>();

                // Task completed
                return Task.CompletedTask;
            }

            /// <summary>
            /// Return name of handler.
            /// </summary>
            /// <returns>A string.</returns>
            public override string GetName() => nameof(SelectDelayedElementHandler);
        }
    }

    #endregion
}