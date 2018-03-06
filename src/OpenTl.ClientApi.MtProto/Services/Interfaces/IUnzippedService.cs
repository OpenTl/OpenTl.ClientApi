namespace OpenTl.ClientApi.MtProto.Services.Interfaces
{
    using OpenTl.Schema;

    internal interface IUnzippedService
    {
        IObject UnzipPackage(TgZipPacked message);
    }
}