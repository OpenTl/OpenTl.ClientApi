namespace OpenTl.ClientApi.MtProto.Interfaces
{
    using DotNetty.Transport.Channels;

    public interface IContextGetter
    {
        IChannelHandlerContext Context { get;}
    }
}