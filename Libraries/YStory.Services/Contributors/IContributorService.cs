using YStory.Core;
using YStory.Core.Domain.Contributors;

namespace YStory.Services.Contributors
{
    /// <summary>
    /// Contributor service interface
    /// </summary>
    public partial interface IContributorService
    {
        /// <summary>
        /// Gets a contributor by contributor identifier
        /// </summary>
        /// <param name="contributorId">Contributor identifier</param>
        /// <returns>Contributor</returns>
        Contributor GetContributorById(int contributorId);

        /// <summary>
        /// Delete a contributor
        /// </summary>
        /// <param name="contributor">Contributor</param>
        void DeleteContributor(Contributor contributor);

        /// <summary>
        /// Gets all contributors
        /// </summary>
        /// <param name="name">Contributor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Contributors</returns>
        IPagedList<Contributor> GetAllContributors(string name = "", 
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Inserts a contributor
        /// </summary>
        /// <param name="contributor">Contributor</param>
        void InsertContributor(Contributor contributor);

        /// <summary>
        /// Updates the contributor
        /// </summary>
        /// <param name="contributor">Contributor</param>
        void UpdateContributor(Contributor contributor);



        /// <summary>
        /// Gets a contributor note note
        /// </summary>
        /// <param name="contributorNoteId">The contributor note identifier</param>
        /// <returns>Contributor note</returns>
        ContributorNote GetContributorNoteById(int contributorNoteId);

        /// <summary>
        /// Deletes a contributor note
        /// </summary>
        /// <param name="contributorNote">The contributor note</param>
        void DeleteContributorNote(ContributorNote contributorNote);
    }
}