using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using Foundation.React.Mvc;
using Sitecore.Pipelines;

namespace Foundation.React.Pipelines.Initialize
{
    [ExcludeFromCodeCoverage]
    public class AddJsxViewEngine
	{
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">
        /// The pipeline arguments.
        /// </param>
        public void Process(PipelineArgs args)
		{
		   ViewEngines.Engines.Add(new JsxViewEngine());
		}
	}
}
