using YStory.Core.Domain.Subscriptions;

namespace YStory.Data.Mapping.Subscriptions
{
    public partial class ShoppingCartItemMap : YStoryEntityTypeConfiguration<ShoppingCartItem>
    {
        public ShoppingCartItemMap()
        {
            this.ToTable("ShoppingCartItem");
            this.HasKey(sci => sci.Id);

            this.Property(sci => sci.CustomerEnteredPrice).HasPrecision(18, 4);

            this.Ignore(sci => sci.ShoppingCartType);
            this.Ignore(sci => sci.IsTaxExempt);

            this.HasRequired(sci => sci.Customer)
                .WithMany(c => c.ShoppingCartItems)
                .HasForeignKey(sci => sci.CustomerId);

            this.HasRequired(sci => sci.Article)
                .WithMany()
                .HasForeignKey(sci => sci.ArticleId);
        }
    }
}
