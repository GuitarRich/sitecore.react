using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Sitecore.Configuration;
using Sitecore.Extensions.StringExtensions;
using Sitecore.Mvc;
using Sitecore.Mvc.Presentation;
using IReactComponent = React.IReactComponent;
using IReactEnvironment = React.IReactEnvironment;
using ReactEnvironment = React.ReactEnvironment;
using ReactNotInitialisedException = React.Exceptions.ReactNotInitialisedException;
using TinyIoCResolutionException = React.TinyIoC.TinyIoCResolutionException;

namespace Sitecore.React.Mvc
{
	/// <summary>Represents the class used to create views that have Razor syntax.</summary>
	public class JsxView : BuildManagerCompiledView
	{
		/// <summary>Gets the layout or master page.</summary>
		/// <returns>The layout or master page.</returns>
		public string LayoutPath { get; private set; }

		/// <summary>Gets a value that indicates whether view start files should be executed before the view.</summary>
		/// <returns>A value that indicates whether view start files should be executed before the view.</returns>
		public bool RunViewStartPages { get; private set; }

		internal IVirtualPathFactory VirtualPathFactory { get; set; }

		internal DisplayModeProvider DisplayModeProvider { get; set; }

		/// <summary>Gets or sets the set of file extensions that will be used when looking up view start files.</summary>
		/// <returns>The set of file extensions that will be used when looking up view start files.</returns>
		public IEnumerable<string> ViewStartFileExtensions { get; private set; }

		/// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.RazorView" /> class.</summary>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="viewPath">The view path.</param>
		/// <param name="layoutPath">The layout or master page.</param>
		/// <param name="runViewStartPages">A value that indicates whether view start files should be executed before the view.</param>
		/// <param name="viewStartFileExtensions">The set of extensions that will be used when looking up view start files.</param>
		public JsxView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions)
		  : this(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions, (IViewPageActivator)null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.RazorView" /> class using the view page activator.</summary>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="viewPath">The view path.</param>
		/// <param name="layoutPath">The layout or master page.</param>
		/// <param name="runViewStartPages">A value that indicates whether view start files should be executed before the view.</param>
		/// <param name="viewStartFileExtensions">The set of extensions that will be used when looking up view start files.</param>
		/// <param name="viewPageActivator">The view page activator.</param>
		public JsxView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions, IViewPageActivator viewPageActivator)
		  : base(controllerContext, viewPath, viewPageActivator)
		{
			this.LayoutPath = layoutPath ?? string.Empty;
			this.RunViewStartPages = runViewStartPages;
			this.ViewStartFileExtensions = viewStartFileExtensions ?? Enumerable.Empty<string>();
		}


		/// <summary>Renders the specified view context by using the specified the writer object.</summary>
		/// <param name="viewContext">Information related to rendering a view, such as view data, temporary data, and form context.</param>
		/// <param name="writer">The writer object.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="viewContext" /> parameter is null.</exception>
		/// <exception cref="T:SInvalidOperationException">An instance of the view type could not be created.</exception>
		public override void Render(ViewContext viewContext, TextWriter writer)
		{
			if (viewContext == null)
			{
				throw new ArgumentNullException(nameof(viewContext));
			}

			this.RenderView(viewContext, writer, null);
		}

