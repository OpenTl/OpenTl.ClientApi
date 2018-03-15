namespace OpenTl.ClientApi.FunctinonalTests
{
    using System;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.FunctinonalTests.Framework;
    using OpenTl.Schema;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class MessageseTests: TestWithAuth
    {
        private Random _random = new Random();
        
        public MessageseTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task UpdatesTest()
        {
            var message = _random.Next().ToString();
            var recieve = false;
            
            ClientApi.UpdatesService.RecieveUpdates += update =>
            {
                return Task.CompletedTask;
            };

            await ClientApi.MessagesService.SendMessageAsync(new TInputPeerUser{UserId = ClientApi.AuthService.CurrentUserId.Value}, message);
            await Task.Delay(3000);
            
            Assert.True(recieve);
        }
    }
}