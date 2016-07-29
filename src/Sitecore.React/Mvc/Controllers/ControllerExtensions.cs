using System.Web.Mvc;

namespace Sitecore.React.Mvc.Controllers
{
	public static class ControllerExtensions
	{
		public static JsxResult React(this Controller controller, string viewName, object model)
		{
			return controller.React(viewName, model, viewName);
		}

		public static JsxResult React(this Controller controller, string viewName, object model, string component)
		{
			if (model != null)
			{
				controller.ViewData.Model = model;
			}

			var jsxResult = new JsxResult
			{
				ViewName = viewName,
				ViewData = controller.ViewData,
				ViewEngineCollection = controller.ViewEngineCollection,
				ComponentName = component
			};

			return jsxResult;
		}
	}
}