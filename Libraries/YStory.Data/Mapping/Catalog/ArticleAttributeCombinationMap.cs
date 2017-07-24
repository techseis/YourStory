using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class ArticleAttributeCombinationMap : YStoryEntityTypeConfiguration<ArticleAttributeCombination>
    {
        public ArticleAttributeCombinationMap()
        {
            this.ToTable("ArticleAttributeCombination");
            this.HasKey(pac => pac.Id);

            this.Property(pac => pac.Sku).HasMaxLength(400);
            this.Property(pac => pac.PublisherPartNumber).HasMaxLength(400);
            this.Property(pac => pac.Gtin).HasMaxLength(400);
            this.Property(pac => pac.OverriddenPrice).HasPrecision(18, 4);

            this.HasRequired(pac => pac.Article)
                .WithMany(p => p.ArticleAttributeCombinations)
                .HasForeignKey(pac => pac.ArticleId);
        }
    }
}