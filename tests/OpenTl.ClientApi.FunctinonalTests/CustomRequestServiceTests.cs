namespace OpenTl.ClientApi.FunctinonalTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using DotNetty.Common.Utilities;

    using OpenTl.ClientApi.MtProto.FunctionalTests.Framework;
    using OpenTl.Schema;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class CustomRequestServiceTests: BaseTest
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
    }
}