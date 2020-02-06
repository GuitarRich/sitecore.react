namespace Foundation.React.Configuration
{
    public class ReactSettings : IReactSettings
    {
        public bool UseDebugReactScript { get; set; }
        public string DynamicPlaceholderType { get; set; }
        public string BundleName { get; set; }
        public bool UseCamelCasePropertyNames { get; set; }
        public bool EnableClientSide { get; set; }
        public bool DisableClientSideWhenEditing { get; set; }
        public string BundleType { get; set; }
        public string ServerScript { get; set; }
        public string ClientScript { get; set; }
        public string ContainerClass { get; set; }
    }
}
