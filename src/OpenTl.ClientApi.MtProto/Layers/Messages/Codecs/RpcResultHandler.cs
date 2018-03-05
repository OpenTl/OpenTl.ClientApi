namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IMessageHandler))]
    internal class RpcResultHandler: SimpleChannelInboundHandler<TRpcResult>, IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(RpcResultHandler));

        protected override void ChannelRead0(IChannelHandlerContext ctx, TRpcResult msg)
        {
            Log.Debug("Handle RpcResult");

            Log.Debug($"Process RpcResult  with request id = '{msg.ReqMsgId}'");

            switch (msg.Result)
            {
                case TRpcError error:
                    HandleRpcError(msg.ReqMsgId, error);
                    break;

                case TgZipPacked zipPacked:
                    ChannelRead(ctx, zipPacked);
                    break;

                default:
                    // ResponseResultSetter.ReturnResult(msg.ReqMsgId, msg.Result);
                    break;
            }
        }
        
        private void HandleRpcError(long messageReqMsgId, TRpcError error)
        {
            // rpc_error

            // Log.Warn($"Recieve error from server: {error.ErrorMessage}");

           // // Exception exception;
            // switch (error.ErrorMessage)
            // {
            //     case var floodMessage when floodMessage.StartsWith("FLOOD_WAIT_"):
            //         var floodMessageTime = Regex.Match(floodMessage, @"\d+").Value;
            //         var seconds = int.Parse(floodMessageTime);
            //         exception = new FloodException(TimeSpan.FromSeconds(seconds));
            //         break;

           // //     case var phoneMigrate when phoneMigrate.StartsWith("PHONE_MIGRATE_"):
            //         var phoneMigrateDcNumber = Regex.Match(phoneMigrate, @"\d+").Value;
            //         var phoneMigrateDcIdx = int.Parse(phoneMigrateDcNumber);
            //         exception = new PhoneMigrationException(phoneMigrateDcIdx);
            //         break;

           // //     case var fileMigrate when fileMigrate.StartsWith("FILE_MIGRATE_"):
            //         var fileMigrateDcNumber = Regex.Match(fileMigrate, @"\d+").Value;
            //         var fileMigrateDcIdx = int.Parse(fileMigrateDcNumber);
            //         exception = new FileMigrationException(fileMigrateDcIdx);
            //         break;

           // //     case var userMigrate when userMigrate.StartsWith("USER_MIGRATE_"):
            //         var userMigrateDcNumber = Regex.Match(userMigrate, @"\d+").Value;
            //         var userMigrateDcIdx = int.Parse(userMigrateDcNumber);
            //         exception = new UserMigrationException(userMigrateDcIdx);
            //         break;

           // //     case "PHONE_CODE_INVALID":
            //         exception = new InvalidPhoneCodeException("The numeric code used to authenticate does not match the numeric code sent by SMS/Telegram");
            //         break;

           // //     case "SESSION_PASSWORD_NEEDED":
            //         exception = new CloudPasswordNeededException("This Account has Cloud Password !");
            //         break;

           // //     case "AUTH_RESTART":
            //         ResponseResultSetter.ReturnException(new AuthRestartException());
            //         return;

           // //     default:
            //         exception = new InvalidOperationException(error.ErrorMessage);
            //         break;
            // }

           // // ResponseResultSetter.ReturnException(messageReqMsgId, exception);
        }
    }
}