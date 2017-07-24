using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class ArticleAttributeMappingMap : YStoryEntityTypeConfiguration<ArticleAttributeMapping>
    {
        public ArticleAttributeMappingMap()
        {
            this.ToTable("Article_ArticleAttribute_Mapping");
            this.HasKey(pam => pam.Id);
            this.Ignore(pam => pam.AttributeControlType);

            this.HasRequired(pam => pam.Article)
                .WithMany(p => p.ArticleAttributeMappings)
                .HasForeignKey(pam => pam.ArticleId);

            this.HasRequired(pam => pam.ArticleAttribute)
                .WithMany()
                .HasForeignKey(pam => pam.ArticleAttributeId);
        }
    }
}