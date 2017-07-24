using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class ArticlePublisherMap : YStoryEntityTypeConfiguration<ArticlePublisher>
    {
        public ArticlePublisherMap()
        {
            this.ToTable("Article_Publisher_Mapping");
            this.HasKey(pm => pm.Id);
            
            this.HasRequired(pm => pm.Publisher)
                .WithMany()
                .HasForeignKey(pm => pm.PublisherId);


            this.HasRequired(pm => pm.Article)
                .WithMany(p => p.ArticlePublishers)
                .HasForeignKey(pm => pm.ArticleId);
        }
    }
}