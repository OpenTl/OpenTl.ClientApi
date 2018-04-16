namespace OpenTl.ClientApi.MtProto.UnitTests.Layers.Messages
{
    using System.IO;
    using System.IO.Compression;

    using DotNetty.Transport.Channels.Embedded;

    using OpenTl.ClientApi.MtProto.Layers.Messages.Adapters;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework.Builders;
    using OpenTl.Common.Extensions;
    using OpenTl.Common.Testing;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    using Xunit;

    public sealed class TgZipPackedHandlerTest : UnitTest
    {
        [Fact]
        public void ZipInContainer()
        {
            this.BuildClientSettingsProps();

            this.RegisterType<MsgContainerHandler>();
            this.RegisterType<GZipPackedHandler>();

            var contanerMessage = new MsgContainer
                                  {
                                      Messages = new[]
                                                 {
                                                     new TContainerMessage
                                                     {
                                                         Body = new TgZipPacked
                                                                {
                                                                    PackedData = ZipObject(new TBoolTrue())
                                                                }
                                                     }
                                                 }
                                  };

            // ---
            var channel = new EmbeddedChannel(this.Resolve<MsgContainerHandler>(), this.Resolve<GZipPackedHandler>());

            channel.WriteInbound(contanerMessage);

            // ---

            Assert.NotNull(channel.ReadInbound<TBoolTrue>());
        }

        private byte[] ZipObject(IObject o)
        {
            var buffer = Serializer.Serialize(o);
            using (var input = new MemoryStream(buffer.ToArray()))
            using (var output = new MemoryStream())
            using (var zipStream = new GZipStream(output, CompressionMode.Compress))
            {
                input.CopyTo(zipStream);
                zipStream.Flush();
                
                output.Position = 0;

                buffer.Release();
                return output.ToArray();
            }
        }
    }
}