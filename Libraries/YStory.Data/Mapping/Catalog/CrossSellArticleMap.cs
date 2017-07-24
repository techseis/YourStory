using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class CrossSellArticleMap : YStoryEntityTypeConfiguration<CrossSellArticle>
    {
        public CrossSellArticleMap()
        {
            this.ToTable("CrossSellArticle");
            this.HasKey(c => c.Id);
        }
    }
}