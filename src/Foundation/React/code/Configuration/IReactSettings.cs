namespace Foundation.React.Configuration
{
    public interface IReactSettings
    {
        bool UseDebugReactScript { get; set; }
        string DynamicPlaceholderType { get; set; }
        string BundleName { get; set; }
        bool UseCamelCasePropertyNames { get; set; }
        bool EnableClientSide { get; set; }
        bool DisableClientSideWhenEditing { get; set; }
        string BundleType { get; set; }
        string ServerScript { get; set; }
        string ClientScript { get; set; }
        string ContainerClass { get; set; }
    }
}
