using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace Foundation.React.Mvc
{
    [ExcludeFromCodeCoverage]
    public class JsxResult : ViewResult
	{
		public string ComponentName { get; set; }
	}
}
