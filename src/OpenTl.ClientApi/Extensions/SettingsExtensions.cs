namespace OpenTl.ClientApi.Extensions
{
    using System;

    using OpenTl.ClientApi.MtProto;

    public static class SettingsExtensions
    {
        public static void EnsureUserAuthorized(this IClientSettings clientSession)
        {
            if (!clientSession.IsUserAuthorized())
            {
                throw new InvalidOperationException("Authorize user first!");
            }
        }

        public static bool IsUserAuthorized(this IClientSettings clientSession)
        {
            return clientSession.ClientSession.UserId.HasValue;
        }
    }
}