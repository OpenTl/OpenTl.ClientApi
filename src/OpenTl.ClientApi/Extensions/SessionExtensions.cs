namespace OpenTl.ClientApi.Extensions
{
    using System.IO;

    using Newtonsoft.Json;

    using OpenTl.ClientApi.MtProto;

    internal static class SessionExtensions
    {
        public static void FromBytes(this IClientSession clientSession, byte[] buffer)
        {
            if (buffer == null)
            {
                return;
            }

            using (var inputStream = new MemoryStream(buffer))

                // using (var zippedStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(inputStream))
            {
                var settings = streamReader.ReadToEnd();
                JsonConvert.PopulateObject(settings, clientSession);
            }
        }

        public static byte[] ToBytes(this IClientSession clientSession)
        {
            using (var outputStream = new MemoryStream())

                // using (var zippedStream = new GZipStream(output, CompressionMode.Compress))
            using (var streamWriter = new StreamWriter(outputStream))
            using (var jWriter = new JsonTextWriter(streamWriter))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(jWriter, clientSession);
                jWriter.Flush();
                streamWriter.Flush();

                return outputStream.ToArray();
            }
        }
    }
}