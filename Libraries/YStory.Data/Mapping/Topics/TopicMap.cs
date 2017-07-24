using YStory.Core.Domain.Topics;

namespace YStory.Data.Mapping.Topics
{
    public class TopicMap : YStoryEntityTypeConfiguration<Topic>
    {
        public TopicMap()
        {
            this.ToTable("Topic");
            this.HasKey(t => t.Id);
        }
    }
}
