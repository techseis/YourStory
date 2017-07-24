using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class ArticleReviewHelpfulnessMap : YStoryEntityTypeConfiguration<ArticleReviewHelpfulness>
    {
        public ArticleReviewHelpfulnessMap()
        {
            this.ToTable("ArticleReviewHelpfulness");
            this.HasKey(pr => pr.Id);

            this.HasRequired(prh => prh.ArticleReview)
                .WithMany(pr => pr.ArticleReviewHelpfulnessEntries)
                .HasForeignKey(prh => prh.ArticleReviewId).WillCascadeOnDelete(true);
        }
    }
}