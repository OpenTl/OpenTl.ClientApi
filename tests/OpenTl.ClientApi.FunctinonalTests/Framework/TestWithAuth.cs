namespace OpenTl.ClientApi.FunctinonalTests.Framework
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
            if (!ClientApi.AuthService.CurrentUserId.HasValue)
            {
                var sentCode = await ClientApi.AuthService.SendCodeAsync(PhoneNumber).ConfigureAwait(false);

                CurrentUser = await ClientApi.AuthService.SignInAsync(PhoneNumber, sentCode, PhoneCode).ConfigureAwait(false);
            }
        }
    }
}