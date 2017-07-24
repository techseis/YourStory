using YStory.Core.Domain.Forums;

namespace YStory.Data.Mapping.Forums
{
    public partial class ForumGroupMap : YStoryEntityTypeConfiguration<ForumGroup>
    {
        public ForumGroupMap()
        {
            this.ToTable("Forums_Group");
            this.HasKey(fg => fg.Id);
            this.Property(fg => fg.Name).IsRequired().HasMaxLength(200);
        }
    }
}
