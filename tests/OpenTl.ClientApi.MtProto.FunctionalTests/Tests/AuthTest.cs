namespace OpenTl.ClientApi.MtProto.FunctionalTests.Tests
{
    using System.Threading;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto.FunctionalTests.Framework;
    using OpenTl.Schema.Auth;

    using Xunit;
    using Xunit.Abstractions;

    public sealed class AuthTest : FunctionalTest
    {
        public AuthTest(ITestOutputHelper output) : base(output)
        {
        }
 
        [Fact]
        public async Task TryAuthWithMigration()
        {
            var settings = Container.Resolve<IClientSettings>();

            const string PhoneNumber = "9996620000";
            const string PhoneCode = "22222";
            
            var requestSendCode = new RequestSendCode
                          {
                              PhoneNumber = PhoneNumber,
                              ApiId = settings.AppId,
                              ApiHash = settings.AppHash
                          };
            
            var sentCode = (TSentCode) await PackageSender.SendRequestAsync(requestSendCode, CancellationToken.None).ConfigureAwait(false);
            
            var requestSignIn = new RequestSignIn
                          {
                              PhoneNumber = PhoneNumber,
                              PhoneCodeHash = sentCode.PhoneCodeHash,
                              PhoneCode = PhoneCode
                          };

            var result = (TAuthorization)await PackageSender.SendRequestAsync(requestSignIn, CancellationToken.None).ConfigureAwait(false);
        }
 
        [Fact]
        public async Task TryAuthWithoutMigration()
        {
            var settings = Container.Resolve<IClientSettings>();

            const string PhoneNumber = "9996620000";
            const string PhoneCode = "22222";
            
            var requestSendCode = new RequestSendCode
                                  {
                                      PhoneNumber = PhoneNumber,
                                      ApiId = settings.AppId,
                                      ApiHash = settings.AppHash
                                  };
            
            var sentCode = (TSentCode) await PackageSender.SendRequestAsync(requestSendCode, CancellationToken.None).ConfigureAwait(false);
            
            var requestSignIn = new RequestSignIn
                                {
                                    PhoneNumber = PhoneNumber,
                                    PhoneCodeHash = sentCode.PhoneCodeHash,
                                    PhoneCode = PhoneCode
                                };

            var result = (TAuthorization)await PackageSender.SendRequestAsync(requestSignIn, CancellationToken.None).ConfigureAwait(false);
        }

        
        protected override void Init()
        {
        }
    }
}