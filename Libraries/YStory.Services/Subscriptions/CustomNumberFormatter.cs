//using System;
//using System.Text.RegularExpressions;

using YStory.Core.Domain.Subscriptions;

namespace YStory.Services.Subscriptions
{
    public partial class CustomNumberFormatter : ICustomNumberFormatter
    {
        #region Fields

        private SubscriptionSettings _subscriptionSettings;

        #endregion

        #region Ctor

        public CustomNumberFormatter(SubscriptionSettings subscriptionSettings)
        {
            _subscriptionSettings = subscriptionSettings;
        }

        #endregion

        #region Methods

        public virtual string GenerateReturnRequestCustomNumber(ReturnRequest returnRequest)
        {
            var customNumber = string.Empty;

            if (string.IsNullOrEmpty(_subscriptionSettings.ReturnRequestNumberMask))
            {
                customNumber = returnRequest.Id.ToString();
            }
            else
            {
                customNumber = _subscriptionSettings.ReturnRequestNumberMask
                    .Replace("{ID}", returnRequest.Id.ToString())
                    .Replace("{YYYY}", returnRequest.CreatedOnUtc.ToString("yyyy"))
                    .Replace("{YY}", returnRequest.CreatedOnUtc.ToString("yy"))
                    .Replace("{MM}", returnRequest.CreatedOnUtc.ToString("MM"))
                    .Replace("{DD}", returnRequest.CreatedOnUtc.ToString("dd"));

                ////if you need to use the format for the ID with leading zeros, use the following code instead of the previous one.
                ////mask for Id example {#:00000000}
                //var rgx = new Regex(@"{#:\d+}");
                //var match = rgx.Match(customNumber);
                //var maskForReplase = match.Value;
                //
                //rgx = new Regex(@"\d+");
                //match = rgx.Match(maskForReplase);
                //
                //var formatValue = match.Value;
                //if(!string.IsNullOrEmpty(formatValue) && !string.IsNullOrEmpty(maskForReplase))
                //    customNumber = customNumber.Replace(maskForReplase, returnRequest.Id.ToString(formatValue));
                //else
                //    customNumber = customNumber.Insert(0, string.Format("{0}-", returnRequest.Id));
            }

            return customNumber;
        }

        public virtual string GenerateSubscriptionCustomNumber(Subscription subscription)
        {
            if (string.IsNullOrEmpty(_subscriptionSettings.CustomSubscriptionNumberMask))
                return subscription.Id.ToString();

            var customNumber = _subscriptionSettings.CustomSubscriptionNumberMask
                .Replace("{ID}", subscription.Id.ToString())
                .Replace("{YYYY}", subscription.CreatedOnUtc.ToString("yyyy"))
                .Replace("{YY}", subscription.CreatedOnUtc.ToString("yy"))
                .Replace("{MM}", subscription.CreatedOnUtc.ToString("MM"))
                .Replace("{DD}", subscription.CreatedOnUtc.ToString("dd")).Trim();

            ////if you need to use the format for the ID with leading zeros, use the following code instead of the previous one.
            ////mask for Id example {#:00000000}
            //var rgx = new Regex(@"{#:\d+}");
            //var match = rgx.Match(customNumber);
            //var maskForReplase = match.Value;

            //rgx = new Regex(@"\d+");
            //match = rgx.Match(maskForReplase);

            //var formatValue = match.Value;
            //if (!string.IsNullOrEmpty(formatValue) && !string.IsNullOrEmpty(maskForReplase))
            //    customNumber = customNumber.Replace(maskForReplase, subscription.Id.ToString(formatValue));
            //else
            //    customNumber = customNumber.Insert(0, string.Format("{0}-", subscription.Id));


            return customNumber;
        }

        #endregion
    }
}