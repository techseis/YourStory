using YStory.Core.Domain.Subscriptions;

namespace YStory.Data.Mapping.Subscriptions
{
    public partial class SubscriptionNoteMap : YStoryEntityTypeConfiguration<SubscriptionNote>
    {
        public SubscriptionNoteMap()
        {
            this.ToTable("SubscriptionNote");
            this.HasKey(on => on.Id);
            this.Property(on => on.Note).IsRequired();

            this.HasRequired(on => on.Subscription)
                .WithMany(o => o.SubscriptionNotes)
                .HasForeignKey(on => on.SubscriptionId);
        }
    }
}