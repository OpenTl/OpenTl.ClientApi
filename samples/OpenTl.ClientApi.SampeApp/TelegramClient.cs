namespace OpenTl.ClientApi.SampeApp
{
    using System;
    using System.Threading.Tasks;

    using OpenTl.ClientApi.MtProto.Exceptions;
    using OpenTl.Schema;
    using OpenTl.Schema.Account;

    public sealed class TelegramClient
    {
        private ITelegramClient _client;

        private TUser _user;

        public async Task Init(IFactorySettings factorySettings)
        {
            _client = await ClientFactory.BuildClient(factorySettings);
            
        }

        public async Task Auth(string phone)
        {
            var sentCode = await _client.AuthService.SendCodeRequestAsync(phone).ConfigureAwait(false);

            var code = ReadLineHelper.Read("Write a code:");

            try
            {
                _user = await _client.AuthService.MakeAuthAsync(phone, sentCode.PhoneCodeHash, code).ConfigureAwait(false);
                
                Console.WriteLine($"User login. Current user is {_user.FirstName} {_user.LastName}");
            }
            catch (CloudPasswordNeededException)
            {
                var password = (TPassword)await _client.AuthService.GetPasswordSetting();

                ReadLine.PasswordMode = true;
                
                var passwordStr = ReadLineHelper.ReadPassword("Write a password:");
                ReadLine.PasswordMode = false;

                _user = await _client.AuthService.MakeAuthWithPasswordAsync(password, passwordStr).ConfigureAwait(false);
            }
            catch (PhoneCodeInvalidException ex)
            {
            }
        }
    }
}