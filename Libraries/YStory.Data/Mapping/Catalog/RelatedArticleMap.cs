using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class RelatedArticleMap : YStoryEntityTypeConfiguration<RelatedArticle>
    {
        public RelatedArticleMap()
        {
            this.ToTable("RelatedArticle");
            this.HasKey(c => c.Id);
        }
    }
}