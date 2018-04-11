namespace OpenTl.ClientApi.FunctinonalTests
{
    using System;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.FunctinonalTests.Framework;
    using OpenTl.Schema;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class MessageaseTests : TestWithAuth
    {
        private readonly Random _random = new Random();

        public MessageaseTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task UpdatesTest()
        {
            ClientApi.UpdatesService.RecieveUpdates += update => Task.CompletedTask;

            await ClientApi.MessagesService.SendMessageAsync(new TInputPeerSelf(), _random.Next().ToString()).ConfigureAwait(false);
            await ClientApi.MessagesService.SendMessageAsync(new TInputPeerSelf(), _random.Next().ToString()).ConfigureAwait(false);
            await ClientApi.MessagesService.SendMessageAsync(new TInputPeerSelf(), _random.Next().ToString()).ConfigureAwait(false);
            await ClientApi.MessagesService.SendMessageAsync(new TInputPeerSelf(), _random.Next().ToString()).ConfigureAwait(false);
            await ClientApi.MessagesService.SendMessageAsync(new TInputPeerSelf(), _random.Next().ToString()).ConfigureAwait(false);
            await ClientApi.MessagesService.SendMessageAsync(new TInputPeerSelf(), _random.Next().ToString()).ConfigureAwait(false);
            await ClientApi.MessagesService.SendMessageAsync(new TInputPeerSelf(), _random.Next().ToString()).ConfigureAwait(false);
            await ClientApi.MessagesService.SendMessageAsync(new TInputPeerSelf(), _random.Next().ToString()).ConfigureAwait(false);
            await Task.Delay(3000).ConfigureAwait(false);
        }
    }
}