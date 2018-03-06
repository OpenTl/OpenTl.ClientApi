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
    internal sealed class RequestService : IRequestService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RequestService));

        private readonly Dictionary<long, RequestCacheItem> _requestCache = new Dictionary<long, RequestCacheItem>();

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

            _requestCache[request.GetHashCode()] = new RequestCacheItem(request, tcs); 
            
            return tcs.Task;
        }
        
        public void AttachRequestToMessageId(IRequest request, long messageId)
        {
            if (!_requestCache.TryGetValue(request.GetHashCode(), out var cacheItem))
            {
                return;
            }

            var timer = new Timer( _ =>
            {
                if (!cacheItem.TaskSource.Task.IsCompleted)
                {
                    Log.Warn($"Message response result timed out for messageid '{messageId}'");
                        
                    _requestCache.Remove(messageId);
                        
                    cacheItem.TaskSource.TrySetCanceled();
                }
            }, null, TimeSpan.FromMinutes(1), TimeSpan.Zero);

            _requestCache.Remove(request.GetHashCode());
            
            cacheItem.Timer = timer;
            _requestCache[messageId] = cacheItem;
        }

        public IRequest GetRequestToReply(long messageId)
        {
            if (!_requestCache.TryGetValue(messageId, out var cacheItem))
            {
                return null;
            }

            cacheItem.Timer.Dispose();
            cacheItem.Timer = null;
            
            _requestCache.Remove(messageId);

            _requestCache[cacheItem.Request.GetHashCode()] = cacheItem;

            return cacheItem.Request;
        }

        public void ReturnException(long messageId, Exception exception)
        {
            if (_requestCache.TryGetValue(messageId, out var cacheItem))
            {
                cacheItem.Timer.Dispose();
                
                cacheItem.TaskSource.TrySetException(exception);
                
                Log.Error($"Request was processed with error", exception);
                
                _requestCache.Remove(messageId);
            }
            else
            {
                Log.Error($"Callback for request with Id {messageId} wasn't found");
            }
        }

        public void ReturnException(Exception exception)
        {
            Log.Error($"All requests was processed with error", exception);

            foreach (var cacheItem in _requestCache.Values)
            {
               cacheItem.Timer.Dispose();
                
               cacheItem.TaskSource.TrySetException(exception);
            }
        }

        public void ReturnResult(long messageId, object obj)
        {
            if (_requestCache.TryGetValue(messageId, out var cacheItem))
            {
                cacheItem.Timer.Dispose();
                
                cacheItem.TaskSource.TrySetResult(obj);
                
                _requestCache.Remove(messageId);
            }
            else
            {
                Log.Error($"Callback for request with Id {messageId} wasn't found");
            }
        }
        
        private sealed class RequestCacheItem
        {
            public RequestCacheItem(IRequest request, TaskCompletionSource<object> taskSource)
            {
                Request = request;
                
                TaskSource = taskSource;
                
            }
            public Timer Timer { get; set; }

            public IRequest Request { get; }

            public TaskCompletionSource<object> TaskSource { get; }
        }
    }
}