namespace OpenTl.ClientApi.SampeApp.Commands
{
    using CommandLine;

    [Verb("signin", HelpText = "Authenticate")]
    public class SignInOptions
    {
        [Option('p', "phone", Required = true, HelpText = "Phone")]
        public string Phone { get; set; }
    }
}