using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;
using System.Web.Optimization.React;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.GetPageRendering;
using Sitecore.Mvc.Presentation;
using Sitecore.React.Configuration;
using Sitecore.React.Repositories;
using ReactSiteConfiguration = React.ReactSiteConfiguration;

namespace Sitecore.React.Pipelines.GetPageRendering
{
	public class AddJsxFiles : GetPageRenderingProcessor
	{
		public override void Process(GetPageRenderingArgs args)
		{
			this.AddRenderingAssets(args.PageContext.PageDefinition.Renderings);

			// Create the bundle for the render
			var bundle = new BabelBundle(Settings.ReactBundleName);

			foreach (var jsxFile in JsxRepository.Current.Items)
			{
				bundle.Include(jsxFile);

				if (!ReactSiteConfiguration.Configuration.Scripts.Any(s => s.Equals(jsxFile)))
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
				var renderingItem = this.GetRenderingItem(rendering);
				if (renderingItem == null)
				{
					return;
				}

				if (renderingItem.TemplateID == Templates.JsxControllerRendering.Id)
				{
					this.AddScriptAssetsFromController(renderingItem);
				}

				if (renderingItem.TemplateID == Templates.JsxViewRendering.Id)
				{
					this.AddScriptAssetsFromView(renderingItem);
				}
			}
		}

		private void AddScriptAssetsFromController(Item renderingItem)
		{
			var jsxFile = renderingItem[Templates.JsxControllerRendering.Fields.JsxFile];
			if (!string.IsNullOrWhiteSpace(jsxFile))
			{
				JsxRepository.Current.AddScript(jsxFile, renderingItem.ID);
			}
		}

		private void AddScriptAssetsFromView(Item renderingItem)
		{
			var jsxFile = renderingItem[Templates.JsxViewRendering.Fields.Path];
			if (!string.IsNullOrWhiteSpace(jsxFile))
			{
				JsxRepository.Current.AddScript(jsxFile, renderingItem.ID);
			}
		}

		private Item GetRenderingItem(Rendering rendering)
		{
			if (rendering.RenderingItem == null)
			{
				Log.Warn($"rendering.RenderingItem is null for {rendering.RenderingItemPath}", this);
				return null;
			}

			return rendering.RenderingItem.InnerItem;
		}
	}
}