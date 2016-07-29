using System.Web.Mvc;

namespace Sitecore.React.Mvc
{
	public class JsxViewEngine : BuildManagerViewEngine
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.RazorViewEngine" /> class.</summary>
		public JsxViewEngine()
			: this(null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.RazorViewEngine" /> class using the view page activator.</summary>
		/// <param name="viewPageActivator">The view page activator.</param>
		public JsxViewEngine(IViewPageActivator viewPageActivator)
			: base(viewPageActivator)
		{
			this.AreaViewLocationFormats = new []
			{
				"~/Areas/{2}/Views/{1}/{0}.jsx",
				"~/Areas/{2}/Views/Shared/{0}.jsx"
			};
			this.AreaMasterLocationFormats = new []
			{
				"~/Areas/{2}/Views/{1}/{0}.jsx",
				"~/Areas/{2}/Views/Shared/{0}.jsx"
			};
			this.AreaPartialViewLocationFormats = new []
			{
				"~/Areas/{2}/Views/{1}/{0}.jsx",
				"~/Areas/{2}/Views/Shared/{0}.jsx"
			};
			this.ViewLocationFormats = new []
			{
				"~/Views/{1}/{0}.jsx",
				"~/Views/Shared/{0}.jsx"
			};
			this.MasterLocationFormats = new []
			{
				"~/Views/{1}/{0}.jsx",
				"~/Views/Shared/{0}.jsx"
			};
			this.PartialViewLocationFormats = new []
			{
				"~/Views/{1}/{0}.jsx",
				"~/Views/Shared/{0}.jsx"
			};
			this.FileExtensions = new []
			{
				"jsx"
			};
		}

		/// <summary>Creates a partial view using the specified controller context and partial path.</summary>
		/// <returns>The partial view.</returns>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="partialPath">The path to the partial view.</param>
		protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
		{
			return new JsxView(controllerContext, partialPath, null, false, this.FileExtensions, this.ViewPageActivator)
			{
				DisplayModeProvider = this.DisplayModeProvider
			};
		}

		/// <summary>Creates a view by using the specified controller context and the paths of the view and master view.</summary>
		/// <returns>The view.</returns>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="viewPath">The path to the view.</param>
		/// <param name="masterPath">The path to the master view.</param>
		protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
		{
			return new JsxView(controllerContext, viewPath, masterPath, true, this.FileExtensions, this.ViewPageActivator)
			{
				DisplayModeProvider = this.DisplayModeProvider
			};
		}
	}
}