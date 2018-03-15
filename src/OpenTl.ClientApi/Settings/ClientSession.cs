namespace OpenTl.ClientApi.Settings
{
    using System;

    using DotNetty.Common.Utilities;

    using NullGuard;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.Common.Auth;

    internal sealed class ClientSession : IClientSession
    {
        private static readonly Random Random = new Random();

        [AllowNull]
        public AuthKey AuthKey { get; set; }

        public ulong SessionId { get; } = (ulong)Random.NextLong();

        public int LastMessageId { get; set; }

        public int SequenceNumber { get; set; }

        [AllowNull]
        public byte[] ServerSalt { get; set; }

        public int? UserId { get; set; }

        public string ServerAddress { get; set; }

        public int Port { get; set; }

        public int TimeOffset { get; set; }

        public IClientSession RecreateSession()
        {
            return new ClientSession
                   {
                       Port = Port,
                       ServerAddress = ServerAddress
                   };
        }
    }
}