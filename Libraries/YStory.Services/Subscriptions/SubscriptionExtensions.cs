using System;
using System.Linq;
using YStory.Core.Domain.Subscriptions;
using YStory.Core.Html;

namespace YStory.Services.Subscriptions
{
    public static class SubscriptionExtensions
    {
        /// <summary>
        /// Formats the subscription note text
        /// </summary>
        /// <param name="subscriptionNote">Subscription note</param>
        /// <returns>Formatted text</returns>
        public static string FormatSubscriptionNoteText(this SubscriptionNote subscriptionNote)
        {
            if (subscriptionNote == null)
                throw new ArgumentNullException("subscriptionNote");

            string text = subscriptionNote.Note;

            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text, false, true, false, false, false, false);

            return text;
        }

         
    }
}
