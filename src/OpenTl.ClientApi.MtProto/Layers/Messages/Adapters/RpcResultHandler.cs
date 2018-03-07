namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.ClientApi.MtProto.Extensions;
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

        protected override void ChannelRead0(IChannelHandlerContext ctx, TRpcResult msg)
        {
            Log.Debug($"Process RpcResult  with request id = '{msg.ReqMsgId}'");

            switch (msg.Result)
            {
                case TRpcError error:
                    HandleRpcError(ctx, msg.ReqMsgId, error);
                    break;

                case TgZipPacked zipPacked:
                    Log.Debug($"Try unzip");

                    var obj = UnzippedService.UnzipPackage(zipPacked);
                    RequestService.ReturnResult(msg.ReqMsgId, obj);
                    break;
                default:
                    RequestService.ReturnResult(msg.ReqMsgId, msg.Result);
                    break;
            }
        }

        private void HandleRpcError(IChannelHandlerContext ctx, long messageReqMsgId, TRpcError error)
        {
            Log.Warn($"Recieve error from server: {error.ErrorMessage}");

            // Exception exception;
            switch (error.ErrorMessage)
            {
                case "PHONE_CODE_INVALID":
                    RequestService.ReturnException(messageReqMsgId, new InvalidPhoneCodeException("The numeric code used to authenticate does not match the numeric code sent by SMS/Telegram"));
                    break;
                case "SESSION_PASSWORD_NEEDED":
                    RequestService.ReturnException(messageReqMsgId, new CloudPasswordNeededException("The numeric code used to authenticate does not match the numeric code sent by SMS/Telegram"));
                    break;
                case var phoneMigrate when phoneMigrate.StartsWith("PHONE_MIGRATE_"):
                case var userMigrate when userMigrate.StartsWith("USER_MIGRATE_"):
                case var netwokMigrate when netwokMigrate.StartsWith("NETWORK_MIGRATE_"):
                    var dcNumber = int.Parse(Regex.Match(error.ErrorMessage, @"\d+").Value);
                    var dcOption = ClientSettings.Config.DcOptions.Items.Find(option => option.Id == dcNumber);

                    ClientSettings.ClientSession.AuthKey = null;
                    ClientSettings.ClientSession.ServerAddress = dcOption.IpAddress;
                    ClientSettings.ClientSession.Port = dcOption.Port;

                    ctx.DisconnectAsync().ConfigureAwait(false);
                    
                    break;
                default:
                    RequestService.ReturnException(messageReqMsgId, new InvalidOperationException(error.ErrorMessage));
                    break;
            //     case var floodMessage when floodMessage.StartsWith("FLOOD_WAIT_"):
            //         var floodMessageTime = Regex.Match(floodMessage, @"\d+").Value;
            //         var seconds = int.Parse(floodMessageTime);
            //         exception = new FloodException(TimeSpan.FromSeconds(seconds));
            //         break;

           // // //     case var phoneMigrate when phoneMigrate.StartsWith("PHONE_MIGRATE_"):
            //         var phoneMigrateDcNumber = Regex.Match(phoneMigrate, @"\d+").Value;
            //         var phoneMigrateDcIdx = int.Parse(phoneMigrateDcNumber);
            //         exception = new PhoneMigrationException(phoneMigrateDcIdx);
            //         break;

           // // //     case var fileMigrate when fileMigrate.StartsWith("FILE_MIGRATE_"):
            //         var fileMigrateDcNumber = Regex.Match(fileMigrate, @"\d+").Value;
            //         var fileMigrateDcIdx = int.Parse(fileMigrateDcNumber);
            //         exception = new FileMigrationException(fileMigrateDcIdx);
            //         break;

           // // //     case var userMigrate when userMigrate.StartsWith("USER_MIGRATE_"):
            //         var userMigrateDcNumber = Regex.Match(userMigrate, @"\d+").Value;
            //         var userMigrateDcIdx = int.Parse(userMigrateDcNumber);
            //         exception = new UserMigrationException(userMigrateDcIdx);
            //         break;

          

           // // //     case "SESSION_PASSWORD_NEEDED":
            //         exception = new CloudPasswordNeededException("This Account has Cloud Password !");
            //         break;

           // // //     case "AUTH_RESTART":
            //         ResponseResultSetter.ReturnException(new AuthRestartException());
            //         return;

         
            }

            // ResponseResultSetter.ReturnException(messageReqMsgId, exception);
        }
    }
}