using System.Web.Mvc;
using React;
using Foundation.React;
using Foundation.React.Configuration;
using Foundation.React.Mvc;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.V8;
using Newtonsoft.Json.Serialization;
using System.Web.Hosting;
using System.IO;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Web.Optimization.React;
using System.Web.Optimization;
using System.Collections.Generic;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ReactConfig), "Configure")]

namespace Foundation.React
{
    [ExcludeFromCodeCoverage]
    public static class ReactConfig
    {
        public static void Configure()
        {
            // Initialize the JavaScript Engine for the application. Default to the V8 engine
            IJsEngineSwitcher engineSwitcher = JsEngineSwitcher.Current;
            engineSwitcher.EngineFactories.Clear();
            engineSwitcher.EngineFactories.AddV8(new V8Settings());

            engineSwitcher.DefaultEngineName = V8JsEngine.EngineName;

            ViewEngines.Engines.Add(new JsxViewEngine());
            ReactSiteConfiguration.Configuration.SetReuseJavaScriptEngines(true);

            if (ReactSettingsProvider.Current.UseCamelCasePropertyNames)
            {
                ReactSiteConfiguration.Configuration.JsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            if (ReactSettingsProvider.Current.BundleType == "webpack")
            {
                ReactSiteConfiguration.Configuration
                    .SetUseDebugReact(ReactSettingsProvider.Current.UseDebugReactScript)
                    .SetLoadBabel(false)
                    .AddScriptWithoutTransform(ReactSettingsProvider.Current.ServerScript);
            }
            else
            {
                var searchPath = string.Format("{0}/Views/", HostingEnvironment.ApplicationPhysicalPath);
                var jsxViews = Directory.GetFiles(searchPath, "*.jsx", SearchOption.AllDirectories)
                    .Select(view => view.Replace(HostingEnvironment.ApplicationPhysicalPath, @"~/"));

                ReactSiteConfiguration.Configuration
                    .SetUseDebugReact(ReactSettingsProvider.Current.UseDebugReactScript)
                    .SetLoadBabel(true);

                if (!ReactSettingsProvider.Current.UseDebugReactScript)
                {
                    ReactSiteConfiguration.Configuration.BabelConfig.Presets = new HashSet<string> { "react", "es2015" };
                }

                // Create the bundle for the render
                var bundle = new BabelBundle(ReactSettingsProvider.Current.BundleName);
                foreach (var view in jsxViews)
                {
                    Sitecore.Diagnostics.Log.Info($"Sitecore.React: Adding view [{view}]", typeof(ReactConfig));
                    bundle.Include(view);
                    if (!ReactSiteConfiguration.Configuration.Scripts.Any(script => script.Equals(view)))
                    {
                        ReactSiteConfiguration.Configuration.AddScript(view);
                    }
                }

                BundleTable.Bundles.Add(bundle);
            }
        }
    }
}
