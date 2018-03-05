namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using System;

    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class BadServerSaltHandler: SimpleChannelInboundHandler<TBadServerSalt>, IMessageHandler
    {
        public IClientSettings ClientSettings { get; set; }
        
        private static readonly ILog Log = LogManager.GetLogger(typeof(BadServerSaltHandler));
        
        protected override void ChannelRead0(IChannelHandlerContext ctx, TBadServerSalt msg)
        {
            Log.Info($"Bad server sault detected! message id = {msg.BadMsgId} ");

            ClientSettings.ClientSession.ServerSalt = BitConverter.GetBytes(msg.NewServerSalt);

            throw new NotImplementedException();

            // ResponseResultSetter.ReturnException(message.BadMsgId, new BadServerSaltException());
        }
    }
}