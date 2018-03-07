namespace OpenTl.ClientApi.MtProto.Exceptions
{
    using System;

    public sealed class CloudPasswordNeededException : Exception
    {
        internal CloudPasswordNeededException() : base("This Account has Cloud Password !")
        {
        }
    }
}