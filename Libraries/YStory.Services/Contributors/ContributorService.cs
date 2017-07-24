using System;
using System.Linq;
using YStory.Core;
using YStory.Core.Data;
using YStory.Core.Domain.Contributors;
using YStory.Services.Events;

namespace YStory.Services.Contributors
{
    /// <summary>
    /// Contributor service
    /// </summary>
    public partial class ContributorService : IContributorService
    {
        #region Fields

        private readonly IRepository<Contributor> _contributorRepository;
        private readonly IRepository<ContributorNote> _contributorNoteRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="contributorRepository">Contributor repository</param>
        /// <param name="contributorNoteRepository">Contributor note repository</param>
        /// <param name="eventPublisher">Event published</param>
        public ContributorService(IRepository<Contributor> contributorRepository,
            IRepository<ContributorNote> contributorNoteRepository,
            IEventPublisher eventPublisher)
        {
            this._contributorRepository = contributorRepository;
            this._contributorNoteRepository = contributorNoteRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets a contributor by contributor identifier
        /// </summary>
        /// <param name="contributorId">Contributor identifier</param>
        /// <returns>Contributor</returns>
        public virtual Contributor GetContributorById(int contributorId)
        {
            if (contributorId == 0)
                return null;

            return _contributorRepository.GetById(contributorId);
        }

        /// <summary>
        /// Delete a contributor
        /// </summary>
        /// <param name="contributor">Contributor</param>
        public virtual void DeleteContributor(Contributor contributor)
        {
            if (contributor == null)
                throw new ArgumentNullException("contributor");

            contributor.Deleted = true;
            UpdateContributor(contributor);

            //event notification
            _eventPublisher.EntityDeleted(contributor);
        }

        /// <summary>
        /// Gets all contributors
        /// </summary>
        /// <param name="name">Contributor name</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Contributors</returns>
        public virtual IPagedList<Contributor> GetAllContributors(string name = "",
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _contributorRepository.Table;
            if (!String.IsNullOrWhiteSpace(name))
                query = query.Where(v => v.Name.Contains(name));
            if (!showHidden)
                query = query.Where(v => v.Active);
            query = query.Where(v => !v.Deleted);
            query = query.OrderBy(v => v.DisplaySubscription).ThenBy(v => v.Name);

            var contributors = new PagedList<Contributor>(query, pageIndex, pageSize);
            return contributors;
        }

        /// <summary>
        /// Inserts a contributor
        /// </summary>
        /// <param name="contributor">Contributor</param>
        public virtual void InsertContributor(Contributor contributor)
        {
            if (contributor == null)
                throw new ArgumentNullException("contributor");

            _contributorRepository.Insert(contributor);

            //event notification
            _eventPublisher.EntityInserted(contributor);
        }

        /// <summary>
        /// Updates the contributor
        /// </summary>
        /// <param name="contributor">Contributor</param>
        public virtual void UpdateContributor(Contributor contributor)
        {
            if (contributor == null)
                throw new ArgumentNullException("contributor");

            _contributorRepository.Update(contributor);

            //event notification
            _eventPublisher.EntityUpdated(contributor);
        }



        /// <summary>
        /// Gets a contributor note note
        /// </summary>
        /// <param name="contributorNoteId">The contributor note identifier</param>
        /// <returns>Contributor note</returns>
        public virtual ContributorNote GetContributorNoteById(int contributorNoteId)
        {
            if (contributorNoteId == 0)
                return null;

            return _contributorNoteRepository.GetById(contributorNoteId);
        }

        /// <summary>
        /// Deletes a contributor note
        /// </summary>
        /// <param name="contributorNote">The contributor note</param>
        public virtual void DeleteContributorNote(ContributorNote contributorNote)
        {
            if (contributorNote == null)
                throw new ArgumentNullException("contributorNote");

            _contributorNoteRepository.Delete(contributorNote);

            //event notification
            _eventPublisher.EntityDeleted(contributorNote);
        }

        #endregion
    }
}