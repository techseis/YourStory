using YStory.Core.Domain.Affiliates;

namespace YStory.Data.Mapping.Affiliates
{
    public partial class AffiliateMap : YStoryEntityTypeConfiguration<Affiliate>
    {
        public AffiliateMap()
        {
            this.ToTable("Affiliate");
            this.HasKey(a => a.Id);

            this.HasRequired(a => a.Address).WithMany().HasForeignKey(x => x.AddressId).WillCascadeOnDelete(false);
        }
    }
}