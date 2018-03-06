namespace OpenTl.ClientApi.MtProto
{
    public interface IApplicationProperties
    {
        string AppVersion { get; set; }
        
        string DeviceModel { get; set; }
        
        string LangCode { get; set; }
        
        string LangPack { get; set; }
        
        string SystemLangCode { get; set; }
        
        string SystemVersion { get; set; }
    }
}