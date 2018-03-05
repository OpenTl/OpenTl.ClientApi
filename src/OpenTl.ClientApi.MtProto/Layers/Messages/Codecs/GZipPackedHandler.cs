namespace OpenTl.ClientApi.MtProto.Layers.Messages.Codecs
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

    using log4net;

    using Newtonsoft.Json;

    using OpenTl.Common.IoC;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    [SingleInstance(typeof(IMessageHandler))]
    internal class GZipPackedHandler : MessageToMessageDecoder<TgZipPacked>,
                                      IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GZipPackedHandler));

        protected override void Decode(IChannelHandlerContext context, TgZipPacked message, List<object> output)
        {
            using (var decompressStream = new MemoryStream())
            {
                using (var stream = new MemoryStream(message.PackedData))
                using (var zipStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    zipStream.CopyTo(decompressStream);
                }

                decompressStream.Position = 0;

                var buffer = PooledByteBufferAllocator.Default.Buffer();
                buffer.WriteBytes(decompressStream.ToArray());

                var unzippedObj = Serializer.Deserialize(buffer);

                if (Log.IsDebugEnabled)
                {
                    var jObject = JsonConvert.SerializeObject(unzippedObj);
                    Log.Debug($"Recived Gzip message {unzippedObj}: {jObject}");
                }

                output.Add(unzippedObj);
            }
        }
    }
}