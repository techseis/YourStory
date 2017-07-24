using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class ArticlePictureMap : YStoryEntityTypeConfiguration<ArticlePicture>
    {
        public ArticlePictureMap()
        {
            this.ToTable("Article_Picture_Mapping");
            this.HasKey(pp => pp.Id);
            
            this.HasRequired(pp => pp.Picture)
                .WithMany()
                .HasForeignKey(pp => pp.PictureId);


            this.HasRequired(pp => pp.Article)
                .WithMany(p => p.ArticlePictures)
                .HasForeignKey(pp => pp.ArticleId);
        }
    }
}