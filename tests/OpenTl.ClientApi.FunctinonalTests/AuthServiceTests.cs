namespace OpenTl.ClientApi.FunctinonalTests
{
    using System.Threading.Tasks;

    using OpenTl.ClientApi.FunctinonalTests.Framework;
    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.Schema;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class AuthServiceTests: BaseTest
    {
        public AuthServiceTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task Authenticate()
        {
            var sentCode = await ClientApi.AuthService.SendCodeAsync(PhoneNumber).ConfigureAwait(false);

            TUser user = null;
            try
            {
                user = await ClientApi.AuthService.SignInAsync(PhoneNumber, sentCode, PhoneCode).ConfigureAwait(false);
                
            }
            catch (CloudPasswordNeededException)
            {
                //TODO: Set the cloud password here 
                const string Password = "";

                user = await ClientApi.AuthService.CheckCloudPasswordAsync(Password).ConfigureAwait(false);
            }
            catch (PhoneCodeInvalidException)
            {
            }

            Assert.NotNull(user);
        }
        
        [Fact]
        public async Task Logout()
        {
            if (!ClientApi.AuthService.CurrentUserId.HasValue)
            {
                Log.Debug("Authenticate");
                
                await Authenticate().ConfigureAwait(false);
            }

            await ClientApi.ContactsService.SearchUserAsync("test");

            await ClientApi.AuthService.LogoutAsync().ConfigureAwait(false);

            await Assert.ThrowsAsync<UserNotAuthorizeException>( () => ClientApi.ContactsService.SearchUserAsync("test"));
            
            await Authenticate().ConfigureAwait(false);
            
            await ClientApi.ContactsService.SearchUserAsync("test");
        }
    }
}