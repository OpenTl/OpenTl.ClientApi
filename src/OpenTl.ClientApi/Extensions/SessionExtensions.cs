namespace OpenTl.ClientApi.Extensions
{
    using System.IO;
    using System.IO.Compression;

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
            using (var zippedStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(zippedStream))
            {
                JsonConvert.PopulateObject(streamReader.ReadToEnd(), clientSession);
            }
        }

        public static byte[] ToBytes(this IClientSession clientSession)
        {
            using (var inputStream = new MemoryStream())
            using (var zippedStream = new GZipStream(inputStream, CompressionMode.Compress))
            using (var streamWriter = new StreamWriter(zippedStream))
            using (var jWriter = new JsonTextWriter(streamWriter))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(jWriter, clientSession);
                
                return inputStream.ToArray();
            }
        }        
    }
}