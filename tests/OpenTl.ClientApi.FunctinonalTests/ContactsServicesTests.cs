namespace OpenTl.ClientApi.FunctinonalTests
{
    using System.Threading.Tasks;

    using OpenTl.ClientApi.FunctinonalTests.Framework;

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
            var contacts = await ClientApi.ContactsService.GetContactsAsync().ConfigureAwait(false);
            
            Assert.NotEmpty(contacts.Contacts.Items);
        }
        
        [Fact]
        public async Task SearchUser()
        {
            var found = await ClientApi.ContactsService.SearchUserAsync("ZapZap").ConfigureAwait(false);
            
            Assert.NotNull(found.Users);
        }
    }
}