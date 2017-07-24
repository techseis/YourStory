using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class ArticleReviewMap : YStoryEntityTypeConfiguration<ArticleReview>
    {
        public ArticleReviewMap()
        {
            this.ToTable("ArticleReview");
            this.HasKey(pr => pr.Id);

            this.HasRequired(pr => pr.Article)
                .WithMany(p => p.ArticleReviews)
                .HasForeignKey(pr => pr.ArticleId);

            this.HasRequired(pr => pr.Customer)
                .WithMany()
                .HasForeignKey(pr => pr.CustomerId);

            this.HasRequired(pr => pr.Store)
                .WithMany()
                .HasForeignKey(pr => pr.StoreId);
        }
    }
}