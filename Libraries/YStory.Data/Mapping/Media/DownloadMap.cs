using YStory.Core.Domain.Media;

namespace YStory.Data.Mapping.Media
{
    public partial class DownloadMap : YStoryEntityTypeConfiguration<Download>
    {
        public DownloadMap()
        {
            this.ToTable("Download");
            this.HasKey(p => p.Id);
            this.Property(p => p.DownloadBinary).IsMaxLength();
        }
    }
}