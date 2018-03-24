namespace OpenTl.ClientApi.MtProto.FunctionalTests.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using DotNetty.Common.Utilities;

    using OpenTl.ClientApi.MtProto.FunctionalTests.Framework;
    using OpenTl.Schema.Help;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class BadServerSaultTest : FunctionalTest
    {
        private static readonly Random Random = new Random();
         
        public BadServerSaultTest(ITestOutputHelper output) : base(output)
        {
        }
 
        [Fact]
        public async Task BadServerSault()
        {
            var resultBefore = await RequestSender.SendRequestAsync(new RequestGetConfig(), CancellationToken.None).ConfigureAwait(false);

            var settings = Container.Resolve<IClientSettings>();
            
            settings.ClientSession.ServerSalt = BitConverter.GetBytes(Random.NextLong()); 

            var resultAfter = await RequestSender.SendRequestAsync(new RequestGetConfig(), CancellationToken.None).ConfigureAwait(false);

            Assert.Equal(resultBefore.MeUrlPrefix, resultAfter.MeUrlPrefix);
        }
    }
}