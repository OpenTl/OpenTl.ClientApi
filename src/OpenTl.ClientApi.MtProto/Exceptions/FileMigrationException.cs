namespace OpenTl.ClientApi.MtProto.Exceptions
{
    using System;

    public sealed class FileMigrationException : Exception
    {
        public int Dc { get; set; }
        
        internal FileMigrationException(int dc)
            : base($"File located on a different DC: {dc}.")
        {
            Dc = dc;
        }
    }
}