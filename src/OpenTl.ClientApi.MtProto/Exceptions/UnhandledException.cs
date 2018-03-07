namespace OpenTl.ClientApi.MtProto.Exceptions
{
    using System;

    public sealed class UnhandledException: Exception
    {
        public UnhandledException(string messsage): base(messsage)
        {
        }
    }
}