using Sitecore.Data;

namespace Foundation.React
{
	public struct Templates
	{
        public struct RenderingFolder
        {
            public static readonly ID Id = new ID("{7EE0975B-0698-493E-B3A2-0B2EF33D0522}");
        }

		public struct JsxControllerRendering
		{
			public static readonly ID Id = new ID("{F31DB7E6-605C-4EF3-84C8-4238961F5649}");

			public struct Fields
			{
				public static readonly ID JsxFile = new ID("{32DC983F-CD27-4644-8A40-41F86AA464E4}");
			}
		}

		public struct  JsxViewRendering
		{
			public static readonly ID Id = new ID("{2AEC5454-4C78-4B37-96FA-8BFCB25758C6}");

			public struct Fields
			{
				public static readonly ID Path = new ID("{51B435BC-F7B9-478A-9C51-52916AF96FF5}");
			}
		}
    }
}
