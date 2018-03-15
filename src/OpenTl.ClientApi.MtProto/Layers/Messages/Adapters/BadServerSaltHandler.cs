namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using System;

    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal sealed class BadServerSaltHandler : SimpleChannelInboundHandler<TBadServerSalt>,
                                          IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BadServerSaltHandler));

        public IClientSettings ClientSettings { get; set; }

        public IRequestService RequestService { get; set; }

        public ISessionWriter SessionWriter { get; set; }
        
        public int Order { get; } = 100;

        public override bool IsSharable { get; } = true;
        
        protected override void ChannelRead0(IChannelHandlerContext ctx, TBadServerSalt msg)
        {
            Log.Info($"#{ClientSettings.ClientSession.SessionId}: Bad server sault detected! message id = {msg.BadMsgId} ");

            ClientSettings.ClientSession.ServerSalt = BitConverter.GetBytes(msg.NewServerSalt);
           
            var request = RequestService.GetRequestToReply(msg.BadMsgId);
            
            var saveTask = SessionWriter.Save(ClientSettings.ClientSession);

            if (request != null)
            {
                saveTask.ContinueWith(_ => ctx.WriteAndFlushAsync(request));
            }
        }
    }
}