using YStory.Core.Domain.Subscriptions;

namespace YStory.Data.Mapping.Subscriptions
{
    public partial class SubscriptionMap : YStoryEntityTypeConfiguration<Subscription>
    {
        public SubscriptionMap()
        {
            this.ToTable("Subscription");
            this.HasKey(o => o.Id);
            this.Property(o => o.CurrencyRate).HasPrecision(18, 8);
            this.Property(o => o.SubscriptionSubtotalInclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionSubtotalExclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionSubTotalDiscountInclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionSubTotalDiscountExclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionShippingInclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionShippingExclTax).HasPrecision(18, 4);
            this.Property(o => o.PaymentMethodAdditionalFeeInclTax).HasPrecision(18, 4);
            this.Property(o => o.PaymentMethodAdditionalFeeExclTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionTax).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionDiscount).HasPrecision(18, 4);
            this.Property(o => o.SubscriptionTotal).HasPrecision(18, 4);
            this.Property(o => o.RefundedAmount).HasPrecision(18, 4);
            this.Property(o => o.CustomSubscriptionNumber).IsRequired();

            this.Ignore(o => o.SubscriptionStatus);
            this.Ignore(o => o.PaymentStatus);
            this.Ignore(o => o.CustomerTaxDisplayType);
            this.Ignore(o => o.TaxRatesDictionary);
            
            this.HasRequired(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId);
            
            //code below is commented because it causes some issues on big databases - http://www.yourstory.com/boards/t/11126/bug-version-20-command-confirm-takes-several-minutes-using-big-databases.aspx
            //this.HasRequired(o => o.BillingAddress).WithOptional().Map(x => x.MapKey("BillingAddressId")).WillCascadeOnDelete(false);
            //this.HasOptional(o => o.ShippingAddress).WithOptionalDependent().Map(x => x.MapKey("ShippingAddressId")).WillCascadeOnDelete(false);
            this.HasRequired(o => o.BillingAddress)
                .WithMany()
                .HasForeignKey(o => o.BillingAddressId)
                .WillCascadeOnDelete(false);
            this.HasOptional(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId)
                .WillCascadeOnDelete(false);
            this.HasOptional(o => o.PickupAddress)
                .WithMany()
                .HasForeignKey(o => o.PickupAddressId)
                .WillCascadeOnDelete(false);
        }
    }
}