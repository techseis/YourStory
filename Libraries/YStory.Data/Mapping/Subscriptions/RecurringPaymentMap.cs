using YStory.Core.Domain.Subscriptions;

namespace YStory.Data.Mapping.Subscriptions
{
    public partial class RecurringPaymentMap : YStoryEntityTypeConfiguration<RecurringPayment>
    {
        public RecurringPaymentMap()
        {
            this.ToTable("RecurringPayment");
            this.HasKey(rp => rp.Id);

            this.Ignore(rp => rp.NextPaymentDate);
            this.Ignore(rp => rp.CyclesRemaining);
            this.Ignore(rp => rp.CyclePeriod);



            //this.HasRequired(rp => rp.InitialSubscription).WithOptional().Map(x => x.MapKey("InitialSubscriptionId")).WillCascadeOnDelete(false);
            this.HasRequired(rp => rp.InitialSubscription)
                .WithMany()
                .HasForeignKey(o => o.InitialSubscriptionId)
                .WillCascadeOnDelete(false);
        }
    }
}