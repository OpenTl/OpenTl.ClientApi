using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OpenTl.ClientApi.MtProto.UnitTests")]

namespace OpenTl.ClientApi.MtProto.Bootstrapper
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using BarsGroup.CodeGuard;

    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Handlers.Logging;
    using DotNetty.Transport.Bootstrapping;
    using DotNetty.Transport.Channels;
    using DotNetty.Transport.Channels.Sockets;

    using OpenTl.ClientApi.MtProto.Layers.Handshake;
    using OpenTl.ClientApi.MtProto.Layers.Messages;
    using OpenTl.ClientApi.MtProto.Layers.Secure;
    using OpenTl.ClientApi.MtProto.Layers.Tcp;
    using OpenTl.ClientApi.MtProto.Layers.Top;
    using OpenTl.Common.IoC;

    [SingleInstance(typeof(INettyBootstrapper))]
    internal sealed class NettyBootstrapper : INettyBootstrapper
    {
        private readonly Bootstrap _bootstrap = new Bootstrap();

        private IChannel _clientChannel;

        public IClientSettings ClientSettings { get; set; }

        public IHandshakeHandler[] HandshakeHandlers { get; set; }

        public ISecureHandler[] SecureHandlers { get; set; }

        public IMessageHandler[] MessageHandlers { get; set; }

        public ITopLevelHandler[] TopLevelHandlers { get; set; }

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
                            pipeline.AddLast(new TcpLayerHandlerAdapter());
                            pipeline.AddLast(HandshakeHandlers);
                            pipeline.AddLast(SecureHandlers);
                            pipeline.AddLast(MessageHandlers.OrderBy(h => h.Order).ToArray());
                            pipeline.AddLast(TopLevelHandlers);
                            pipeline.AddLast(new LoggingHandler(LogLevel.TRACE));
                        })
                );

            _clientChannel = await _bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(ClientSettings.ClientSession.ServerAddress), ClientSettings.ClientSession.Port))
                                             .ConfigureAwait(false);
        }
    }
}