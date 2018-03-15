namespace OpenTl.ClientApi.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.IntegrationTests.Framework;
    using OpenTl.Schema;
    using OpenTl.Schema.Contacts;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class MessageStatistic
    {
        public long FromUserId { get; set; }
        
        public long ToUserId { get; set; }
        
        public int SendCount { get; set; }
        
        public int RecieveCount { get; set; }
    }
    
    public sealed class ClientItem
    {
        public IClientApi ClientApi { get; set; }

        public TUser User { get; set; }

        public TContacts Contacts { get; set; }
    }

    public sealed class MultyClientsTests : BaseTest
    {
        private const int ClientsCount = 4;

        private const string PhoneTemplate = "999661000";

        private const string PhoneCode = "11111";

        private static readonly Random Random = new Random();

        private int _messagesCount = ClientsCount * 4;
        
        private readonly List<ClientItem> _clients = new List<ClientItem>(ClientsCount);
        
        private readonly List<MessageStatistic> _statistics = new List<MessageStatistic>();

        public MultyClientsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task MultyClients()
        {
            await FillClients();

            for (var i = 0; i < ClientsCount; i++)
            {
                var client = _clients[i];

                await SendMessage(client.ClientApi, i);
            }

            await Task.Delay(_messagesCount * 2000 + 1000);

            Assert.All(_statistics, statistic => Assert.Equal(statistic.SendCount, statistic.RecieveCount));
        }

        private async Task FillClients()
        {
            for (var i = 0; i < ClientsCount; i++)
            {
                var clientApi = await GenerateClientApi(i).ConfigureAwait(false);

                var phoneNumber = PhoneTemplate + i;

                TUser user;

                if (!clientApi.AuthService.CurrentUserId.HasValue)
                {
                    var sentCode = await clientApi.AuthService.SendCodeAsync(phoneNumber).ConfigureAwait(false);
                    await Task.Delay(5000);
                    if (await clientApi.AuthService.IsPhoneRegisteredAsync(phoneNumber).ConfigureAwait(false))
                    {
                        user = await clientApi.AuthService.SignInAsync(phoneNumber, sentCode, PhoneCode).ConfigureAwait(false);
                    }
                    else
                    {
                        user = await clientApi.AuthService.SignUpAsync(phoneNumber, sentCode, PhoneCode, "Test", "Test").ConfigureAwait(false);
                    }
                    await Task.Delay(1000);
                }
                else
                {
                    var fullUser = await clientApi.UsersService.GetCurrentUserFull().ConfigureAwait(false);
                    user = (TUser)fullUser.User;
                }

                var contacts = await clientApi.ContactsService.GetContactsAsync().ConfigureAwait(false);

                var index = i;
                clientApi.UpdatesService.RecieveUpdates += async update => await HandleUpdate(update, clientApi, index).ConfigureAwait(false);

                _clients.Add(
                    new ClientItem
                    {
                        ClientApi = clientApi,
                        User = user,
                        Contacts = contacts
                    });
                
            }
        }

        private async Task HandleUpdate(IUpdates update, IClientApi clientApi, int index)
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

            var toUser = _clients[index].User;
            var contacts = _clients[i].Contacts;

            if (contacts.Contacts.Items.Cast<TContact>().All(c => c.UserId != toUser.Id))
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
                           ToUserId = toUser.Id,
                       };
                _statistics.Add(stat);
            }

            if (_statistics.Count(s => s.FromUserId == fromUserId) != _messagesCount)
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