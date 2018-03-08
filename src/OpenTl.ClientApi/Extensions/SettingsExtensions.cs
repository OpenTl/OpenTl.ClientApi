namespace OpenTl.ClientApi.Extensions
{
    using System;

    using OpenTl.ClientApi.MtProto;

    public static class SettingsExtensions
    {
        public static bool IsUserAuthorized(this IClientSettings clientSession) => clientSession.ClientSession.UserId.HasValue;
        
        public static void EnsureUserAuthorized(this IClientSettings clientSession)
        {
            if (!clientSession.IsUserAuthorized())
            {
                throw new InvalidOperationException("Authorize user first!");
            }
        }
    }
}