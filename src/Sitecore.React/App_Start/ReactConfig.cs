using System.Web.Mvc;
using React;
using Sitecore.React;
using Sitecore.React.Configuration;
using Sitecore.React.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ReactConfig), "Configure")]

namespace Sitecore.React
{
	public static class ReactConfig
	{
		public static void Configure()
		{
			ViewEngines.Engines.Add(new JsxViewEngine());
			ReactSiteConfiguration.Configuration.SetReuseJavaScriptEngines(true);

		    var loadBabel = true;
		    if (ReactSettingsProvider.Current.BundleType == "webpack")
		    {
		        loadBabel = false;
		    }

            ReactSiteConfiguration.Configuration
                .SetUseDebugReact(ReactSettingsProvider.Current.UseDebugReactScript)
                .SetLoadBabel(loadBabel)
                .AddScriptWithoutTransform(ReactSettingsProvider.Current.ServerScript);
		}
	}
}