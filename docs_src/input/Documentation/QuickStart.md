## You can get the settings for Telegram [here](https://my.telegram.org/apps)

## Init connection and authenticate

``` C#
using OpenTl.ClientApi;
using OpenTl.ClientApi.MtProto.Exceptions;
using OpenTl.Schema;

var settings = new FactorySettings
    {
        AppHash = //e.g 456a6654ad8f52c54bc4542505884cad
        AppId = // e.g 12345
        ServerAddress = //e.g 149.154.167.50
        ServerPublicKey = // e.g -----BEGIN RSA PUBLIC KEY-----\nMIIBCgKCAQEAwVACPi9w23mF3tBk...
        ServerPort = // e.g 443
        SessionTag = "session", // by defaut
        Properties = new ApplicationProperties
                        {
                            AppVersion = "1.0.0", // You can leave as in the example
                            DeviceModel = "PC", // You can leave as in the example
                            LangCode = "en", // You can leave as in the example
                            LangPack = "tdesktop", // You can leave as in the example
                            SystemLangCode = "en", // You can leave as in the example
                            SystemVersion = "Win 10 Pro" // You can leave as in the example
                        }
    };

//Create the client
var clientApi = await ClientFactory.BuildClient(settings).ConfigureAwait(false);

// If the user is not authenticated
if (!clientApi.AuthService.CurrentUserId.HasValue){
    // Auth
    var phone = "+7555555555"; // User phone number with plus
    var sentCode = await clientApi.AuthService.SendCodeAsync(phone).ConfigureAwait(false);

    var code = "1234321" // Sent code

    TUser user;
    try
    {
        user = await clientApi.AuthService.SignInAsync(phone, sentCode, code).ConfigureAwait(false);
    }
    // If the user has a cloud password
    catch (CloudPasswordNeededException)
    {
        var passwordStr = "qweasd" // User cloud password
        user = await clientApi.AuthService.CheckCloudPasswordAsync(passwordStr).ConfigureAwait(false);
    }
    // If the phone code is invalid
    catch (PhoneCodeInvalidException ex)
    {
    }
}
```

## Send the message

``` C#
// Send message to myself
await clientApi.MessagesService.SendMessageAsync(new TInputPeerSelf(), "Hello").ConfigureAwait(false);

// Send message to user. User must be added to contacts. See ContactsService.ImportContactsAsync
TUser toUser = null; // Must be set
var inputUser = new TInputPeerUser
                {
                    UserId = toUser.Id,
                    AccessHash = toUser.AccessHash
                }
await clientApi.MessagesService.SendMessageAsync(inputUser, "Hello").ConfigureAwait(false);
```

## Recive message

### Subscribe to event UpdatesService.RecieveUpdates

The additional logic from the developer is not required. Updates come at once. If you miss an update, it will no longer come

``` C#
// Send message to myself
await clientApi.UpdatesService += update =>
{
    // handle updates
     switch (update)
        {
            case TUpdates updates:
                break;
            case TUpdatesCombined updatesCombined:
                break;
            case TUpdateShort updateShort:
                break;
            case TUpdateShortChatMessage updateShortChatMessage:
                break;
            case TUpdateShortMessage updateShortMessage:
                break;
            case TUpdateShortSentMessage updateShortSentMessage:
                break;
            case TUpdatesTooLong updatesTooLong:
                break;
        }
};
```

### Receive updates by states (manually)

It does not depend on whether there is a connection or not. The application can be turned off. The developer implements the logic of updating the status and requesting updates himself.

``` C#
// Get the current state
var currentState = await clientApi.UpdatesService.GetCurrentStateAsync().ConfigureAwait(false);

// ...
// Any time after
// ...

// Get updates from the state
var updatesFromState = await client.UpdatesService.GetUpdatesAsync(currentState).ConfigureAwait(false);

 // handle updates
switch (updatesFromState)
{
    case TUpdates updates:
        break;
    case TUpdatesCombined updatesCombined:
        break;
    case TUpdateShort updateShort:
        break;
    case TUpdateShortChatMessage updateShortChatMessage:
        break;
    case TUpdateShortMessage updateShortMessage:
        break;
    case TUpdateShortSentMessage updateShortSentMessage:
        break;
    case TUpdatesTooLong updatesTooLong:
        break;
}
```