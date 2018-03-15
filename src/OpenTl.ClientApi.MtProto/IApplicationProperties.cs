namespace OpenTl.ClientApi.MtProto
{
    /// <summary>Information about your application</summary>
    public interface IApplicationProperties
    {
        /// <summary>Application version</summary>
        string AppVersion { get; set; }

        /// <summary>Decive model</summary>
        string DeviceModel { get; set; }

        /// <summary>Language code</summary>
        string LangCode { get; set; }

        /// <summary>Language pack</summary>
        string LangPack { get; set; }

        /// <summary>System langualge code</summary>
        string SystemLangCode { get; set; }

        /// <summary>System version</summary>
        string SystemVersion { get; set; }
    }
}