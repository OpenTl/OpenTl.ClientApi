namespace OpenTl.ClientApi.MtProto.Layers.Handshake.Adapters
{
    using System;

    using BarsGroup.CodeGuard;

    using DotNetty.Transport.Channels;

    using log4net;

    using OpenTl.Common.Auth;
    using OpenTl.Common.Auth.Client;
    using OpenTl.Common.GuardExtensions;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;

    [SingleInstance(typeof(IHandshakeHandler))]
    internal class HandshakeHandlerAdapter: SimpleChannelInboundHandler<IObject>,
                                          IHandshakeHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HandshakeHandlerAdapter));

        private byte[] _newNonce;

        private byte[] _clientAgree;

        private byte[] _nonce;

        public IClientSettings ClientSettings { get; set; }

        public override async void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);

            if (ClientSettings.ClientSession.AuthKey == null)
            {
                Log.Info("Try do authentication");

                var step1 = Step1ClientHelper.GetRequest();
                
                _nonce = step1.Nonce;
                
                await context.WriteAndFlushAsync(step1);
            }
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IObject msg)
        {
            switch (msg)
            {
                case TResPQ resPq:
                    Guard.That(resPq.Nonce).IsItemsEquals(_nonce);
                    
                    var requestReqDhParams = Step2ClientHelper.GetRequest(resPq, ClientSettings.PublicKey, out _newNonce);
                    ctx.WriteAndFlushAsync(requestReqDhParams);
                    break;
                case TServerDHParamsOk dhParamsOk:
                    var request = Step3ClientHelper.GetRequest(dhParamsOk, _newNonce, out _clientAgree, out var serverTime);
                    ClientSettings.ClientSession.TimeOffset = serverTime - (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                    
                    ctx.WriteAndFlushAsync(request);
                    break;
                case TDhGenOk dhGenOk:
                    ClientSettings.ClientSession.AuthKey = new AuthKey(_clientAgree);
                    ClientSettings.ClientSession.ServerSalt = SaltHelper.ComputeSalt(_newNonce, dhGenOk.ServerNonce);
                    ctx.FireChannelRead(msg);
                    break;
                case TServerDHParamsFail _:
                case TDhGenRetry _:
                case TDhGenFail _:
                    throw new NotSupportedException();
                default:
                    ctx.FireChannelRead(msg);
                    break;
            }
        }
    }
}