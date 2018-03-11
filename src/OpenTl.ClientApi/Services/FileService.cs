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

    using OpenTl.ClientApi.Extensions;
    using OpenTl.ClientApi.MtProto;
    using OpenTl.ClientApi.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Upload;

    [SingleInstance(typeof(IFileService))]
    internal class FileService : IFileService
    {
        private static readonly Random Random = new Random();

        private readonly int DownloadDocumentPartSize = 128 * 1024; // 128kb for document

        private readonly int DownloadPhotoPartSize = 64 * 1024; // 64kb for photo

        public IPackageSender PackageSender { get; set; }

        public IClientSettings ClientSettings { get; set; }

        public async Task<IFile> DownloadFile(IInputFileLocation location, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            ClientSettings.EnsureUserAuthorized();

            int filePartSize;
            if (location is TInputDocumentFileLocation)
            {
                filePartSize = DownloadDocumentPartSize;
            }
            else
            {
                filePartSize = DownloadPhotoPartSize;
            }

            //TODO: Add handle FileMigrationException
            return await PackageSender.SendRequestAsync(
                       new RequestGetFile
                       {
                           Location = location,
                           Limit = filePartSize,
                           Offset = offset
                       },
                       cancellationToken).ConfigureAwait(false);

            // try
            // {
            //     return await PackageSender.SendRequestAsync(
            //                new RequestGetFile
            //                {
            //                    Location = location,
            //                    Limit = filePartSize,
            //                    Offset = offset
            //                },
            //                cancellationToken).ConfigureAwait(false);
            // }
            // catch (FileMigrationException ex)
            // {
            //     var exportedAuth = (TExportedAuthorization)await PackageSender.SendRequestAsync(
            //                                                    new RequestExportAuthorization
            //                                                    {
            //                                                        DcId = ex.Dc
            //                                                    },
            //                                                    cancellationToken).ConfigureAwait(false);

            // //     var authKey = ClientSettings.Session.AuthKey;
            //     var timeOffset = ClientSettings.Session.TimeOffset;
            //     var serverAddress = ClientSettings.Session.ServerAddress;
            //     var serverPort = ClientSettings.Session.Port;

            // //     await ConnectApiService.ReconnectToDcAsync(ex.Dc).ConfigureAwait(false);
            //     await PackageSender.SendRequestAsync(
            //         new RequestImportAuthorization
            //         {
            //             Bytes = exportedAuth.Bytes,
            //             Id = exportedAuth.Id
            //         },
            //         cancellationToken).ConfigureAwait(false);
            //     var result = await GetFile(location, offset, cancellationToken).ConfigureAwait(false);

            // //     ClientSettings.Session.AuthKey = authKey;
            //     ClientSettings.Session.TimeOffset = timeOffset;
            //     ClientSettings.Session.ServerAddress = serverAddress;
            //     ClientSettings.Session.Port = serverPort;
            //     await ConnectApiService.ConnectAsync().ConfigureAwait(false);

            // //     return result;
            // }
        }

        public async Task<IInputFile> UploadFile(string name, Stream stream, CancellationToken cancellationToken = default(CancellationToken))
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
                    await PackageSender.SendRequestAsync(
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
                    await PackageSender.SendRequestAsync(
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