namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal sealed class RpcResultHandler : SimpleChannelInboundHandler<TRpcResult>,
                                      IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RpcResultHandler));

        public int Order { get; } = 25;

        public override bool IsSharable { get; } = true;
        
        public IClientSettings ClientSettings { get; set; }
        
        public IRequestService RequestService { get; set; }

        public IUnzippedService UnzippedService { get; set; }
        
        public ISessionWriter SessionWriter { get; set; }

        protected override void ChannelRead0(IChannelHandlerContext ctx, TRpcResult msg)
        {
            Log.Debug($"Process RpcResult  with request id = '{msg.ReqMsgId}'");

            switch (msg.Result)
            {
                case TRpcError error:
                    SendConfirm(ctx, msg);

                    HandleRpcError(ctx, msg.ReqMsgId, error);
                    break;
                case TgZipPacked zipPacked:
                    Log.Debug($"Try unzip");

                    var obj = UnzippedService.UnzipPackage(zipPacked);
                    RequestService.ReturnResult(msg.ReqMsgId, obj);
                    
                    SendConfirm(ctx, msg);
                    break;
                default:
                    RequestService.ReturnResult(msg.ReqMsgId, msg.Result);

                    SendConfirm(ctx, msg);
                    break;
            }
        }

        private static Task SendConfirm(IChannelHandlerContext ctx, TRpcResult msg)
        {
            return ctx.WriteAsync(
                new TMsgsAck
                {
                    MsgIds = new TVector<long>(msg.ReqMsgId)
                });
        }

        private void HandleRpcError(IChannelHandlerContext ctx, long messageReqMsgId, TRpcError error)
        {
            Log.Warn($"Recieve error from server: {error.ErrorMessage}");

            // Exception exception;
            switch (error.ErrorMessage)
            {
                case "PHONE_CODE_INVALID":
                    RequestService.ReturnException(messageReqMsgId, new PhoneCodeInvalidException());
                    break;
                case "SESSION_PASSWORD_NEEDED":
                    RequestService.ReturnException(messageReqMsgId, new CloudPasswordNeededException());
                    break;
                case var phoneMigrate when phoneMigrate.StartsWith("PHONE_MIGRATE_"):
                case var userMigrate when userMigrate.StartsWith("USER_MIGRATE_"):
                case var netwokMigrate when netwokMigrate.StartsWith("NETWORK_MIGRATE_"):
                    var dcNumber = int.Parse(Regex.Match(error.ErrorMessage, @"\d+").Value);
                    var dcOption = ClientSettings.Config.DcOptions.Items.Find(option => option.Id == dcNumber);

                    ClientSettings.ClientSession.AuthKey = null;
                    ClientSettings.ClientSession.ServerAddress = dcOption.IpAddress;
                    ClientSettings.ClientSession.Port = dcOption.Port;

                    ctx.Flush();

                    SessionWriter.Save(ClientSettings.ClientSession)
                        .ContinueWith(_ => ctx.DisconnectAsync());
                    break;
                case var fileMigrate when fileMigrate.StartsWith("FILE_MIGRATE_"):
                    var fileMigrateDcNumber = Regex.Match(fileMigrate, @"\d+").Value;
                    var fileMigrateDcIdx = int.Parse(fileMigrateDcNumber);
                    
                    RequestService.ReturnException(messageReqMsgId, new FileMigrationException(fileMigrateDcIdx));
                    break;
                case var floodMessage when floodMessage.StartsWith("FLOOD_WAIT_"):
                    var floodMessageTime = Regex.Match(floodMessage, @"\d+").Value;
                    var seconds = int.Parse(floodMessageTime);
                    
                    RequestService.ReturnException(messageReqMsgId, new FloodWaitException(TimeSpan.FromSeconds(seconds)));
                    break;
                default:
                    RequestService.ReturnException(messageReqMsgId, new UnhandledException(error.ErrorMessage));
                    break;
            }
        }
    }
}