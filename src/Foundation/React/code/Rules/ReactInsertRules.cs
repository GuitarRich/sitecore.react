using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data.Items;
using Sitecore.Data.Masters;
using Sitecore.DependencyInjection;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Sitecore.XA.Foundation.SitecoreExtensions.Repositories;

namespace Foundation.React.Rules
{
    [ExcludeFromCodeCoverage]
    public class ReactInsertRules : InsertRule
    {
        public ReactInsertRules()
        {
        }

        public ReactInsertRules(int count)
        {
        }

        public override void Expand(List<Item> masters, Item item)
        {
            if (!item.Parent.InheritsFrom(Templates.RenderingFolder.Id))
            {
                return;
            }

            var service = ServiceLocator.ServiceProvider.GetService<IContentRepository>();
            masters.Add(service.GetItem(Templates.JsxControllerRendering.Id));
            masters.Add(service.GetItem(Templates.JsxViewRendering.Id));
        }
    }
}
