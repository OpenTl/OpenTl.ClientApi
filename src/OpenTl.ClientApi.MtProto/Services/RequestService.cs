namespace OpenTl.ClientApi.MtProto.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using log4net;

    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IRequestService))]
    internal class RequestService : IRequestService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RequestService));

        private readonly Dictionary<long, (Timer, TaskCompletionSource<object>)> _resultCallbacks = new Dictionary<long, (Timer, TaskCompletionSource<object>)>();

        public Task<object> RegisterRequest(IRequest request, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();

            if (cancellationToken != default(CancellationToken))
            {
                cancellationToken.Register(
                    () =>
                    {
                        if (!tcs.Task.IsCompleted)
                        {
                            tcs.TrySetCanceled(cancellationToken);
                        }
                    });
            }
            _resultCallbacks[request.GetHashCode()] = (null, tcs);
            
            return tcs.Task;
        }

        public void SetMessageId(IRequest request, long messageId)
        {
            if (!_resultCallbacks.TryGetValue(request.GetHashCode(), out var value))
            {
                return;
            }

            (var _, var tcs) = value;
            
            var timer = new Timer( _ =>
            {
                if (!tcs.Task.IsCompleted)
                {
                    Log.Warn($"Message response result timed out for messageid '{messageId}'");
                        
                    _resultCallbacks.Remove(messageId);
                        
                    tcs.TrySetCanceled();
                }
            }, null, TimeSpan.FromMinutes(1), TimeSpan.Zero);

            _resultCallbacks.Remove(request.GetHashCode());
            _resultCallbacks[messageId] = (timer, tcs);
        }

        public void ReturnException(long messageId, Exception exception)
        {
            if (_resultCallbacks.TryGetValue(messageId, out var data))
            {
                (Timer timer, TaskCompletionSource<object> callback) = data;
                timer.Dispose();
                
                callback.TrySetException(exception);
                
                Log.Error($"Request was processed with error", exception);
                
                _resultCallbacks.Remove(messageId);
            }
            else
            {
                Log.Error($"Callback for request with Id {messageId} wasn't found");
            }
        }

        public void ReturnException(Exception exception)
        {
            Log.Error($"All requests was processed with error", exception);

            foreach ((Timer timer, TaskCompletionSource<object> callback) in _resultCallbacks.Values)
            {
                timer.Dispose();
                callback.TrySetException(exception);
            }
        }

        public void ReturnResult(long messageId, object obj)
        {
            if (_resultCallbacks.TryGetValue(messageId, out var data))
            {
                (Timer timer, TaskCompletionSource<object> callback) = data;
                timer.Dispose();
                
                callback.TrySetResult(obj);
                
                _resultCallbacks.Remove(messageId);
            }
            else
            {
                Log.Error($"Callback for request with Id {messageId} wasn't found");
            }
        }
    }
}