namespace OpenTl.ClientApi.MtProto.Extensions
{
    using System;

    internal static class SessionExtentions
    {
        public static bool SessionWasHandshaked(this IClientSession session)
        {
            return session.AuthKey != null;
        }
        
        public static long GenerateMessageId(this IClientSession session)
        {
            if (session.LastMessageId >= 4194303 - 4)
            {
                session.LastMessageId = 0;
            }
            else
            {
                session.LastMessageId += 4;
            }

            var seconds = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

            var newMessageId =
                ((seconds / 1000 + session.TimeOffset) << 32) |
                ((seconds % 1000) << 22) |
                session.LastMessageId;

            return newMessageId;
        }

        public static int GenerateSequenceNumber(this IClientSession session, bool confirmed)
        {
            return confirmed
                       ? session.SequenceNumber++ * 2 + 1
                       : session.SequenceNumber * 2;
        }
    }
}