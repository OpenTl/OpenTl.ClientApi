namespace OpenTl.ClientApi.IntegrationTests.Framework
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Schema.Contacts;

    using Xunit.Abstractions;

    public sealed class ClientItem
    {
        public IClientApi ClientApi { get; set; }

        public TUser User { get; set; }

        public TContacts Contacts { get; set; }
    }
    
    public abstract class MultyClientTest: BaseTest
    {
        private const string PhoneTemplate = "999661000";

        private const string PhoneCode = "11111";

        protected abstract int ClientsCount { get; }

        protected readonly List<ClientItem> Clients = new List<ClientItem>();
        
        protected MultyClientTest(ITestOutputHelper output) : base(output)
        {
            FillClients().Wait();
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

                    await Task.Delay(5000).ConfigureAwait(false);
                        
                    if (await clientApi.AuthService.IsPhoneRegisteredAsync(phoneNumber).ConfigureAwait(false))
                    {
                        user = await clientApi.AuthService.SignInAsync(phoneNumber, sentCode, PhoneCode).ConfigureAwait(false);
                    }
                    else
                    {
                        user = await clientApi.AuthService.SignUpAsync(phoneNumber, sentCode, PhoneCode, "Test", "Test").ConfigureAwait(false);
                    }

                    await Task.Delay(3000).ConfigureAwait(false);
                }
                else
                {
                    var fullUser = await clientApi.UsersService.GetCurrentUserFullAsync().ConfigureAwait(false);
                    user = (TUser)fullUser.User;
                }

                var contacts = await clientApi.ContactsService.GetContactsAsync().ConfigureAwait(false);

                var index = i;
                clientApi.UpdatesService.RecieveUpdates += async update => await HandleUpdate(update, clientApi, index).ConfigureAwait(false);

                Clients.Add(
                    new ClientItem
                    {
                        ClientApi = clientApi,
                        User = user,
                        Contacts = contacts
                    });
            }
        }

        protected abstract Task HandleUpdate(IUpdates update, IClientApi clientApi, int index);
    }
}