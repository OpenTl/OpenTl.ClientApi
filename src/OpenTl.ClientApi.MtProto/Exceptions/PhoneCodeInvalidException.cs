namespace OpenTl.ClientApi.MtProto.Exceptions
{
    using System;

    public sealed class PhoneCodeInvalidException : Exception
    {
        internal PhoneCodeInvalidException() : base("The numeric code used to authenticate does not match the numeric code sent by SMS/Telegram")
        {
        }
    }
}