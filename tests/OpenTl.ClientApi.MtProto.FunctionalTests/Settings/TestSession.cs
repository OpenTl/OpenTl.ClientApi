namespace OpenTl.ClientApi.MtProto.FunctionalTests.Settings
{
    using System;

    using DotNetty.Common.Utilities;

    using OpenTl.Common.Auth;

    internal sealed class TestSession: IClientSession
    {
        private static readonly Random Random = new Random();

        public AuthKey AuthKey { get; set; }

        public ulong SessionId { get; } = (ulong)Random.NextLong();

        public int LastMessageId { get; set; }

        public int SequenceNumber { get; set; }

        public byte[] ServerSalt { get; set; }

        public int? UserId { get; set; }

        public string ServerAddress { get; set; }

        public int Port { get; set; }

        public int TimeOffset { get; set; }

        public IClientSession Clone()
        {
            throw new NotImplementedException();
        }
    }
}