		/// <summary>Renders the specified view context by using the specified writer and <see cref="T:System.Web.Mvc.WebViewPage" /> instance.</summary>
		/// <param name="viewContext">The view context.</param>
		/// <param name="writer">The writer that is used to render the view to the response.</param>
		/// <param name="instance">The <see cref="T:System.Web.Mvc.WebViewPage" /> instance.</param>
		protected override void RenderView(ViewContext viewContext, TextWriter writer, object instance)
		{
			if (writer == null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			var placeholderKeys = this.GetPlaceholders(this.ViewPath);
			var componentName = Path.GetFileNameWithoutExtension(this.ViewPath)?.Replace("-", string.Empty);
			var props = this.GetProps(viewContext.ViewData.Model, placeholderKeys);

			IReactComponent reactComponent = this.Environment.CreateComponent(componentName, props);
			writer.WriteLine(reactComponent.RenderHtml());
			writer.Write("<script>");
			writer.Write(reactComponent.RenderJavaScript());
			writer.WriteLine("</script>");
		}

		private IReactEnvironment Environment
		{
			get
			{
				try
				{
					return ReactEnvironment.Current;
				}
				catch (TinyIoCResolutionException ex)
				{
					throw new ReactNotInitialisedException("ReactJS.NET has not been initialised correctly.", ex);
				}
			}
		}

		protected virtual string[] GetPlaceholders(string viewPath)
		{
			const string NoPlaceholders = "NONE";

			var placeholderRegEx = Settings.GetSetting("React.PlaceholderRegEx", @"\{this\.props\.placeholder\.([\$A-Za-z\.\-_\ ]+)}");
			var placeholderStart = Settings.GetSetting("React.PlaceholderStartsWith", @"{this.props.placeholder");
			var placeholderEnd = Settings.GetSetting("React.PlaceholderEndsWith", @"}");

			var htmlCache = Caching.CacheManager.GetHtmlCache(Context.Site);
			var cacheKey = $"$React.PlaceholderKeys.{viewPath}";
			var keys = htmlCache?.GetHtml(cacheKey);
			if (!string.IsNullOrWhiteSpace(keys))
			{
				return keys.Equals(NoPlaceholders) ? new string[0] : keys.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries);
			}

			var jsxContents = File.ReadAllText(HttpContext.Current.Server.MapPath(viewPath));
			if (string.IsNullOrWhiteSpace(jsxContents))
			{
				return new string[0];
			}

			var regex = new Regex(placeholderRegEx);
			var matches = regex.Matches(jsxContents);

			var placeholderKeys = (from Match match in matches
								   select match.Value.Replace(placeholderStart, string.Empty).Replace(placeholderEnd, string.Empty)).ToArray();

			// Make sure we set the cache with the placeholder keys. Even if there are not 
			// any keys - this should make subsequent calls faster as they will not have to 
			// do the regex search.
			htmlCache?.SetHtml(cacheKey, placeholderKeys.Any() ? string.Join(",", placeholderKeys) : NoPlaceholders);

			return placeholderKeys;
		}

		internal Rendering Rendering => RenderingContext.Current.Rendering;

		protected virtual dynamic GetProps(object viewModel, string[] placeholderKeys)
		{
			dynamic props = new ExpandoObject();
			var propsDictionary = (IDictionary<string, object>)props;

			dynamic placeholders = new ExpandoObject();
			var placeholdersDictionary = (IDictionary<string, object>)placeholders;

			propsDictionary["placeholder"] = placeholders;
			propsDictionary["data"] = viewModel;

			//var placeholdersField = this.Rendering.RenderingItem.InnerItem["Place Holders"];
			//if (string.IsNullOrEmpty(placeholdersField))
			if (!placeholderKeys.Any())
			{
				return props;
			}

			var controlId = this.Rendering.Parameters["id"] ?? string.Empty;
			dynamic placeholderId = null;

			//var placeholderKeys = placeholdersField.Split(Constants.Comma, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();
			foreach (var placeholderKey in placeholderKeys)
			{
				// TODO: add this as a config setting
				if (placeholderKey.StartsWith("$Id."))
				{
					if (placeholderId == null)
					{
						placeholderId = new ExpandoObject();
						placeholdersDictionary["$Id"] = placeholderId;
					}

					((IDictionary<string, object>)placeholderId)[placeholderKey.Mid(3)] = PageContext.Current.HtmlHelper.Sitecore().Placeholder(controlId + placeholderKey.Mid(3)).ToString();
				}
				else
				{
					placeholdersDictionary[placeholderKey] = PageContext.Current.HtmlHelper.Sitecore().Placeholder(placeholderKey).ToString();
				}
			}

			return props;
		}
	}
}
