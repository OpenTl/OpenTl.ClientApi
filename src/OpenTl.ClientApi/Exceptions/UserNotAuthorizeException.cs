namespace OpenTl.ClientApi.Exceptions
{
    using System;

    public class UserNotAuthorizeException: Exception
    {
        public UserNotAuthorizeException(): base("Authorize user first!")
        {
        }
    }
}