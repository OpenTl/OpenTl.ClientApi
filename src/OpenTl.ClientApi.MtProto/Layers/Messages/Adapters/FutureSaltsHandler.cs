namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using System;

    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class FutureSaltsHandler : SimpleChannelInboundHandler<TFutureSalts>,
                                        IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FutureSaltsHandler));

        public int Order { get; } = 100;

        public override bool IsSharable { get; } = true;
        
        protected override void ChannelRead0(IChannelHandlerContext ctx, TFutureSalts msg)
        {
            Log.Debug($"Handle Future Salts for request {msg.ReqMsgId}");

            throw new NotImplementedException("The future sault does not supported yet");
        }
    }
}
