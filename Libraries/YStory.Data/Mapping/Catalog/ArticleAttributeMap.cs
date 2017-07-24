using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class ArticleAttributeMap : YStoryEntityTypeConfiguration<ArticleAttribute>
    {
        public ArticleAttributeMap()
        {
            this.ToTable("ArticleAttribute");
            this.HasKey(pa => pa.Id);
            this.Property(pa => pa.Name).IsRequired();
        }
    }
}