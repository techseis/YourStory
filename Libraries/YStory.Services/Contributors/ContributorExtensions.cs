using System;
using YStory.Core.Domain.Contributors;
using YStory.Core.Html;

namespace YStory.Services.Contributors
{
    public static class ContributorExtensions
    {
        /// <summary>
        /// Formats the contributor note text
        /// </summary>
        /// <param name="contributorNote">Contributor note</param>
        /// <returns>Formatted text</returns>
        public static string FormatContributorNoteText(this ContributorNote contributorNote)
        {
            if (contributorNote == null)
                throw new ArgumentNullException("contributorNote");

            string text = contributorNote.Note;

            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);

            return text;
        }
    }
}
