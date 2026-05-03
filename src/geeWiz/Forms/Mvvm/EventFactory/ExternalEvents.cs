// Revit API
using Autodesk.Revit.UI;

namespace geeWiz.Forms.Mvvm
{
    // This was AI written/guided for the most part
    // Comments have been added to break down what this is achieving
    // The goal is to make a simpler way of creating handled events for Mvvm
    // Revit/UI have to be on different threads, so we generally use async

    /// <summary>
    /// Wraps a simple sync Action<UIApplication> into an IExternalEventHandler.
    /// </summary>
    public sealed class ActionExternalEventHandler : IExternalEventHandler
    {
        private readonly string _name;

    // The actual logic to run inside Revit
    private readonly Action<UIApplication> _action;

        // The Revit ExternalEvent associated with this handler
        public ExternalEvent ExternalEvent { get; }

        public ActionExternalEventHandler(string name, Action<UIApplication> action)
        {
            _name = name;
            _action = action;

            // Register THIS handler with Revit
            ExternalEvent = ExternalEvent.Create(this);
        }

        // This is called by Revit when the event is executed
        public void Execute(UIApplication app)
        {
            _action?.Invoke(app);
        }

        public string GetName() => _name;
    }

    /// <summary>
    /// Base async external event handler (no result) with TaskCompletionSource plumbing.
    /// Subclass and implement ExecuteAsyncCore.
    /// </summary>
    public abstract class AsyncExternalEventHandler : IExternalEventHandler
    {
        // This bridges Revit → Task (async/await world)
        private TaskCompletionSource<object> _tcs;

        // Prevents concurrent execution
        private readonly object _lock = new object();

        public ExternalEvent ExternalEvent { get; }

        protected AsyncExternalEventHandler()
        {
            ExternalEvent = ExternalEvent.Create(this);
        }

        // Called by YOUR code
        public Task RaiseAsync()
        {
            lock (_lock)
            {
                // Prevent re-entry (Revit can't handle concurrent external events well)
                if (_tcs != null && !_tcs.Task.IsCompleted)
                    throw new InvalidOperationException("ExternalEvent already running.");

                // Create a new TaskCompletionSource for this invocation
                _tcs = new TaskCompletionSource<object>();
            }

            // Tell Revit: "run this handler soon"
            ExternalEvent.Raise();

            // Return the Task so caller can await it
            return _tcs.Task;
        }

        // Called by Revit on its main thread
        public void Execute(UIApplication app)
        {
            ExecuteInternalAsync(app);
        }

        // Async bridge method (fire-and-forget from Revit's perspective)
        private async void ExecuteInternalAsync(UIApplication app)
        {
            try
            {
                // Run your actual async logic
                await ExecuteAsyncCore(app).ConfigureAwait(false);

                // Signal completion to awaiting caller
                _tcs?.TrySetResult(null);
            }
            catch (Exception ex)
            {
                // Propagate exception back to awaiting caller
                _tcs?.TrySetException(ex);
            }
            finally
            {
                // Reset state
                _tcs = null;
            }
        }

        // YOU implement this in derived classes
        protected abstract Task ExecuteAsyncCore(UIApplication app);

        public abstract string GetName();
    }

    /// <summary>
    /// Base async external event handler with typed result.
    /// </summary>
    public abstract class AsyncExternalEventHandler<TResult> : IExternalEventHandler
    {
        // Same as above, but strongly typed result
        private TaskCompletionSource<TResult> _tcs;

        private readonly object _lock = new object();

        public ExternalEvent ExternalEvent { get; }

        protected AsyncExternalEventHandler()
        {
            ExternalEvent = ExternalEvent.Create(this);
        }

        public Task<TResult> RaiseAsync()
        {
            lock (_lock)
            {
                if (_tcs != null && !_tcs.Task.IsCompleted)
                    throw new InvalidOperationException("ExternalEvent already running.");

                _tcs = new TaskCompletionSource<TResult>();
            }

            ExternalEvent.Raise();
            return _tcs.Task;
        }

        public void Execute(UIApplication app)
        {
            ExecuteInternalAsync(app);
        }

        private async void ExecuteInternalAsync(UIApplication app)
        {
            try
            {
                // Capture result from your logic
                TResult result = await ExecuteAsyncCore(app).ConfigureAwait(false);

                // Complete Task with result
                _tcs?.TrySetResult(result);
            }
            catch (Exception ex)
            {
                _tcs?.TrySetException(ex);
            }
            finally
            {
                _tcs = null;
            }
        }

        // YOU implement this
        protected abstract Task<TResult> ExecuteAsyncCore(UIApplication app);

        public abstract string GetName();
    }
}