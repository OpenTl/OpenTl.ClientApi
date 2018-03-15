namespace OpenTl.ClientApi.MtProto.Layers.Messages.Adapters
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

    using log4net;

    using Newtonsoft.Json;

    using OpenTl.ClientApi.MtProto.Services.Interfaces;
    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    [SingleInstance(typeof(IMessageHandler), typeof(IUnzippedService))]
    internal sealed class GZipPackedHandler : MessageToMessageDecoder<TgZipPacked>,
                                       IMessageHandler,
                                       IUnzippedService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GZipPackedHandler));

        public int Order { get; } = 50;

        public override bool IsSharable { get; } = true;
        
        public IClientSettings ClientSettings { get; set; }

        protected override void Decode(IChannelHandlerContext context, TgZipPacked message, List<object> output)
        {
            var unzippedObj = UnzipPackage(message);
            
            output.Add(unzippedObj);
        }

        public IObject UnzipPackage(TgZipPacked message)
        {
            Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Process TgZipPacked message");
            
            using (var decompressStream = new MemoryStream())
            {
                using (var stream = new MemoryStream(message.PackedData))
                using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    zipStream.CopyTo(decompressStream);
                }

                decompressStream.Position = 0;

                var buffer = PooledByteBufferAllocator.Default.Buffer();

                try
                {
                    buffer.WriteBytes(decompressStream.ToArray());

                    var unzippedObj = Serializer.Deserialize(buffer);

                    if (Log.IsDebugEnabled)
                    {
                        var jObject = JsonConvert.SerializeObject(unzippedObj);
                        Log.Debug($"#{ClientSettings.ClientSession.SessionId}: Recived Gzip message {unzippedObj}: {jObject}");
                    }

                    return unzippedObj;
                }
                finally
                {
                    buffer.Release();
                }
            }
        }
    }
}