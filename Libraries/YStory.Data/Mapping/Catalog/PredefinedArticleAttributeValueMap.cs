using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class PredefinedArticleAttributeValueMap : YStoryEntityTypeConfiguration<PredefinedArticleAttributeValue>
    {
        public PredefinedArticleAttributeValueMap()
        {
            this.ToTable("PredefinedArticleAttributeValue");
            this.HasKey(pav => pav.Id);
            this.Property(pav => pav.Name).IsRequired().HasMaxLength(400);

            this.Property(pav => pav.PriceAdjustment).HasPrecision(18, 4);
            this.Property(pav => pav.WeightAdjustment).HasPrecision(18, 4);
            this.Property(pav => pav.Cost).HasPrecision(18, 4);

            this.HasRequired(pav => pav.ArticleAttribute)
                .WithMany()
                .HasForeignKey(pav => pav.ArticleAttributeId);
        }
    }
}