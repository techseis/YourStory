using YStory.Core.Domain.Catalog;

namespace YStory.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class ArticleAttributeExtensions
    {
        /// <summary>
        /// A value indicating whether this article attribute should have values
        /// </summary>
        /// <param name="articleAttributeMapping">Article attribute mapping</param>
        /// <returns>Result</returns>
        public static bool ShouldHaveValues(this ArticleAttributeMapping articleAttributeMapping)
        {
            if (articleAttributeMapping == null)
                return false;

            if (articleAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                articleAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                articleAttributeMapping.AttributeControlType == AttributeControlType.Datepicker ||
                articleAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute controle types support values
            return true;
        }

        /// <summary>
        /// A value indicating whether this article attribute can be used as condition for some other attribute
        /// </summary>
        /// <param name="articleAttributeMapping">Article attribute mapping</param>
        /// <returns>Result</returns>
        public static bool CanBeUsedAsCondition(this ArticleAttributeMapping articleAttributeMapping)
        {
            if (articleAttributeMapping == null)
                return false;

            if (articleAttributeMapping.AttributeControlType == AttributeControlType.ReadonlyCheckboxes || 
                articleAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                articleAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                articleAttributeMapping.AttributeControlType == AttributeControlType.Datepicker ||
                articleAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return false;

            //other attribute controle types support it
            return true;
        }

        /// <summary>
        /// A value indicating whether this article attribute should can have some validation rules
        /// </summary>
        /// <param name="articleAttributeMapping">Article attribute mapping</param>
        /// <returns>Result</returns>
        public static bool ValidationRulesAllowed(this ArticleAttributeMapping articleAttributeMapping)
        {
            if (articleAttributeMapping == null)
                return false;

            if (articleAttributeMapping.AttributeControlType == AttributeControlType.TextBox ||
                articleAttributeMapping.AttributeControlType == AttributeControlType.MultilineTextbox ||
                articleAttributeMapping.AttributeControlType == AttributeControlType.FileUpload)
                return true;

            //other attribute controle types does not have validation
            return false;
        }

        /// <summary>
        /// A value indicating whether this article attribute is non-combinable
        /// </summary>
        /// <param name="articleAttributeMapping">Article attribute mapping</param>
        /// <returns>Result</returns>
        public static bool IsNonCombinable(this ArticleAttributeMapping articleAttributeMapping)
        {
            //When you have a article with several attributes it may well be that some are combinable,
            //whose combination may form a new SKU with its own inventory,
            //and some non-combinable are more used to add accesories

            if (articleAttributeMapping == null)
                return false;

            //we can add a new property to "ArticleAttributeMapping" entity indicating whether it's combinable/non-combinable
            //but we assume that attributes
            //which cannot have values (any value can be entered by a customer)
            //are non-combinable
            var result = !ShouldHaveValues(articleAttributeMapping);
            return result;
        }
    }
}
