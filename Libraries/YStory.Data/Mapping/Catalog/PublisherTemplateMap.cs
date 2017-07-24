using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class PublisherTemplateMap : YStoryEntityTypeConfiguration<PublisherTemplate>
    {
        public PublisherTemplateMap()
        {
            this.ToTable("PublisherTemplate");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(p => p.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}