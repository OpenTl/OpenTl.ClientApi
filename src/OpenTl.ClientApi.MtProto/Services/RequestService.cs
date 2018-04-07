namespace OpenTl.ClientApi.MtProto.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using log4net;

    using NullGuard;

    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IRequestService))]
    internal sealed class RequestService : IRequestService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RequestService));

        private readonly LinkedList<RequestCacheItem> _requestQueue = new LinkedList<RequestCacheItem>();

        public IClientSettings ClientSettings { get; set; }

        public void AttachRequestToMessageId(IRequest request, long messageId)
        {
            var cacheItem = _requestQueue.ToArray().FirstOrDefault(item => item.Request == request);

            if (cacheItem == null)
            {
                return;
            }

            var timer = new Timer(
                _ =>
                {
                    if (!cacheItem.TaskSource.Task.IsCompleted)
                    {
                        Log.Warn($"#{ClientSettings.ClientSession.SessionId}: Message response result timed out for messageid '{messageId}'");

                        _requestQueue.Remove(cacheItem);

                        cacheItem.TaskSource.TrySetCanceled();
                    }
                },
                null,
                TimeSpan.FromMinutes(10),
                TimeSpan.Zero);

            cacheItem.MessageId = messageId;
            cacheItem.Timer = timer;
        }

        [return:AllowNull]
        public Type GetExpectedResultType(long messageId)
        {
            var cacheItem = _requestQueue.FirstOrDefault(item => item.MessageId == messageId);

            return cacheItem?.Request.GetType().GetInterfaces().First(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)).GetGenericArguments()[0];
        }
        
        public IEnumerable<IRequest> GetAllRequestToReply()
        {
            foreach (var cacheItem in _requestQueue)
            {
                cacheItem.Timer?.Dispose();
                cacheItem.Timer = null;
            }

            return _requestQueue.Select(item => item.Request);
        }

        [return:AllowNull]
        public IRequest GetRequestToReply(long messageId)
        {
            var cacheItem = _requestQueue.FirstOrDefault(item => item.MessageId == messageId);

            if (cacheItem == null)
            {
                return null;
            }

            cacheItem.Timer?.Dispose();
            cacheItem.Timer = null;

            return cacheItem.Request;
        }

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

            _requestQueue.AddLast(new RequestCacheItem(request, tcs));

            return tcs.Task;
        }

        public void ReturnException(long messageId, Exception exception)
        {
            var cacheItem = _requestQueue.FirstOrDefault(item => item.MessageId == messageId);

            if (cacheItem != null)
            {
                _requestQueue.Remove(cacheItem);

                cacheItem.Timer?.Dispose();

                cacheItem.TaskSource.TrySetException(exception);

                Log.Info($"#{ClientSettings.ClientSession.SessionId}: Request was processed with error", exception);
            }
            else
            {
                Log.Info($"#{ClientSettings.ClientSession.SessionId}: Callback for request with Id {messageId} wasn't found");
            }
        }

        public void ReturnException(Exception exception)
        {
            Log.Error($"#{ClientSettings.ClientSession.SessionId}: All requests was processed with error", exception);

            foreach (var cacheItem in _requestQueue)
            {
                cacheItem.Timer?.Dispose();

                cacheItem.TaskSource.TrySetException(exception);
            }
            
            _requestQueue.Clear();
        }

        public void ReturnResult(long messageId, object obj)
        {
            var cacheItem = _requestQueue.FirstOrDefault(item => item.MessageId == messageId);
            if (cacheItem != null)
            {
                _requestQueue.Remove(cacheItem);

                cacheItem.Timer?.Dispose();

                cacheItem.TaskSource.TrySetResult(obj);
            }
            else
            {
                Log.Info($"#{ClientSettings.ClientSession.SessionId}: Callback for request with Id {messageId} wasn't found");
            }
        }

        private sealed class RequestCacheItem
        {
            public long MessageId { get; set; }

            [AllowNull]
            public Timer Timer { get; set; }

            public IRequest Request { get; }

            public TaskCompletionSource<object> TaskSource { get; }

            public RequestCacheItem(IRequest request, TaskCompletionSource<object> taskSource)
            {
                Request = request;

                TaskSource = taskSource;
            }
        }
    }
}