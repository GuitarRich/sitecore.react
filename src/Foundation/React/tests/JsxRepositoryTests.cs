using System;
using System.Linq;
using Sitecore.Data;
using Sitecore.React.Repositories;
using Xunit;

namespace Sitecore.Foundation.Alerts.Tests
{
	public class JsxRepositoryTests
	{

		protected readonly string TestJsxFile = "~/views/test/test.jsx";
		protected readonly ID TestJsxRenderingId = new ID("{45e23770-8591-40ec-8d53-4dcc7579cb29}");

		protected readonly string Test2JsxFile = "~/views/test/test.jsx";
		protected readonly ID Test2JsxRenderingId = new ID("{28cb0dc4-9a67-4bfd-b6ce-d8014b721e99}");

		protected readonly string Test3JsxFile = "~/views/test/test.jsx";
		protected readonly ID Test3JsxRenderingId = new ID("{e8f241a9-ba28-4904-81d8-3614d6a355f7}");


		[Fact]
		public void Test_AddScript__NewRenderingId_Contains1Script()
		{
			JsxRepository.Current.AddScript(this.TestJsxFile, this.TestJsxRenderingId);

			Assert.True(JsxRepository.Current.Items.Count() == 1);
		}

		[Fact]
		public void Test_AddScript__DuplicateRenderingId_Contains1Script()
		{
			JsxRepository.Current.AddScript(this.TestJsxFile, this.TestJsxRenderingId);
			JsxRepository.Current.AddScript(this.TestJsxFile, this.TestJsxRenderingId);

			Assert.True(JsxRepository.Current.Items.Count() == 1);
		}

		[Fact]
		public void Test_AddScript__MultipleRenderings_ContainsMultipleScripts()
		{
			JsxRepository.Current.AddScript(this.TestJsxFile, this.TestJsxRenderingId);
			JsxRepository.Current.AddScript(this.Test2JsxFile, this.Test2JsxRenderingId);
			JsxRepository.Current.AddScript(this.Test3JsxFile, this.Test3JsxRenderingId);

			Assert.True(JsxRepository.Current.Items.Count() == 3);
		}
	}
}
