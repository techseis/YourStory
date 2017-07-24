using YStory.Core.Domain.Contributors;

namespace YStory.Data.Mapping.Contributors
{
    public partial class ContributorNoteMap : YStoryEntityTypeConfiguration<ContributorNote>
    {
        public ContributorNoteMap()
        {
            this.ToTable("ContributorNote");
            this.HasKey(vn => vn.Id);
            this.Property(vn => vn.Note).IsRequired();

            this.HasRequired(vn => vn.Contributor)
                .WithMany(v => v.ContributorNotes)
                .HasForeignKey(vn => vn.ContributorId);
        }
    }
}