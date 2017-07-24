using YStory.Core.Domain.Tax;

namespace YStory.Data.Mapping.Tax
{
    public class TaxCategoryMap : YStoryEntityTypeConfiguration<TaxCategory>
    {
        public TaxCategoryMap()
        {
            this.ToTable("TaxCategory");
            this.HasKey(tc => tc.Id);
            this.Property(tc => tc.Name).IsRequired().HasMaxLength(400);
        }
    }
}
