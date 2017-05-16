namespace Sitecore.React.Configuration
{
    public interface IReactSettings
    {
        bool UseDebugReactScript { get; set; }
        string DynamicPlaceholderType { get; set; }
        string BundleName { get; set; }
        bool EnableClientside { get; set; }
        string BundleType { get; set; }
        string ServerScript { get; set; }
        string ClientScript { get; set; }
    }
}
