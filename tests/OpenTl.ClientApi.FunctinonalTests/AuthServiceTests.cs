namespace OpenTl.ClientApi.FunctinonalTests
{
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto.FunctionalTests.Framework;

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
            
            Assert.True(checkedPhone.PhoneRegistered);
        }
    }
}