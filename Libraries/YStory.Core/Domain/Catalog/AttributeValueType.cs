namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents an attribute value type
    /// </summary>
    public enum AttributeValueType
    {
        /// <summary>
        /// Simple attribute value
        /// </summary>
        Simple = 0,
        /// <summary>
        /// Associated to a article (used when configuring bundled articles)
        /// </summary>
        AssociatedToArticle = 10,
    }
}
