namespace Sitecore.React.Configuration
{
	public static class Settings
	{
		public static bool UsePredictableDynamicPlaceholderNames => Sitecore.Configuration.Settings.GetBoolSetting("React.DynamicPlaceholders.PredictableNames", false);

		public static string ReactBundleName => Sitecore.Configuration.Settings.GetSetting("React.BundleName", "~/bundles/react");
	}
}