using YStory.Core.Domain.Catalog;

namespace YStory.Data.Mapping.Catalog
{
    public partial class SpecificationAttributeMap : YStoryEntityTypeConfiguration<SpecificationAttribute>
    {
        public SpecificationAttributeMap()
        {
            this.ToTable("SpecificationAttribute");
            this.HasKey(sa => sa.Id);
            this.Property(sa => sa.Name).IsRequired();
        }
    }
}