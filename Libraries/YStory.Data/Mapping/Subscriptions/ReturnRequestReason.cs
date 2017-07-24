using YStory.Core.Domain.Subscriptions;

namespace YStory.Data.Mapping.Subscriptions
{
    public partial class ReturnRequestReasonMap : YStoryEntityTypeConfiguration<ReturnRequestReason>
    {
        public ReturnRequestReasonMap()
        {
            this.ToTable("ReturnRequestReason");
            this.HasKey(rrr => rrr.Id);
            this.Property(rrr => rrr.Name).IsRequired().HasMaxLength(400);
        }
    }
}