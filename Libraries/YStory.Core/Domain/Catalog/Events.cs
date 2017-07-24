namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Article review approved event
    /// </summary>
    public class ArticleReviewApprovedEvent
    {
        public ArticleReviewApprovedEvent(ArticleReview articleReview)
        {
            this.ArticleReview = articleReview;
        }

        /// <summary>
        /// Article review
        /// </summary>
        public ArticleReview ArticleReview { get; private set; }
    }
}