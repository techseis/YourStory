namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents the article sorting
    /// </summary>
    public enum ArticleSortingEnum
    {
        /// <summary>
        /// Position (display subscription)
        /// </summary>
        Position = 0,
        /// <summary>
        /// Name: A to Z
        /// </summary>
        NameAsc = 5,
        /// <summary>
        /// Name: Z to A
        /// </summary>
        NameDesc = 6,
        /// <summary>
        /// Price: Low to High
        /// </summary>
        PriceAsc = 10,
        /// <summary>
        /// Price: High to Low
        /// </summary>
        PriceDesc = 11,
        /// <summary>
        /// Article creation date
        /// </summary>
        CreatedOn = 15,
    }
}