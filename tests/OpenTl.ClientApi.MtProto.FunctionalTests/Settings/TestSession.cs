namespace OpenTl.ClientApi.MtProto.FunctionalTests.Settings
{
    using System;

    using OpenTl.Common.Auth;

    public class TestSession: IClientSession
    {
        private static readonly Random Random = new Random();

        public AuthKey AuthKey { get; set; }

        public ulong SessionId { get; } = ((ulong)Random.Next() << 32) | (ulong)Random.Next();

        public int LastMessageId { get; set; }

        public int SequenceNumber { get; set; }

        public byte[] ServerSalt { get; set; }

        public long UserId { get; set; }

        public string ServerAddress { get; set; }

        public int Port { get; set; }

        public int TimeOffset { get; set; }
    }
}