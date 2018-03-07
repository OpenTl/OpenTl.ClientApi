namespace OpenTl.ClientApi.MtProto.Exceptions
{
    using System;

    public sealed class FloodWaitException : Exception
    {
        public TimeSpan TimeToWait { get; }

        internal FloodWaitException(TimeSpan timeToWait)
            : base($"Flood prevention. Telegram now requires your program to do requests again only after {timeToWait.TotalSeconds} seconds have passed ({nameof(TimeToWait)} property).")
        {
            TimeToWait = timeToWait;
        }
    }
}