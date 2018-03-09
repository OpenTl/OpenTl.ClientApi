namespace OpenTl.ClientApi.MtProto.FunctionalTests.Framework
{
    using System.Threading.Tasks;

    using OpenTl.Schema;

    using Xunit.Abstractions;

    public class TestWithAuth: BaseTest
    {
        protected TUser CurrentUser { get; private set; }
        
        public TestWithAuth(ITestOutputHelper output) : base(output)
        {
            PrepareToTesting().Wait();
        }

         private async Task PrepareToTesting()
        {
            if (!TelegramClient.AuthService.CurrentUserId.HasValue)
            {
                var sentCode = await TelegramClient.AuthService.SendCodeAsync(PhoneNumber).ConfigureAwait(false);

                CurrentUser = await TelegramClient.AuthService.SignInAsync(PhoneNumber, sentCode, PhoneCode).ConfigureAwait(false);
            }
        }
    }
}