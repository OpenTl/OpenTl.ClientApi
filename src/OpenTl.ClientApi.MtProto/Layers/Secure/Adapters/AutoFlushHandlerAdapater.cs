namespace OpenTl.ClientApi.MtProto.Layers.Secure.Adapters
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using DotNetty.Transport.Channels;

    using OpenTl.Common.IoC;

    [SingleInstance(typeof(ISecureHandler))]
    internal sealed class AutoFlushHandlerAdapater: ChannelHandlerAdapter, ISecureHandler, IDisposable
    {
        private Timer _timer;

        internal static TimeSpan FlushInterval = TimeSpan.FromMinutes(1);

        public override bool IsSharable { get; } = true;

        public override Task WriteAsync(IChannelHandlerContext context, object message)
        {
            if (_timer == null)
            {
                _timer = new Timer(state => ((IChannelHandlerContext)state).Flush(), context, FlushInterval, TimeSpan.Zero);
            }
            else
            {
                _timer.Change(FlushInterval, TimeSpan.Zero);
            }
            
            return base.WriteAsync(context, message);
        }

        public override void Flush(IChannelHandlerContext context)
        {
            _timer?.Dispose();
            _timer = null;
            
            base.Flush(context);
        }
        
        public override Task DisconnectAsync(IChannelHandlerContext context)
        {
            _timer?.Dispose();
            _timer = null;

            return base.DisconnectAsync(context);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}