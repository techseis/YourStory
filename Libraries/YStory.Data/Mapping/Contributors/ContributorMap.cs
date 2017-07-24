using YStory.Core.Domain.Contributors;

namespace YStory.Data.Mapping.Contributors
{
    public partial class ContributorMap : YStoryEntityTypeConfiguration<Contributor>
    {
        public ContributorMap()
        {
            this.ToTable("Contributor");
            this.HasKey(v => v.Id);

            this.Property(v => v.Name).IsRequired().HasMaxLength(400);
            this.Property(v => v.Email).HasMaxLength(400);
            this.Property(v => v.MetaKeywords).HasMaxLength(400);
            this.Property(v => v.MetaTitle).HasMaxLength(400);
            this.Property(v => v.PageSizeOptions).HasMaxLength(200);
        }
    }
}