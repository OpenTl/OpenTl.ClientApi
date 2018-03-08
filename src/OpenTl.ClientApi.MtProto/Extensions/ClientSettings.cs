namespace OpenTl.ClientApi.MtProto.Extensions
{
    internal static class ClientSettings
    {
        public static bool ConnectionWasInitialize(this IClientSettings clientSettings)
        {
            return clientSettings.ClientSession.SessionWasHandshaked() && clientSettings.Config != null;
        }
    }
}