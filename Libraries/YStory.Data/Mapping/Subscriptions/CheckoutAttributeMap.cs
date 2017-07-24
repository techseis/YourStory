using YStory.Core.Domain.Subscriptions;

namespace YStory.Data.Mapping.Subscriptions
{
    public partial class CheckoutAttributeMap : YStoryEntityTypeConfiguration<CheckoutAttribute>
    {
        public CheckoutAttributeMap()
        {
            this.ToTable("CheckoutAttribute");
            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Name).IsRequired().HasMaxLength(400);

            this.Ignore(ca => ca.AttributeControlType);
        }
    }
}