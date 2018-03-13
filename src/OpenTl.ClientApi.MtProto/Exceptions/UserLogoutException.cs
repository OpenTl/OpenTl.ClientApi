namespace OpenTl.ClientApi.MtProto.Exceptions
{
    using System;
    
    public class UserLogoutException: Exception
    {
        public UserLogoutException(): base("User logout!")
        {
        }        
    }
}