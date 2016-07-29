using System.Web.Mvc;
using React;
using Sitecore.React;
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
		}
	}
}