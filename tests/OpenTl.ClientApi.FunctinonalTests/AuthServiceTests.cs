namespace OpenTl.ClientApi.FunctinonalTests
{
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.ClientApi.MtProto.FunctionalTests.Framework;
    using OpenTl.Schema;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class AuthServiceTests: BaseTest
    {
        public AuthServiceTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task IsPhoneRegistered()
        {
            var checkedPhone = await TelegramClient.AuthService.IsPhoneRegisteredAsync(PhoneNumber);
            
            Assert.True(checkedPhone);
        }
        
        [Fact]
        public async Task Authenticate()
        {
            var sentCode = await TelegramClient.AuthService.SendCodeAsync(PhoneNumber).ConfigureAwait(false);

            TUser user = null;
            try
            {
                user = await TelegramClient.AuthService.SignInAsync(PhoneNumber, sentCode, PhoneCode).ConfigureAwait(false);
                
            }
            catch (CloudPasswordNeededException)
            {
                //TODO: Set the cloud password here 
                const string Password = "";

                user = await TelegramClient.AuthService.CheckCloudPasswordAsync(Password).ConfigureAwait(false);
            }
            catch (PhoneCodeInvalidException ex)
            {
            }
            

            Assert.NotNull(user);
        }
    }
}