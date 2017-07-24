using YStory.Core.Domain.Subscriptions;

namespace YStory.Data.Mapping.Subscriptions
{
    public partial class RecurringPaymentHistoryMap : YStoryEntityTypeConfiguration<RecurringPaymentHistory>
    {
        public RecurringPaymentHistoryMap()
        {
            this.ToTable("RecurringPaymentHistory");
            this.HasKey(rph => rph.Id);

            this.HasRequired(rph => rph.RecurringPayment)
                .WithMany(rp => rp.RecurringPaymentHistory)
                .HasForeignKey(rph => rph.RecurringPaymentId);

            //entity framework issue if we have navigation property to 'Subscription'
            //1. save recurring payment with an subscription
            //2. save recurring payment history with an subscription
            //3. update associated subscription => exception is thrown
        }
    }
}