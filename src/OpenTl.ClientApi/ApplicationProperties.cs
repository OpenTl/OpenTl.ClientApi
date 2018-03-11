namespace OpenTl.ClientApi
{
    using OpenTl.ClientApi.MtProto;

    public sealed class ApplicationProperties : IApplicationProperties
    {
        public string AppVersion { get; set; }

        public string DeviceModel { get; set; }

        public string LangCode { get; set; }

        public string LangPack { get; set; }

        public string SystemLangCode { get; set; }

        public string SystemVersion { get; set; }
    }
}