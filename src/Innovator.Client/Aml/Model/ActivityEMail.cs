using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Activity EMail </summary>
  public class ActivityEMail : Item, INullRelationship<Activity>, IRelationship<EMailMessage>
  {
    protected ActivityEMail() { }
    public ActivityEMail(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ActivityEMail() { Innovator.Client.Item.AddNullItem<ActivityEMail>(new ActivityEMail { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>alternate_identity</c> property of the item</summary>
    public IProperty_Item<Identity> AlternateIdentity()
    {
      return this.Property("alternate_identity");
    }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>event</c> property of the item</summary>
    public IProperty_Text Event()
    {
      return this.Property("event");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>target</c> property of the item</summary>
    public IProperty_Text Target()
    {
      return this.Property("target");
    }
  }
}