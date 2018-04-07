namespace OpenTl.ClientApi.MtProto.Interfaces
{
    using DotNetty.Transport.Channels;

    internal interface IContextGetter
    {
        IChannelHandlerContext Context { get;}
    }
}