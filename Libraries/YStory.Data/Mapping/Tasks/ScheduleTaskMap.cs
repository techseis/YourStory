using YStory.Core.Domain.Tasks;

namespace YStory.Data.Mapping.Tasks
{
    public partial class ScheduleTaskMap : YStoryEntityTypeConfiguration<ScheduleTask>
    {
        public ScheduleTaskMap()
        {
            this.ToTable("ScheduleTask");
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).IsRequired();
            this.Property(t => t.Type).IsRequired();
        }
    }
}