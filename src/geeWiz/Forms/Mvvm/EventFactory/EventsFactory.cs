// Revit API
using Autodesk.Revit.UI;

namespace geeWiz.Forms.Mvvm
{
    /// <summary>
    /// Factory responsible for creating different "flavours" of ExternalEvent wrappers.
    /// 
    /// This was AI written/guided for the most part.
    /// Comments have been added to break down what this is achieving.
    /// The goal is to make a simpler way of creating handled events for Mvvm.
    /// Revit/UI have to be on different threads, so we generally use async.
    /// </summary>
    public static class ExternalEventFactory
    {
        /// <summary>
        /// Creates a SIMPLE (synchronous) external event.
        /// You pass in a name + action to run inside Revit's API context.
        /// </summary>
        /// <param name="name">The event name.</param>
        /// <param name="action">The event action.</param>
        /// <returns></returns>
        public static IExternalEventAdapter CreateSync(string name, Action<UIApplication> action)
        {
            // Wrap your action into a handler Revit understands
            var handler = new ActionExternalEventHandler(name, action);
            
            // Wrap the Revit ExternalEvent in an adapter (clean interface)
            return new ExternalEventAdapter(handler.ExternalEvent);
        }

        /// <summary>
        /// Creates an ASYNC external event (no return value).
        /// THandler must inherit from AsyncExternalEventHandler.
        /// </summary>
        /// <typeparam name="THandler">The event handler.</typeparam>
        /// <returns></returns>
        public static IExternalEventAdapterAsync CreateAsync<THandler>()
            where THandler : AsyncExternalEventHandler, new()
        {
            // Create handler via default constructor
            var handler = new THandler();

            // Wrap it in an async adapter
            return new ExternalEventAdapterAsync(handler);
        }

        /// <summary>
        /// Same as above, but lets you control how the handler is created.
        /// (useful for dependency injection, passing parameters, etc.).
        /// </summary>
        /// <typeparam name="THandler">The event handler.</typeparam>
        /// <param name="handlerFactory">The handler factory.</param>
        /// <returns></returns>
        public static IExternalEventAdapterAsync CreateAsync<THandler>(Func<THandler> handlerFactory)
            where THandler : AsyncExternalEventHandler
        {
            var handler = handlerFactory();
            return new ExternalEventAdapterAsync(handler);
        }

        /// <summary>
        /// Async external event WITH a return value.
        /// </summary>
        /// <typeparam name="TResult">Event result.</typeparam>
        /// <typeparam name="THandler">Event handler.</typeparam>
        /// <returns>An ExternalEventAdapterAsync.</returns>
        public static IExternalEventAdapterAsync<TResult> CreateAsync<TResult, THandler>()
            where THandler : AsyncExternalEventHandler<TResult>, new()
        {
            var handler = new THandler();
            return new ExternalEventAdapterAsync<TResult>(handler);
        }

        /// <summary>
        /// Async external event WITH a return value, using custom factory.
        /// </summary>
        /// <typeparam name="TResult">Event result.</typeparam>
        /// <typeparam name="THandler">Event handler.</typeparam>
        /// <returns>An ExternalEventAdapterAsync.</returns>
        public static IExternalEventAdapterAsync<TResult> CreateAsync<TResult, THandler>(Func<THandler> handlerFactory)
            where THandler : AsyncExternalEventHandler<TResult>
        {
            var handler = handlerFactory();
            return new ExternalEventAdapterAsync<TResult>(handler);
        }
    }

    /// <summary>
    /// SIMPLE interface: fire-and-forget external event.
    /// </summary>
    public interface IExternalEventAdapter
    {
        /// <summary>
        /// Raises the event.
        /// </summary>
        void Raise();
    }

    /// <summary>
    /// ASYNC interface (no result).
    /// </summary>
    public interface IExternalEventAdapterAsync
    {
        /// <summary>
        /// Raises the event.
        /// </summary>
        Task RaiseAsync();
    }

    /// <summary>
    /// ASYNC interface WITH result.
    /// </summary>
    /// <typeparam name="TResult">The result of the task.</typeparam>
    public interface IExternalEventAdapterAsync<TResult>
    {
        /// <summary>
        /// Raises the event.
        /// </summary>
        Task<TResult> RaiseAsync();
    }

    /// <summary>
    /// Concrete adapter for standard Revit ExternalEvent.
    /// </summary>
    public sealed class ExternalEventAdapter : IExternalEventAdapter
    {
        /// <summary>
        /// The related event handler.
        /// </summary>
        private readonly ExternalEvent _externalEvent;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="externalEvent">The related event.</param>
        public ExternalEventAdapter(ExternalEvent externalEvent)
        {
            _externalEvent = externalEvent;
        }

        /// <summary>
        /// Just forwards the call to Revit's API.
        /// </summary>
        public void Raise() => _externalEvent.Raise();
    }

    /// <summary>
    /// // Adapter for async handler (no result).
    /// </summary>
    public sealed class ExternalEventAdapterAsync : IExternalEventAdapterAsync
    {
        /// <summary>
        /// The related event handler.
        /// </summary>
        private readonly AsyncExternalEventHandler _handler;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="handler">The event handler.</param>
        public ExternalEventAdapterAsync(AsyncExternalEventHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Delegates to handler which manages Task completion.
        /// </summary>
        /// <returns>A Task.</returns>
        public Task RaiseAsync() => _handler.RaiseAsync();
    }

    /// <summary>
    /// Adapter for async handler WITH result.
    /// </summary>
    /// <typeparam name="TResult">The result.</typeparam>
    public sealed class ExternalEventAdapterAsync<TResult> : IExternalEventAdapterAsync<TResult>
    {
        /// <summary>
        /// The related event handler.
        /// </summary>
        private readonly AsyncExternalEventHandler<TResult> _handler;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="handler">The event handler.</param>
        public ExternalEventAdapterAsync(AsyncExternalEventHandler<TResult> handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Returns a Task from the handler.
        /// </summary>
        /// <returns>A Task.</returns>
        public Task<TResult> RaiseAsync() => _handler.RaiseAsync();
    }

}
