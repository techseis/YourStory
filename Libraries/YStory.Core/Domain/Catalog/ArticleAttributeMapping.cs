using System.Collections.Generic;
using YStory.Core.Domain.Localization;

namespace YStory.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a article attribute mapping
    /// </summary>
    public partial class ArticleAttributeMapping : BaseEntity, ILocalizedEntity
    {
        private ICollection<ArticleAttributeValue> _articleAttributeValues;

        /// <summary>
        /// Gets or sets the article identifier
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Gets or sets the article attribute identifier
        /// </summary>
        public int ArticleAttributeId { get; set; }

        /// <summary>
        /// Gets or sets a value a text prompt
        /// </summary>
        public string TextPrompt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the display subscription
        /// </summary>
        public int DisplaySubscription { get; set; }

        //validation fields

        /// <summary>
        /// Gets or sets the validation rule for minimum length (for textbox and multiline textbox)
        /// </summary>
        public int? ValidationMinLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for maximum length (for textbox and multiline textbox)
        /// </summary>
        public int? ValidationMaxLength { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file allowed extensions (for file upload)
        /// </summary>
        public string ValidationFileAllowedExtensions { get; set; }

        /// <summary>
        /// Gets or sets the validation rule for file maximum size in kilobytes (for file upload)
        /// </summary>
        public int? ValidationFileMaximumSize { get; set; }

        /// <summary>
        /// Gets or sets the default value (for textbox and multiline textbox)
        /// </summary>
        public string DefaultValue { get; set; }



        /// <summary>
        /// Gets or sets a condition (depending on other attribute) when this attribute should be enabled (visible).
        /// Leave empty (or null) to enable this attribute.
        /// Conditional attributes that only appear if a previous attribute is selected, such as having an option 
        /// for personalizing clothing with a name and only providing the text input box if the "Personalize" radio button is checked.
        /// </summary>
        public string ConditionAttributeXml { get; set; }



        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public AttributeControlType AttributeControlType
        {
            get
            {
                return (AttributeControlType)this.AttributeControlTypeId;
            }
            set
            {
                this.AttributeControlTypeId = (int)value; 
            }
        }

        /// <summary>
        /// Gets the article attribute
        /// </summary>
        public virtual ArticleAttribute ArticleAttribute { get; set; }

        /// <summary>
        /// Gets the article
        /// </summary>
        public virtual Article Article { get; set; }
        
        /// <summary>
        /// Gets the article attribute values
        /// </summary>
        public virtual ICollection<ArticleAttributeValue> ArticleAttributeValues
        {
            get { return _articleAttributeValues ?? (_articleAttributeValues = new List<ArticleAttributeValue>()); }
            protected set { _articleAttributeValues = value; }
        }

    }

}
