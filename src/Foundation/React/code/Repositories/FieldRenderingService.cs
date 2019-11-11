using System;
using Foundation.React.Enum;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;

namespace Foundation.React.Repositories
{
    public class FieldRenderingService : IFieldRenderingService
    {
        public virtual string Render(Item datasource, ID field, WebEditing canEdit = WebEditing.ReadOnly)
        {
            if (datasource == null)
            {
                throw new ArgumentNullException(nameof(datasource));
            }

            if (field.IsNull)
            {
                throw new ArgumentNullException(nameof(field));
            }

            var convertField = field.ToString();
            var editingParameter = (canEdit == WebEditing.CanEdit) ? "disable-web-editing=false" : "disable-web-editing=true";

            return FieldRenderer.Render(datasource, convertField, editingParameter);
        }
    }
}
