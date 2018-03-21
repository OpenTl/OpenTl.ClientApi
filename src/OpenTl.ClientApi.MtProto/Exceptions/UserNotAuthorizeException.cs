namespace OpenTl.ClientApi.MtProto.Exceptions
{
    using System;

    public sealed class UserNotAuthorizeException: Exception
    {
        public UserNotAuthorizeException(): base("Authorize user first!")
        {
        }
    }
}