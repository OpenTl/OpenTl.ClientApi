namespace OpenTl.ClientApi.MtProto.Exceptions
{
    using System;

    public sealed class PhoneNumberInvalidException : Exception
    {
        internal PhoneNumberInvalidException() : base("Phone number is invalid or not registered on the server")
        {
        }
    }
}