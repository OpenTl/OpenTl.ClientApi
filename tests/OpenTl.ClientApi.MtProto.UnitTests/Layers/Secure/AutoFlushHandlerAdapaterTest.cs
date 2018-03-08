namespace OpenTl.ClientApi.MtProto.UnitTests.Layers.Secure
{
    using System;
    using System.Threading.Tasks;

    using DotNetty.Transport.Channels.Embedded;

    using OpenTl.ClientApi.MtProto.Layers.Secure.Adapters;
    using OpenTl.ClientApi.MtProto.UnitTests.Framework;
    using OpenTl.Common.Testing;

    using Xunit;

    public sealed class AutoFlushHandlerAdapaterTest: UnitTest
    {

        [Fact]
        public void DontSendWithoutFlush()
        {
            this.RegisterType<AutoFlushHandlerAdapater>();

            var requestEncoder = this.Resolve<AutoFlushHandlerAdapater>();

            var channel = new EmbeddedChannel(requestEncoder);

            // ---

            channel.WriteAsync(new object());

            // ---
            
            Assert.Null(channel.ReadOutbound<object>());
        }
        
        [Fact]
        public void SendWithFlush()
        {
            this.RegisterType<AutoFlushHandlerAdapater>();

            var requestEncoder = this.Resolve<AutoFlushHandlerAdapater>();

            var channel = new EmbeddedChannel(requestEncoder);

            // ---

            channel.WriteAndFlushAsync(new object());

            // ---
            
            Assert.NotNull(channel.ReadOutbound<object>());
        }
        
        [Fact]
        public async Task SendWithFlushByTimeOut()
        {
            this.RegisterType<AutoFlushHandlerAdapater>();

            var requestEncoder = this.Resolve<AutoFlushHandlerAdapater>();
            AutoFlushHandlerAdapater.FlushInterval = TimeSpan.FromSeconds(1);
            
            var channel = new EmbeddedChannel(requestEncoder);

            // ---

#pragma warning disable 4014
            channel.WriteAsync(new object());
#pragma warning restore 4014

            await Task.Delay(2000).ConfigureAwait(false);

            // ---
            
            Assert.NotNull(channel.ReadOutbound<object>());
        }
    }
}