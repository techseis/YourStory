using YStory.Core.Domain.Subscriptions;

namespace YStory.Data.Mapping.Subscriptions
{
    public partial class SubscriptionItemMap : YStoryEntityTypeConfiguration<SubscriptionItem>
    {
        public SubscriptionItemMap()
        {
            this.ToTable("SubscriptionItem");
            this.HasKey(subscriptionItem => subscriptionItem.Id);

            this.Property(subscriptionItem => subscriptionItem.UnitPriceInclTax).HasPrecision(18, 4);
            this.Property(subscriptionItem => subscriptionItem.UnitPriceExclTax).HasPrecision(18, 4);
            this.Property(subscriptionItem => subscriptionItem.PriceInclTax).HasPrecision(18, 4);
            this.Property(subscriptionItem => subscriptionItem.PriceExclTax).HasPrecision(18, 4);
            this.Property(subscriptionItem => subscriptionItem.DiscountAmountInclTax).HasPrecision(18, 4);
            this.Property(subscriptionItem => subscriptionItem.DiscountAmountExclTax).HasPrecision(18, 4);
            this.Property(subscriptionItem => subscriptionItem.OriginalArticleCost).HasPrecision(18, 4);
            this.Property(subscriptionItem => subscriptionItem.ItemWeight).HasPrecision(18, 4);


            this.HasRequired(subscriptionItem => subscriptionItem.Subscription)
                .WithMany(o => o.SubscriptionItems)
                .HasForeignKey(subscriptionItem => subscriptionItem.SubscriptionId);

            this.HasRequired(subscriptionItem => subscriptionItem.Article)
                .WithMany()
                .HasForeignKey(subscriptionItem => subscriptionItem.ArticleId);
        }
    }
}