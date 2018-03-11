namespace OpenTl.ClientApi.SampeApp
{
    using System;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.Schema;

    public sealed class Client
    {
        private IClientApi _clientApi;

        private TUser _user;

        public async Task Init(IFactorySettings factorySettings)
        {
            _clientApi = await ClientFactory.BuildClient(factorySettings);
            
        }

        public async Task Auth(string phone)
        {
            var sentCode = await _clientApi.AuthService.SendCodeAsync(phone).ConfigureAwait(false);

            var code = ReadLineHelper.Read("Write a code:");

            try
            {
                _user = await _clientApi.AuthService.SignInAsync(phone, sentCode, code).ConfigureAwait(false);
                
                Console.WriteLine($"User login. Current user is {_user.FirstName} {_user.LastName}");
            }
            catch (CloudPasswordNeededException)
            {

                ReadLine.PasswordMode = true;
                
                var passwordStr = ReadLineHelper.ReadPassword("Write a password:");
                ReadLine.PasswordMode = false;

                _user = await _clientApi.AuthService.CheckCloudPasswordAsync(passwordStr).ConfigureAwait(false);
            }
            catch (PhoneCodeInvalidException ex)
            {
            }
        }
    }
}