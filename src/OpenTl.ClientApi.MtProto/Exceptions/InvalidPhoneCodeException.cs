namespace OpenTl.ClientApi.MtProto.Exceptions
{
    using System;

    public sealed class InvalidPhoneCodeException : Exception
    {
        internal InvalidPhoneCodeException(string msg) : base(msg)
        {
        }
    }
}