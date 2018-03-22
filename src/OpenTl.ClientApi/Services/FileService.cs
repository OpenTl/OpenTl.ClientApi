namespace OpenTl.ClientApi.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using DotNetty.Common.Utilities;

    using NullGuard;

    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Auth;
    using OpenTl.Schema.Upload;

    [SingleInstance(typeof(IFileService))]
    internal sealed class FileService : IFileService
    {
        private static readonly Random Random = new Random();

        private static readonly int DownloadDocumentPartSize = 128 * 1024; // 128kb for document

        private static readonly int DownloadPhotoPartSize = 64 * 1024; // 64kb for photo

        public ICustomRequestsService RequestService { get; set; }

        public IClientSettings ClientSettings { get; set; }

        /// <inheritdoc />
        [return:AllowNull]
        public async Task<byte[]> DownloadFullFileAsync(IInputFileLocation location, CancellationToken cancellationToken = default(CancellationToken)) 
        {
            ClientSettings.EnsureUserAuthorized();

            try
            {
                return await DownloadAllFilePartsAsync(location, cancellationToken);         
            }
            catch (FileMigrationException ex)
            {
                return await RequestService.SendRequestToOtherDcAsync(ex.Dc, api => api.FileService.DownloadAllFilePartsAsync(location, cancellationToken), cancellationToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        [return:AllowNull]
        public async Task<byte[]> DownloadAllFilePartsAsync(IInputFileLocation location, CancellationToken cancellationToken)
        {
            var filePartSize = location is TInputDocumentFileLocation
                                   ? DownloadDocumentPartSize
                                   : DownloadPhotoPartSize;
            
            var offset = 0;
            var bytes = new List<byte>();
            while (true)
            {
               
                    var file = await DownloadFilePartAsync(location, offset, cancellationToken).ConfigureAwait(false);
                    if (file == null)
                        return null;

                    bytes.AddRange(file.Bytes);
                    if (file.Bytes.Length < filePartSize)
                    {
                        return bytes.ToArray();
                    }

                    offset += file.Bytes.Length;
            }
        }

        private async Task<TFile> DownloadFilePartAsync(IInputFileLocation location, int offset, CancellationToken cancellationToken)
        {
            var filePartSize = location is TInputDocumentFileLocation
                                   ? DownloadDocumentPartSize
                                   : DownloadPhotoPartSize;

            var requestGetFile = new RequestGetFile
                                 {
                                     Location = location,
                                     Limit = filePartSize,
                                     Offset = offset
                                 };
            return (TFile) await RequestService.SendRequestAsync(requestGetFile, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IInputFile> UploadFileAsync(string name, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            const long TenMb = 10 * 1024 * 1024;
            var isBigFileUpload = stream.Length >= TenMb;

            var file = await ReadStream(stream).ConfigureAwait(false);
            var fileParts = GetFileParts(file);

            var partNumber = 0;
            var partsCount = fileParts.Count;
            var fileId = Random.NextLong();
            while (fileParts.Count != 0)
            {
                var part = fileParts.Dequeue();

                if (isBigFileUpload)
                {
                    await RequestService.SendRequestAsync(
                        new RequestSaveBigFilePart
                        {
                            FileId = fileId,
                            FilePart = partNumber,
                            Bytes = part,
                            FileTotalParts = partsCount
                        },
                        cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await RequestService.SendRequestAsync(
                        new RequestSaveFilePart
                        {
                            FileId = fileId,
                            FilePart = partNumber,
                            Bytes = part
                        },
                        cancellationToken).ConfigureAwait(false);
                }

                partNumber++;
            }

            if (isBigFileUpload)
            {
                return new TInputFileBig
                       {
                           Id = fileId,
                           Name = name,
                           Parts = partsCount
                       };
            }

            return new TInputFile
                   {
                       Id = fileId,
                       Name = name,
                       Parts = partsCount,
                       Md5Checksum = GetFileHash(file)
                   };
        }

        private static string GetFileHash(byte[] data)
        {
            string md5Checksum;
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(data);
                var hashResult = new StringBuilder(hash.Length * 2);

                foreach (var t in hash)
                {
                    hashResult.Append(t.ToString("x2"));
                }

                md5Checksum = hashResult.ToString();
            }

            return md5Checksum;
        }

        private static Queue<byte[]> GetFileParts(byte[] file)
        {
            var fileParts = new Queue<byte[]>();

            const int MaxFilePart = 512 * 1024;

            using (var stream = new MemoryStream(file))
            {
                while (stream.Position != stream.Length)
                {
                    if (stream.Length - stream.Position > MaxFilePart)
                    {
                        var temp = new byte[MaxFilePart];
                        stream.Read(temp, 0, MaxFilePart);
                        fileParts.Enqueue(temp);
                    }
                    else
                    {
                        var length = stream.Length - stream.Position;
                        var temp = new byte[length];
                        stream.Read(temp, 0, (int)length);
                        fileParts.Enqueue(temp);
                    }
                }
            }

            return fileParts;
        }

        private static async Task<byte[]> ReadStream(Stream stream)
        {
            var file = new byte[stream.Length];

            await stream.ReadAsync(file, 0, (int)stream.Length).ConfigureAwait(false);

            return file;
        }
    }
}