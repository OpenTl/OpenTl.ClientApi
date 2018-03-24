namespace OpenTl.ClientApi.FunctinonalTests
{
    using System;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.FunctinonalTests.Framework;
    using OpenTl.Schema;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class MessageseTests : TestWithAuth
    {
        private readonly Random _random = new Random();

        public MessageseTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task UpdatesTest()
        {
            var message = _random.Next().ToString();

            ClientApi.UpdatesService.RecieveUpdates += update => Task.CompletedTask;

            await ClientApi.MessagesService.SendMessageAsync(new TInputPeerUser { UserId = ClientApi.AuthService.CurrentUserId.Value }, message).ConfigureAwait(false);
            await Task.Delay(3000).ConfigureAwait(false);
        }
    }
}