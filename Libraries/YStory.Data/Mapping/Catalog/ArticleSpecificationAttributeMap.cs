using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class ArticleSpecificationAttributeMap : YStoryEntityTypeConfiguration<ArticleSpecificationAttribute>
    {
        public ArticleSpecificationAttributeMap()
        {
            this.ToTable("Article_SpecificationAttribute_Mapping");
            this.HasKey(psa => psa.Id);

            this.Property(psa => psa.CustomValue).HasMaxLength(4000);

            this.Ignore(psa => psa.AttributeType);

            this.HasRequired(psa => psa.SpecificationAttributeOption)
                .WithMany()
                .HasForeignKey(psa => psa.SpecificationAttributeOptionId);


            this.HasRequired(psa => psa.Article)
                .WithMany(p => p.ArticleSpecificationAttributes)
                .HasForeignKey(psa => psa.ArticleId);
        }
    }
}