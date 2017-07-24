using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class ArticleTagMap : YStoryEntityTypeConfiguration<ArticleTag>
    {
        public ArticleTagMap()
        {
            this.ToTable("ArticleTag");
            this.HasKey(pt => pt.Id);
            this.Property(pt => pt.Name).IsRequired().HasMaxLength(400);
        }
    }
}