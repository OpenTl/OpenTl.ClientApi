namespace OpenTl.ClientApi.Services.Interfaces
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.Schema;
    using OpenTl.Schema.Upload;

    public interface IFileService
    {
        /// <summary>Download full file</summary>
        /// <param name="location">Location</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>File</returns>
        Task<byte[]> DownloadFullFileAsync(IInputFileLocation location, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Download file without handle <see cref="FileMigrationException"/></summary>
        /// <param name="location">Location</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>File</returns>
        Task<byte[]> DownloadAllFilePartsAsync(IInputFileLocation location, CancellationToken cancellationToken);

        /// <summary>Upload file</summary>
        /// <param name="name">File name</param>
        /// <param name="stream">File stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<IInputFile> UploadFileAsync(string name, Stream stream, CancellationToken cancellationToken = default(CancellationToken));
    }
}