using OpenTl.Schema.Updates;

namespace OpenTl.ClientApi.Settings
{
    using System;

    using NullGuard;

    using OpenTl.ClientApi.MtProto;
    using OpenTl.Common.Auth;
    using OpenTl.Common.Extensions;

    internal sealed class ClientSession : IClientSession
    {
        private static readonly Random Random = new Random();

        [AllowNull]
        public AuthKey AuthKey { get; set; }

        public ulong SessionId { get; private set; } = (ulong)Random.NextLong();

        public int LastMessageId { get; set; }

        public int SequenceNumber { get; set; }
        
        [AllowNull]
        public TState UpdateState { get; set; }
        
        [AllowNull]
        public byte[] ServerSalt { get; set; }

        public int? UserId { get; set; }

        public string ServerAddress { get; set; }

        public int Port { get; set; }

        public int TimeOffset { get; set; }

        public IClientSession Clone()
        {
            return new ClientSession
                   {
                       SessionId = SessionId
                   };
        }
    }
}