namespace OpenTl.ClientApi
{
    using OpenTl.ClientApi.MtProto;

    /// <summary>Information about your application</summary>
    public sealed class ApplicationProperties : IApplicationProperties
    {
        /// <inheritdoc />
        public string AppVersion { get; set; }

        /// <inheritdoc />
        public string DeviceModel { get; set; }

        /// <inheritdoc />
        public string LangCode { get; set; }

        /// <inheritdoc />
        public string LangPack { get; set; }

        /// <inheritdoc />
        public string SystemLangCode { get; set; }

        /// <inheritdoc />
        public string SystemVersion { get; set; }
    }
}