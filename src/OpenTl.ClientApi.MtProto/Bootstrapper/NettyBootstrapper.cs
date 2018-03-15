namespace OpenTl.ClientApi.MtProto.Bootstrapper
{
    using System.Net;
    using System.Threading.Tasks;

    using BarsGroup.CodeGuard;

    using Castle.Windsor;

    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Handlers.Logging;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;

    using log4net;

    using OpenTl.ClientApi.MtProto.Layers.Handshake;
    using OpenTl.ClientApi.MtProto.Layers.Messages;
    using OpenTl.ClientApi.MtProto.Layers.Secure;
    using OpenTl.ClientApi.MtProto.Layers.Tcp;
    using OpenTl.ClientApi.MtProto.Layers.Top;
    using OpenTl.Common.IoC;

    [SingleInstance(typeof(INettyBootstrapper))]
    internal sealed class NettyBootstrapper : INettyBootstrapper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NettyBootstrapper));

        private readonly Bootstrap _bootstrap = new Bootstrap();

        public IWindsorContainer Container { get; set; }
        
        public IClientSettings ClientSettings { get; set; }

        public async Task Init()
        {
            Guard.That(ClientSettings.ClientSession.ServerAddress).IsNotNullOrWhiteSpace();
            Guard.That(ClientSettings.ClientSession.Port).IsNotDefault();

            _bootstrap
                .Group(new MultithreadEventLoopGroup())
                .Channel<TcpSocketChannel>()
                .Handler(
                    new ActionChannelInitializer<ISocketChannel>(
                        channel =>
                        {
                            var pipeline = channel.Pipeline;

                            pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.LittleEndian, int.MaxValue, 0, 4, -4, 0, true));
                            pipeline.AddLast(Container.ResolveAll<ITcpHandler>());
                            pipeline.AddLast(Container.ResolveAll<IHandshakeHandler>());
                            pipeline.AddLast(Container.ResolveAll<ISecureHandler>());
                            pipeline.AddLast(Container.ResolveAll<IMessageHandler>());
                            pipeline.AddLast(Container.ResolveAll<ITopLevelHandler>());
                           
                            if (Log.IsDebugEnabled)
                            {
                                pipeline.AddLast(new LoggingHandler(LogLevel.DEBUG));
                            }
                        })
                );

            await Connect();
        }

        public async Task Connect()
        {
            await _bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(ClientSettings.ClientSession.ServerAddress), ClientSettings.ClientSession.Port))
                                             .ConfigureAwait(false);
        }
    }
}