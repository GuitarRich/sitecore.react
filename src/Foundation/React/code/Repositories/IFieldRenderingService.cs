using Foundation.React.Enum;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Foundation.React.Repositories
{
    public interface IFieldRenderingService
    {
        string Render(Item datasource, ID field, WebEditing canEdit = WebEditing.ReadOnly);
    }
}
