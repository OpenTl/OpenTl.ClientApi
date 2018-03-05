namespace OpenTl.ClientApi.MtProto
{
    using OpenTl.Common.Interfaces;

    public interface IClientSession: ISession
    {
        string ServerAddress { get; set; }

        int Port { get; set; }

        int TimeOffset { get; set; }

        long GenerateMsgId();

        (long, int) GenerateMsgIdAndSeqNo(bool confirmed);

        byte[] ToBytes();
    }
}