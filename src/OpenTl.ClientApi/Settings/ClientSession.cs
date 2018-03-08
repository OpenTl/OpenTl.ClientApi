namespace OpenTl.ClientApi.Settings
{
    using System;

    using DotNetty.Common.Utilities;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.Common.Auth;
    
    using NullGuard;

    internal sealed class ClientSession: IClientSession
    {
        private static readonly Random Random = new Random();
        
        [AllowNull]
        public AuthKey AuthKey { get; set; }

        public ulong SessionId { get; } = (ulong)Random.NextLong();

        public int LastMessageId { get; set; }

        public int SequenceNumber { get; set; }

        public byte[] ServerSalt { get; set; }

        public long? UserId { get; set; }

        public string ServerAddress { get; set; }

        public int Port { get; set; }

        public int TimeOffset { get; set; }
    }
}