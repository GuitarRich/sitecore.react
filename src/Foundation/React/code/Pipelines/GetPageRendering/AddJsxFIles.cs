using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Optimization;
using System.Web.Optimization.React;
using Foundation.React.Configuration;
using Foundation.React.Repositories;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.GetPageRendering;
using Sitecore.Mvc.Presentation;
using ReactSiteConfiguration = React.ReactSiteConfiguration;

namespace Foundation.React.Pipelines.GetPageRendering
{
    [ExcludeFromCodeCoverage]
    public class AddJsxFiles : GetPageRenderingProcessor
	{
		public override void Process(GetPageRenderingArgs args)
		{
            // Safety check - if the config is set to webpack, don't run this pipeline or it will fail.
		    if (ReactSettingsProvider.Current.BundleType == Settings.BundleTypes.Webpack)
		    {
		        return;
		    }

			AddRenderingAssets(args.PageContext.PageDefinition.Renderings);

			// Create the bundle for the render
			var bundle = new BabelBundle(ReactSettingsProvider.Current.BundleName);

			foreach (var jsxFile in JsxRepository.Current.Items)
			{
				bundle.Include(jsxFile);

				if (!ReactSiteConfiguration.Configuration.Scripts.Any(script => script.Equals(jsxFile)))
				{
					ReactSiteConfiguration.Configuration.AddScript(jsxFile);
				}
			}

			BundleTable.Bundles.Add(bundle);
		}

		private void AddRenderingAssets(IEnumerable<Rendering> renderings)
		{
			foreach (var rendering in renderings)
			{
				var renderingItem = GetRenderingItem(rendering);
				if (renderingItem == null)
				{
					return;
				}

				if (renderingItem.TemplateID == Templates.JsxControllerRendering.Id)
				{
					AddScriptAssetsFromController(renderingItem);
				}

				if (renderingItem.TemplateID == Templates.JsxViewRendering.Id)
				{
					AddScriptAssetsFromView(renderingItem);
				}
			}
		}

		private static void AddScriptAssetsFromController(Item renderingItem)
		{
			var jsxFile = renderingItem[Templates.JsxControllerRendering.Fields.JsxFile];
			if (!string.IsNullOrWhiteSpace(jsxFile))
			{
				JsxRepository.Current.AddScript(jsxFile, renderingItem.ID);
			}
		}

		private static void AddScriptAssetsFromView(Item renderingItem)
		{
			var jsxFile = renderingItem[Templates.JsxViewRendering.Fields.Path];
			if (!string.IsNullOrWhiteSpace(jsxFile))
			{
				JsxRepository.Current.AddScript(jsxFile, renderingItem.ID);
			}
		}

		private Item GetRenderingItem(Rendering rendering)
		{
		    if (rendering.RenderingItem != null)
		    {
		        return rendering.RenderingItem.InnerItem;
		    }

		    Log.Warn($"rendering.RenderingItem is null for {rendering.RenderingItemPath}", this);
		    return null;
		}
	}
}
