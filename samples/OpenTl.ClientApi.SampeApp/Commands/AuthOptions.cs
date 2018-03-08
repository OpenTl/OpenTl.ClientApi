namespace OpenTl.ClientApi.SampeApp.Commands
{
    using CommandLine;

    [Verb("auth", HelpText = "Authenticate")]
    public class AuthOptions
    {
        [Option('p', "phone", Required = true, HelpText = "Phone")]
        public string Phone { get; set; }
    }
}