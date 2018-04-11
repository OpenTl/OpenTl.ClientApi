namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using DotNetty.Common.Utilities;
    using DotNetty.Transport.Channels;

    using log4net;

    using Newtonsoft.Json;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

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
            Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Process RpcResult  with request id = '{msg.ReqMsgId}'");


            object result;
            var buffer = ctx.Allocator.Buffer(msg.Result.Length);
            try
            {
                buffer.WriteBytes(msg.Result);
                var expectedResultType = RequestService.GetExpectedResultType(msg.ReqMsgId);
                result = Serializer.Deserialize(buffer, expectedResultType);
                
                if (Log.IsDebugEnabled)
                {
                    var jMessages = JsonConvert.SerializeObject(result);
                    Log.Debug(jMessages);
                }
            }
            finally
            {
                buffer.SafeRelease();
            }
                
            switch (result)
            {
                case TRpcError error:
                    SendConfirm(ctx, msg);

                    HandleRpcError(ctx, msg.ReqMsgId, error);
                    break;
                case TgZipPacked zipPacked:
                    Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Try unzip");

                    var obj = UnzippedService.UnzipPackage(zipPacked);
                    RequestService.ReturnResult(msg.ReqMsgId, obj);
                    
                    SendConfirm(ctx, msg);
                    break;
                default:
                    RequestService.ReturnResult(msg.ReqMsgId, result);

                    SendConfirm(ctx, msg);
                    break;
            }
        }

        private static void SendConfirm(IChannelHandlerContext ctx, TRpcResult msg)
        {
            ctx.WriteAsync(
                new TMsgsAck
                {
                    MsgIds = new TVector<long>(msg.ReqMsgId)
                });
        }

        private void HandleRpcError(IChannelHandlerContext ctx, long messageReqMsgId, TRpcError error)
        {
            Log.Warn($"#{ClientSettings.ClientSession.SessionId}: Recieve error from server: {error.ErrorMessage}");
            if (Log.IsDebugEnabled)
            {
                var jMessages = JsonConvert.SerializeObject(error);
                Log.Debug(jMessages);
            }
            
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
                    var dcOption = ClientSettings.Config.DcOptions.First(option => option.Id == dcNumber);

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
                case "AUTH_KEY_UNREGISTERED":
                    ClientSettings.ClientSession.AuthKey = null;
                    ClientSettings.ClientSession.UserId = null;
                    SessionWriter.Save(ClientSettings.ClientSession).ContinueWith(_ => RequestService.ReturnException(messageReqMsgId, new UserNotAuthorizeException()));
                    break;
                default:
                    RequestService.ReturnException(messageReqMsgId, new UnhandledException(error.ErrorMessage));
                    break;
            }
        }
    }
}