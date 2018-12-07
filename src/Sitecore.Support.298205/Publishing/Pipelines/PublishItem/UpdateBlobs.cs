using Sitecore.Data.Items;
using Sitecore.Publishing;
using Sitecore.Publishing.Pipelines.PublishItem;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.Support.Publishing.Pipelines.PublishItem
{
  public class UpdateBlobs : PublishItemProcessor
  {
    private static MethodInfo _copyBlobFields;

    static UpdateBlobs()
    {
      _copyBlobFields = typeof(Sitecore.Publishing.PublishHelper)
          .GetMethod("CopyBlobFields", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public override void Process(PublishItemContext context)
    {
      if (context.Action != PublishAction.PublishSharedFields)
      {
        return;
      }

      var sourceItem = context.PublishHelper.GetSourceItem(context.ItemId);
      if (sourceItem == null)
      {
        return;
      }

      var targetItem = context.PublishHelper.GetTargetItem(context.ItemId);
      if (targetItem != null)
      {
        using (new SecurityDisabler())
        using (new EditContext(targetItem))
        {
          _copyBlobFields.Invoke(context.PublishHelper, new object[] { sourceItem, targetItem });
        }
      }
    }
  }
}