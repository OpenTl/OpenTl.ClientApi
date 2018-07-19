namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using DotNetty.Transport.Channels;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal sealed class UpdatesHandler : SimpleChannelInboundHandler<IUpdates>, IMessageHandler
    {
        public int Order { get; } = 100;

        public override bool IsSharable { get; } = true;
        
        protected override void ChannelRead0(IChannelHandlerContext ctx, IUpdates msg)
        {
        }
    }
}