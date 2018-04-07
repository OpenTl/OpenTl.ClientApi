namespace OpenTl.ClientApi.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.IntegrationTests.Framework;
    using OpenTl.Schema;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class MessageStatistic
    {
        public long FromUserId { get; set; }

        public long ToUserId { get; set; }

        public int SendCount { get; set; }

        public int RecieveCount { get; set; }
    }

    public sealed class MultyClientsTests : MultyClientTest
    {

        private static readonly Random Random = new Random();

        private readonly List<MessageStatistic> _statistics = new List<MessageStatistic>();

        private int MessagesCount =>  ClientsCount * 4;

        protected override int ClientsCount { get; } = 3;

        public MultyClientsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task MultyClients()
        {
            for (var i = 0; i < ClientsCount; i++)
            {
                var client = Clients[i];

                await SendMessage(client.ClientApi, i);
            }

            await Task.Delay(MessagesCount * 2500 + 1000);

            Assert.All(_statistics, statistic => Assert.Equal(statistic.SendCount, statistic.RecieveCount));
        }

        protected override async Task HandleUpdate(IUpdates update, IClientApi clientApi, int index)
        {
            switch (update)
            {
                case TUpdateShortMessage updateShortMessage:
                    if (updateShortMessage.Message.StartsWith("Test_"))
                    {
                        var stat = _statistics.First(s => s.FromUserId == updateShortMessage.UserId && s.ToUserId == clientApi.AuthService.CurrentUserId.Value);
                        stat.RecieveCount++;

                        await Task.Delay(1500).ConfigureAwait(false);
                        await SendMessage(clientApi, index).ConfigureAwait(false);
                    }

                    break;
                case TUpdates updates:
                    break;
                case TUpdatesCombined updatesCombined:
                    break;
                case TUpdateShortChatMessage updateShortChatMessage:
                    break;
                case TUpdateShortSentMessage updateShortSentMessage:
                    break;
                case TUpdatesTooLong updatesTooLong:
                    break;
            }
        }

        private async Task SendMessage(IClientApi fromClient, int i)
        {
            int index;
            do
            {
                index = Random.Next(0, ClientsCount);
            }
            while (index == i);

            var toUser = Clients[index].User;
            var contacts = Clients[i].Contacts;

            if (contacts.Contacts.Cast<TContact>().All(c => c.UserId != toUser.Id))
            {
                await fromClient.ContactsService.ImportContactsAsync(
                    new[]
                    {
                        new TInputPhoneContact
                        {
                            ClientId = toUser.Id,
                            FirstName = "Test",
                            LastName = "Test",
                            Phone = toUser.Phone
                        }
                    }).ConfigureAwait(false);
            }

            var fromUserId = fromClient.AuthService.CurrentUserId.Value;
            var stat = _statistics.FirstOrDefault(s => s.FromUserId == fromUserId && s.ToUserId == toUser.Id);
            if (stat == null)
            {
                stat = new MessageStatistic
                       {
                           FromUserId = fromUserId,
                           ToUserId = toUser.Id
                       };
                _statistics.Add(stat);
            }

            if (_statistics.Count(s => s.FromUserId == fromUserId) != MessagesCount)
            {
                stat.SendCount++;

                await fromClient.MessagesService.SendMessageAsync(
                                    new TInputPeerUser
                                    {
                                        UserId = toUser.Id,
                                        AccessHash = toUser.AccessHash
                                    },
                                    "Test_" + Random.Next())
                                .ConfigureAwait(false);
            }
        }
    }
}