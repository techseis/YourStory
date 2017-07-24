using YStory.Core.Domain.Common;

namespace YStory.Data.Mapping.Common
{
    public partial class SearchTermMap : YStoryEntityTypeConfiguration<SearchTerm>
    {
        public SearchTermMap()
        {
            this.ToTable("SearchTerm");
            this.HasKey(st => st.Id);
        }
    }
}
