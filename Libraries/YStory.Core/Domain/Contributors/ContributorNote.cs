using System;

namespace YStory.Core.Domain.Contributors
{
    /// <summary>
    /// Represents a contributor note
    /// </summary>
    public partial class ContributorNote : BaseEntity
    {
        /// <summary>
        /// Gets or sets the contributor identifier
        /// </summary>
        public int ContributorId { get; set; }

        /// <summary>
        /// Gets or sets the note
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the date and time of contributor note creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the contributor
        /// </summary>
        public virtual Contributor Contributor { get; set; }
    }

}
