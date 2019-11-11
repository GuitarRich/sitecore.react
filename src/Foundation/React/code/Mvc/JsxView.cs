#pragma warning disable AV1564
#pragma warning disable AV1706

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Foundation.React.Configuration;
using Microsoft.Extensions.DependencyInjection;
using React;
using Sitecore;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Extensions.StringExtensions;
using Sitecore.Mvc;
using Sitecore.Mvc.Presentation;
using IReactEnvironment = React.IReactEnvironment;
using ReactNotInitializedException = React.Exceptions.ReactNotInitialisedException;
using TinyIoCResolutionException = React.TinyIoC.TinyIoCResolutionException;

namespace Foundation.React.Mvc
{
    /// <summary>
    /// Represents the class used to create views that have Razor syntax.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class JsxView : BuildManagerCompiledView
    {
        private readonly BaseCacheManager _baseCacheManager;

		/// <summary>Gets the layout or master page.</summary>
		/// <returns>The layout or master page.</returns>
		public string LayoutPath { get; }

		/// <summary>Gets a value that indicates whether view start files should be executed before the view.</summary>
		/// <returns>A value that indicates whether view start files should be executed before the view.</returns>
		public bool RunViewStartPages { get; }

		internal DisplayModeProvider DisplayModeProvider { get; set; }

		/// <summary>Gets or sets the set of file extensions that will be used when looking up view start files.</summary>
		/// <returns>The set of file extensions that will be used when looking up view start files.</returns>
		public IEnumerable<string> ViewStartFileExtensions { get; }

		/// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.RazorView" /> class.</summary>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="viewPath">The view path.</param>
		/// <param name="layoutPath">The layout or master page.</param>
		/// <param name="runViewStartPages">A value that indicates whether view start files should be executed before the view.</param>
		/// <param name="viewStartFileExtensions">The set of extensions that will be used when looking up view start files.</param>
		public JsxView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions)
		  : this(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions, null)
        {
            _baseCacheManager = ServiceLocator.ServiceProvider.GetService<BaseCacheManager>();
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
			LayoutPath = layoutPath ?? string.Empty;
			RunViewStartPages = runViewStartPages;
			ViewStartFileExtensions = viewStartFileExtensions ?? Enumerable.Empty<string>();
            _baseCacheManager = ServiceLocator.ServiceProvider.GetService<BaseCacheManager>();
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

			RenderView(viewContext, writer, null);
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

			var placeholderKeys = GetPlaceholders(ViewPath);
			var componentName = Path.GetFileNameWithoutExtension(ViewPath)?.Replace("-", string.Empty);
			var props = GetProps(viewContext.ViewData.Model, placeholderKeys);

		    var reactComponent = GetEnvironment().CreateComponent($"{componentName}", props);
            if (!string.IsNullOrWhiteSpace(ReactSettingsProvider.Current.ContainerClass))
            {
                reactComponent.ContainerClass = ReactSettingsProvider.Current.ContainerClass;
            }

            var isEditingOverrideEnabled = ReactSettingsProvider.Current.DisableClientSideWhenEditing 
                                           && Context.PageMode.IsExperienceEditorEditing;

            if (ReactSettingsProvider.Current.EnableClientSide && !isEditingOverrideEnabled)
		    {
		        writer.WriteLine(reactComponent.RenderHtml());

		        var tagBuilder = new TagBuilder("script")
		        {
		            InnerHtml = reactComponent.RenderJavaScript(true)
		        };

		        writer.Write(Environment.NewLine);
		        writer.Write(tagBuilder.ToString());
		    }
		    else
		    {
		        writer.WriteLine(reactComponent.RenderHtml(renderServerOnly: true));
		    }
        }

		private static IReactEnvironment GetEnvironment()
		{
			try
			{
				return ReactEnvironment.Current;
			}
			catch (TinyIoCResolutionException ex)
			{
				throw new ReactNotInitializedException("ReactJS.NET has not been initialized correctly.", ex);
			}
		}

		protected virtual IList<string> GetPlaceholders(string viewPath)
		{
			const string noPlaceholders = "NONE";

		    const string placeholderSearch = @"<Placeholder\b[^>]*>";
            char[] comma = { ',' };

            var htmlCache = _baseCacheManager.GetHtmlCache(Context.Site);
            var cacheKey = $"$React.PlaceholderKeys.{viewPath}";
            var keys = htmlCache?.GetHtml(cacheKey);
            if (!string.IsNullOrWhiteSpace(keys))
            {
                return keys.Equals(noPlaceholders) ? new string[0] : keys.Split(comma, StringSplitOptions.RemoveEmptyEntries);
            }

            var placeholderKeys = new List<string>();

            var jsxContents = File.ReadAllText(HttpContext.Current.Server.MapPath(viewPath));
		    if (string.IsNullOrWhiteSpace(jsxContents))
		    {
		    	return new string[0];
		    }

		    var regex = new Regex(placeholderSearch);
		    var matches = regex.Matches(jsxContents);

		    foreach (Match match in matches)
            {
                AddPlaceholderKey(match, placeholderKeys);
            }

            // Make sure we set the cache with the placeholder keys. Even if there are not 
            // any keys - this should make subsequent calls faster as they will not have to 
            // do the regex search.
            htmlCache?.SetHtml(cacheKey, placeholderKeys.Any() ? string.Join(",", placeholderKeys) : noPlaceholders);

            return placeholderKeys;
        }

        private static void AddPlaceholderKey(Capture match, ICollection<string> placeholderKeys)
        {
            const string placeholderKey = "placeholderKey={['\"](?<name>[\\$A-Za-z0-9\\.\\-_\\ ]+)['\"]}";
            const string isDynamicPlaceholder = "isDynamic=[{]?['\"]?(?<dynamic>\\w +)['\"]?[}]?";

            var placeholderKeySearch = new Regex(placeholderKey);
            var placeholderKeyMatches = placeholderKeySearch.Matches(match.Value);

            if (placeholderKeyMatches.Count == 1)
            {
                // there should only ever be ONE
                var isDynamicPlaceholderSearch = new Regex(isDynamicPlaceholder);
                var isDynamicMatches = isDynamicPlaceholderSearch.Matches(match.Value);

                var dynamicPrefix = string.Empty;
                if (isDynamicMatches.Count == 1)
                {
                    // there should only ever be ONE
                    bool.TryParse(isDynamicMatches[0].Value, out var isDynamic);
                    dynamicPrefix = isDynamic ? "$Id." : string.Empty;
                }

                placeholderKeys.Add(dynamicPrefix + placeholderKeyMatches[0].Groups["name"].Value);
            }
        }

	    internal Rendering Rendering => RenderingContext.Current.Rendering;

        protected virtual IDictionary<string, object> GetProps(object viewModel, IList<string> placeholderKeys)
		{
			var props = new ExpandoObject();
			var propsDictionary = (IDictionary<string, object>)props;

			var placeholders = new ExpandoObject();
			var placeholdersDictionary = (IDictionary<string, object>)placeholders;

			propsDictionary["placeholder"] = placeholders;
            propsDictionary["data"] = viewModel;
            propsDictionary["isSitecore"] = true;
            propsDictionary["isEditing"] = Context.PageMode.IsExperienceEditor;

			if (!placeholderKeys.Any())
			{
				return props;
			}

			var controlId = Rendering.Parameters["id"] ?? string.Empty;
            ExpandoObject placeholderId = null;

			foreach (var placeholderKey in placeholderKeys)
			{
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
