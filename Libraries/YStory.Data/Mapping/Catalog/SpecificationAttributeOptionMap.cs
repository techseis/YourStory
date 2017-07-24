using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class SpecificationAttributeOptionMap : YStoryEntityTypeConfiguration<SpecificationAttributeOption>
    {
        public SpecificationAttributeOptionMap()
        {
            this.ToTable("SpecificationAttributeOption");
            this.HasKey(sao => sao.Id);
            this.Property(sao => sao.Name).IsRequired();
            this.Property(sao => sao.ColorSquaresRgb).HasMaxLength(100);

            this.HasRequired(sao => sao.SpecificationAttribute)
                .WithMany(sa => sa.SpecificationAttributeOptions)
                .HasForeignKey(sao => sao.SpecificationAttributeId);
        }
    }
}