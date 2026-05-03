// Revit API
using Autodesk.Revit.UI;

namespace geeWiz.Forms.Mvvm
{
    // This was AI written/guided for the most part
    // Comments have been added to break down what this is achieving
    // The goal is to make a simpler way of creating handled events for Mvvm
    // Revit/UI have to be on different threads, so we generally use async

    // Factory responsible for creating different "flavours" of ExternalEvent wrappers
    public static class ExternalEventFactory
    {
        // Creates a SIMPLE (synchronous) external event
        // You pass in a name + action to run inside Revit's API context
        public static IExternalEventAdapter CreateSync(
        string name,
        Action<UIApplication> action)
        {
            // Wrap your action into a handler Revit understands
            var handler = new ActionExternalEventHandler(name, action);

        // Wrap the Revit ExternalEvent in an adapter (clean interface)
        return new ExternalEventAdapter(handler.ExternalEvent);
        }

        // Creates an ASYNC external event (no return value)
        // THandler must inherit from AsyncExternalEventHandler
        public static IExternalEventAdapterAsync CreateAsync<THandler>()
            where THandler : AsyncExternalEventHandler, new()
        {
            // Create handler via default constructor
            var handler = new THandler();

            // Wrap it in an async adapter
            return new ExternalEventAdapterAsync(handler);
        }

        // Same as above, but lets you control how the handler is created
        // (useful for dependency injection, passing parameters, etc.)
        public static IExternalEventAdapterAsync CreateAsync<THandler>(
            Func<THandler> handlerFactory)
            where THandler : AsyncExternalEventHandler
        {
            var handler = handlerFactory();
            return new ExternalEventAdapterAsync(handler);
        }

        // Async external event WITH a return value
        public static IExternalEventAdapterAsync<TResult> CreateAsync<TResult, THandler>()
            where THandler : AsyncExternalEventHandler<TResult>, new()
        {
            var handler = new THandler();
            return new ExternalEventAdapterAsync<TResult>(handler);
        }

        // Same as above, but with custom factory
        public static IExternalEventAdapterAsync<TResult> CreateAsync<TResult, THandler>(
            Func<THandler> handlerFactory)
            where THandler : AsyncExternalEventHandler<TResult>
        {
            var handler = handlerFactory();
            return new ExternalEventAdapterAsync<TResult>(handler);
        }
    }

    // SIMPLE interface: fire-and-forget external event
    public interface IExternalEventAdapter
    {
        void Raise();
    }

    // ASYNC interface (no result)
    public interface IExternalEventAdapterAsync
    {
        Task RaiseAsync();
    }

    // ASYNC interface WITH result
    public interface IExternalEventAdapterAsync<TResult>
    {
        Task<TResult> RaiseAsync();
    }

    // Concrete adapter for standard Revit ExternalEvent
    public sealed class ExternalEventAdapter : IExternalEventAdapter
    {
        private readonly ExternalEvent _externalEvent;

        public ExternalEventAdapter(ExternalEvent externalEvent)
        {
            _externalEvent = externalEvent;
        }

        // Just forwards the call to Revit's API
        public void Raise() => _externalEvent.Raise();
    }

    // Adapter for async handler (no result)
    public sealed class ExternalEventAdapterAsync : IExternalEventAdapterAsync
    {
        private readonly AsyncExternalEventHandler _handler;

        public ExternalEventAdapterAsync(AsyncExternalEventHandler handler)
        {
            _handler = handler;
        }

        // Delegates to handler which manages Task completion
        public Task RaiseAsync() => _handler.RaiseAsync();
    }

    // Adapter for async handler WITH result
    public sealed class ExternalEventAdapterAsync<TResult> : IExternalEventAdapterAsync<TResult>
    {
        private readonly AsyncExternalEventHandler<TResult> _handler;

        public ExternalEventAdapterAsync(AsyncExternalEventHandler<TResult> handler)
        {
            _handler = handler;
        }

        // Returns a Task<TResult> from the handler
        public Task<TResult> RaiseAsync() => _handler.RaiseAsync();
    }

}
