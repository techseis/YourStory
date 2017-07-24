using YStory.Core.Domain.Blogs;

namespace YStory.Data.Mapping.Blogs
{
    public partial class BlogCommentMap : YStoryEntityTypeConfiguration<BlogComment>
    {
        public BlogCommentMap()
        {
            this.ToTable("BlogComment");
            this.HasKey(comment => comment.Id);

            this.HasRequired(comment => comment.BlogPost)
                .WithMany(blog => blog.BlogComments)
                .HasForeignKey(comment => comment.BlogPostId);

            this.HasRequired(comment => comment.Customer)
                .WithMany()
                .HasForeignKey(comment => comment.CustomerId);

            this.HasRequired(comment => comment.Store)
                .WithMany()
                .HasForeignKey(comment => comment.StoreId);
        }
    }
}