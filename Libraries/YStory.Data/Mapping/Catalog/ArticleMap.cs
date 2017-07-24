using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class ArticleMap : YStoryEntityTypeConfiguration<Article>
    {
        public ArticleMap()
        {
            this.ToTable("Article");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(p => p.MetaKeywords).HasMaxLength(400);
            this.Property(p => p.MetaTitle).HasMaxLength(400);
            this.Property(p => p.Sku).HasMaxLength(400);
            this.Property(p => p.PublisherPartNumber).HasMaxLength(400);
            this.Property(p => p.Gtin).HasMaxLength(400);
            this.Property(p => p.Price).HasPrecision(18, 4);
            this.Property(p => p.OldPrice).HasPrecision(18, 4);
            this.Property(p => p.ArticleCost).HasPrecision(18, 4);
            this.Property(p => p.RequiredArticleIds).HasMaxLength(1000);

            this.Ignore(p => p.ArticleType);
            this.Ignore(p => p.RecurringCyclePeriod);
            this.Ignore(p => p.RentalPricePeriod);

            this.HasMany(p => p.ArticleTags)
                .WithMany(pt => pt.Articles)
                .Map(m => m.ToTable("Article_ArticleTag_Mapping"));
        }
    }
}