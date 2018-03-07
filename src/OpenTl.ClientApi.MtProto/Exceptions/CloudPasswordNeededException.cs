namespace OpenTl.ClientApi.MtProto.Exceptions
{
    using System;

    public sealed class CloudPasswordNeededException : Exception
    {
        internal CloudPasswordNeededException(string msg) : base(msg)
        {
        }
    }
}