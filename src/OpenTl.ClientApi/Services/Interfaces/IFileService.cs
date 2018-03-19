namespace OpenTl.ClientApi.Services.Interfaces
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Schema.Upload;

    public interface IFileService
    {
        /// <summary>Download file</summary>
        /// <param name="location">Location</param>
        /// <param name="offset">Offset</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>File</returns>
        Task<IFile> DownloadFileAsync(IInputFileLocation location, int offset = 0, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Upload file</summary>
        /// <param name="name">File name</param>
        /// <param name="stream">File stream</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<IInputFile> UploadFileAsync(string name, Stream stream, CancellationToken cancellationToken = default(CancellationToken));
    }
}