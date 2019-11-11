namespace Foundation.React.Configuration
{
    public static class ReactSettingsProvider
    {
        private static IReactSettings _reactSettings;

        public static IReactSettings Current => _reactSettings ?? (_reactSettings =
                                                   Sitecore.Configuration.Factory.CreateObject("sitecore.react", true) as ReactSettings);
    }
}
