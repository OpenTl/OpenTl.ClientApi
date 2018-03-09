namespace OpenTl.ClientApi.FunctinonalTests
{
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto.FunctionalTests.Framework;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class ContactsServicesTests: TestWithAuth
    {
        public ContactsServicesTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task GetContacts()
        {
            var contacts = await TelegramClient.ContactsService.GetContactsAsync().ConfigureAwait(false);
            
            Assert.NotEmpty(contacts.Contacts.Items);
        }
        
        [Fact]
        public async Task SearchUser()
        {
            var found = await TelegramClient.ContactsService.SearchUserAsync("ZapZap").ConfigureAwait(false);
            
            Assert.NotNull(found.Users);
        }
        
        // [Fact]
        // public async Task GetStatuses()
        // {
        //     var statuses = await TelegramClient.ContactsService.GetStatusesAsync().ConfigureAwait(false);
        //     
        //     Assert.NotEmpty(statuses);
        // }
    }
}