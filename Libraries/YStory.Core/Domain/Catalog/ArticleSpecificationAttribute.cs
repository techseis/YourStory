namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article specification attribute
    /// </summary>
    public partial class ArticleSpecificationAttribute : BaseEntity
    {
        /// <summary>
        /// Gets or sets the article identifier
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the attribute type ID
        /// </summary>
        public int AttributeTypeId { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
        public int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        /// Gets or sets the custom value
        /// </summary>
        public string CustomValue { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute can be filtered by
        /// </summary>
        public bool AllowFiltering { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute will be shown on the article page
        /// </summary>
        public bool ShowOnArticlePage { get; set; }

        /// <summary>
        /// Gets or sets the display subscription
        /// </summary>
        public int DisplaySubscription { get; set; }
        
        /// <summary>
        /// Gets or sets the article
        /// </summary>
        public virtual Article Article { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute option
        /// </summary>
        public virtual SpecificationAttributeOption SpecificationAttributeOption { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public SpecificationAttributeType AttributeType
        {
            get
            {
                return (SpecificationAttributeType)this.AttributeTypeId;
            }
            set
            {
                this.AttributeTypeId = (int)value;
            }
        }
    }
}
