using Newtonsoft.Json;

namespace OpenTl.ClientApi.SampeApp
{
    using System;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.ClientApi.SampeApp.Helpers;
    using OpenTl.Schema;

    public sealed class Client
    {
        private IClientApi _clientApi;

        private TUser _user;

        public async Task Init(IFactorySettings factorySettings)
        {
            _clientApi = await ClientFactory.BuildClientAsync(factorySettings).ConfigureAwait(false);
            
            _clientApi.UpdatesService.AutoReceiveUpdates += update =>
            {
                Console.WriteLine($"updates: {JsonConvert.SerializeObject(update)}");
            };

            if (_clientApi.AuthService.CurrentUserId.HasValue)
            {
                _clientApi.UpdatesService.StartReceiveUpdates(TimeSpan.FromSeconds(1));
            }
        }

        public async Task LogOut()
        {
            await _clientApi.AuthService.LogoutAsync().ConfigureAwait(false);
        }
        
        public async Task SignIn(string phone)
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
            catch (PhoneCodeInvalidException)
            {
            }
            
             _clientApi.UpdatesService.StartReceiveUpdates(TimeSpan.FromSeconds(1));
        }
    }
}