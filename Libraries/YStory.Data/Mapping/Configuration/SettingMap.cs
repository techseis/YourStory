using YStory.Core.Domain.Configuration;

namespace YStory.Data.Mapping.Configuration
{
    public partial class SettingMap : YStoryEntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            this.ToTable("Setting");
            this.HasKey(s => s.Id);
            this.Property(s => s.Name).IsRequired().HasMaxLength(200);
            this.Property(s => s.Value).IsRequired().HasMaxLength(2000);
        }
    }
}