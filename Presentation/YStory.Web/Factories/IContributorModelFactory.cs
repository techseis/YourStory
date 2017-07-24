using YStory.Web.Models.Contributors;

namespace YStory.Web.Factories
{
    /// <summary>
    /// Represents the interface of the contributor model factory
    /// </summary>
    public partial interface IContributorModelFactory
    {
        /// <summary>
        /// Prepare the apply contributor model
        /// </summary>
        /// <param name="model">The apply contributor model</param>
        /// <param name="validateContributor">Whether to validate that the customer is already a contributor</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>The apply contributor model</returns>
        ApplyContributorModel PrepareApplyContributorModel(ApplyContributorModel model, bool validateContributor,bool excludeProperties);

        /// <summary>
        /// Prepare the contributor info model
        /// </summary>
        /// <param name="model">Contributor info model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>Contributor info model</returns>
        ContributorInfoModel PrepareContributorInfoModel(ContributorInfoModel model, bool excludeProperties);
    }
}
