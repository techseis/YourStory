using YStory.Core.Domain.Messages;

namespace YStory.Data.Mapping.Messages
{
    public partial class NewsLetterSubscriptionMap : YStoryEntityTypeConfiguration<NewsLetterSubscription>
    {
        public NewsLetterSubscriptionMap()
        {
            this.ToTable("NewsLetterSubscription");
            this.HasKey(nls => nls.Id);

            this.Property(nls => nls.Email).IsRequired().HasMaxLength(255);
        }
    }
}