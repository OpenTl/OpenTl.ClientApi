namespace OpenTl.ClientApi.MtProto.FunctionalTests.Settings
{
    public sealed class TestApplicationProperties: IApplicationProperties
    {
        public string AppVersion { get; set; } = "1.0.0";

        public string DeviceModel { get; set; } = "PC";

        public string LangCode { get; set; } = "en";

        public string LangPack { get; set; } = "tdesktop";

        public string SystemLangCode { get; set; } = "en";

        public string SystemVersion { get; set; } = "Win 10.0";
    }
}