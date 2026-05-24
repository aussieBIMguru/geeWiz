// Revit API
using Autodesk.Revit.UI;

namespace geeWiz.Forms.Mvvm
{
    /// <summary>
    /// Wraps a simple sync Action into an IExternalEventHandler.
    /// 
    /// This was AI written/guided for the most part.
    /// Comments have been added to break down what this is achieving.
    /// The goal is to make a simpler way of creating handled events for Mvvm.
    /// Revit/UI have to be on different threads, so we generally use async.
    /// </summary>
    public sealed class ActionExternalEventHandler : IExternalEventHandler
    {
        /// <summary>
        /// Name of event handler.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The actual logic to run inside Revit
        /// </summary>
        private readonly Action<UIApplication> _action;

        /// <summary>
        /// The Revit ExternalEvent associated with this handler.
        /// </summary>
        public ExternalEvent ExternalEvent { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Name of handler.</param>
        /// <param name="action">Action for handler.</param>
        public ActionExternalEventHandler(string name, Action<UIApplication> action)
        {
            this._name = name;
            this._action = action;

            // Register THIS handler with Revit
            this.ExternalEvent = ExternalEvent.Create(this);
        }

        /// <summary>
        /// This is called by Revit when the event is executed.
        /// </summary>
        /// <param name="app">The UIApplication.</param>
        public void Execute(UIApplication app)
        {
            this._action?.Invoke(app);
        }

        /// <summary>
        /// Get the name of the handler.
        /// </summary>
        /// <returns>A String.</returns>
        public string GetName() => _name;
    }

    /// <summary>
    /// Base async external event handler (no result) with TaskCompletionSource plumbing.
    /// Subclass and implement ExecuteAsyncCore.
    /// </summary>
    public abstract class AsyncExternalEventHandler : IExternalEventHandler
    {
        /// <summary>
        /// This bridges Revit → Task (async/await world).
        /// </summary>
        private TaskCompletionSource<object> _tcs;

        /// <summary>
        /// Prevents concurrent execution.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Related event.
        /// </summary>
        public ExternalEvent ExternalEvent { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected AsyncExternalEventHandler()
        {
            this.ExternalEvent = ExternalEvent.Create(this);
        }


        /// <summary>
        /// Called by YOUR code.
        /// </summary>
        /// <returns></returns>
        public Task RaiseAsync()
        {
            lock (_lock)
            {
                // Prevent re-entry (Revit can't handle concurrent external events well)
                if (this._tcs != null && !this._tcs.Task.IsCompleted)
                    throw new InvalidOperationException("ExternalEvent already running.");

                // Create a new TaskCompletionSource for this invocation
                this._tcs = new TaskCompletionSource<object>();
            }

            // Tell Revit: "run this handler soon"
            this.ExternalEvent.Raise();

            // Return the Task so caller can await it
            return _tcs.Task;
        }

        /// <summary>
        /// Called by Revit on its main thread.
        /// </summary>
        /// <param name="app">The UIApplication.</param>
        public void Execute(UIApplication app)
        {
            this.ExecuteInternalAsync(app);
        }

        /// <summary>
        /// Async bridge method (fire-and-forget from Revit's perspective).
        /// </summary>
        /// <param name="app">The UIApplication.</param>
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

        /// <summary>
        /// YOU implement this in derived classes.
        /// </summary>
        /// <param name="app">The UIApplication.</param>
        /// <returns>A Task.</returns>
        protected abstract Task ExecuteAsyncCore(UIApplication app);

        /// <summary>
        /// Get handler name.
        /// </summary>
        /// <returns>A String.</returns>
        public abstract string GetName();
    }

    /// <summary>
    /// Base async external event handler with typed result.
    /// </summary>
    public abstract class AsyncExternalEventHandler<TResult> : IExternalEventHandler
    {
        /// <summary>
        /// Same as above, but strongly typed result.
        /// </summary>
        private TaskCompletionSource<TResult> _tcs;

        /// <summary>
        /// Lock the handler.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Related event.
        /// </summary>
        public ExternalEvent ExternalEvent { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected AsyncExternalEventHandler()
        {
            ExternalEvent = ExternalEvent.Create(this);
        }

        /// <summary>
        /// Raise the event.
        /// </summary>
        /// <returns>A Task.</returns>
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

        /// <summary>
        /// Execute the task.
        /// </summary>
        /// <param name="app">The UIApplication.</param>
        public void Execute(UIApplication app)
        {
            ExecuteInternalAsync(app);
        }

        /// <summary>
        /// Execute the task.
        /// </summary>
        /// <param name="app">The UIApplication.</param>
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

        /// <summary>
        /// YOU implement this.
        /// </summary>
        /// <param name="app">The UIApplication.</param>
        /// <returns>A Task.</returns>
        protected abstract Task<TResult> ExecuteAsyncCore(UIApplication app);

        /// <summary>
        /// Get the name of the event.
        /// </summary>
        /// <returns>A String.</returns>
        public abstract string GetName();
    }
}