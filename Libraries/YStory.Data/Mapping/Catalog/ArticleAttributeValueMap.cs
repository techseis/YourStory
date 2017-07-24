using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class ArticleAttributeValueMap : YStoryEntityTypeConfiguration<ArticleAttributeValue>
    {
        public ArticleAttributeValueMap()
        {
            this.ToTable("ArticleAttributeValue");
            this.HasKey(pav => pav.Id);
            this.Property(pav => pav.Name).IsRequired().HasMaxLength(400);
            this.Property(pav => pav.ColorSquaresRgb).HasMaxLength(100);

            this.Property(pav => pav.PriceAdjustment).HasPrecision(18, 4);
            this.Property(pav => pav.WeightAdjustment).HasPrecision(18, 4);
            this.Property(pav => pav.Cost).HasPrecision(18, 4);

            this.Ignore(pav => pav.AttributeValueType);

            this.HasRequired(pav => pav.ArticleAttributeMapping)
                .WithMany(pam => pam.ArticleAttributeValues)
                .HasForeignKey(pav => pav.ArticleAttributeMappingId);
        }
    }
}