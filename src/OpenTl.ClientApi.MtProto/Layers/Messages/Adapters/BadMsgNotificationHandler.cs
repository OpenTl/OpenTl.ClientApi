namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using System;

    using DotNetty.Transport.Channels;

    using log4net;

    using Newtonsoft.Json;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal sealed class BadMsgNotificationHandler : SimpleChannelInboundHandler<TBadMsgNotification>,
                                               IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BadMsgNotificationHandler));

        public int Order { get; } = 100;

        public override bool IsSharable { get; } = true;

        public IRequestService RequestService { get; set; }

        public IClientSettings ClientSettings { get; set; }
        protected override void ChannelRead0(IChannelHandlerContext ctx, TBadMsgNotification msg)
        {
            switch (msg.ErrorCode)
            {
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 32:
                case 33:
                case 34:
                case 35:
                case 48:
                case 64:
                    var jUpdate = JsonConvert.SerializeObject(msg);
                    Log.Error($"#{ClientSettings.ClientSession.SessionId}: Handle a bad message notification for request id = {msg.BadMsgId} and seqNo = {msg.BadMsgSeqno} :\n {jUpdate}");

                    var request = RequestService.GetRequestToReply(msg.BadMsgId);
                    if (request != null)
                    {
                        ctx.WriteAndFlushAsync(request);
                    }
                    break;
                default:
                    var exception = new UnhandledException($"Wrong TBadMsgNotification code {msg.ErrorCode}");
                    Log.Error($"#{ClientSettings.ClientSession.SessionId}: Handle a bad message notification for request id = {msg.BadMsgId} and seqNo = {msg.BadMsgSeqno}", exception);

                    RequestService.ReturnException(msg.BadMsgId, exception);
                    break;
            }
        }
    }
}
