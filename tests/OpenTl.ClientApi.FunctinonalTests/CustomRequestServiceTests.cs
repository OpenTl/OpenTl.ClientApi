namespace OpenTl.ClientApi.FunctinonalTests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.FunctinonalTests.Framework;
    using OpenTl.Common.Extensions;
    using OpenTl.Schema;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class CustomRequestServiceTests: TestWithAuth
    {
        private static readonly Random Random = new Random();
        
        public CustomRequestServiceTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task SendPing()
        {
            var requestPing = new RequestPing { PingId = Random.NextLong() };
            
            var pong = await ClientApi.CustomRequestsService.SendRequestAsync(requestPing, CancellationToken.None).ConfigureAwait(false);
            
            Assert.Equal(requestPing.PingId, pong.PingId);
        }
        
        [Fact]
        public async Task SendToOtherDc()
        {
            var config = await ClientApi.HelpService.GetConfig().ConfigureAwait(false);

            var otherDc = config.DcOptions.First(d => d.Id != config.ThisDc);

            var otherConfig1 = await ClientApi.CustomRequestsService.SendRequestToOtherDcAsync(otherDc.Id, async clienApi => await clienApi.HelpService.GetConfig().ConfigureAwait(false)).ConfigureAwait(false);
            var otherConfig2 = await ClientApi.CustomRequestsService.SendRequestToOtherDcAsync(otherDc.Id, async clienApi => await clienApi.HelpService.GetConfig().ConfigureAwait(false)).ConfigureAwait(false);

            var config2 = await ClientApi.HelpService.GetConfig().ConfigureAwait(false);
            
            Assert.Equal(config.ThisDc, config2.ThisDc);
            Assert.Equal(otherConfig1.ThisDc, otherConfig2.ThisDc);
        }
    }
}