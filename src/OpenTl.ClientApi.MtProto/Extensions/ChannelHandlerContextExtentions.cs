namespace OpenTl.ClientApi.MtProto.Extensions
{
    using System.Net;
    using System.Threading.Tasks;

    using DotNetty.Transport.Channels;

    internal static class ChannelHandlerContextExtentions
    {
        public static Task ConnectBySession(this IChannelHandlerContext context, IClientSession clientSession)
        {
            return context.Channel.ConnectAsync(new IPEndPoint(IPAddress.Parse(clientSession.ServerAddress), clientSession.Port));
        }        
    }
}