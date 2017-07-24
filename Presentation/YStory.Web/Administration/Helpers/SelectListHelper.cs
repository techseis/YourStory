using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YStory.Admin.Infrastructure.Cache;
using YStory.Core.Caching;
using YStory.Services.Catalog;
using YStory.Services.Contributors;

namespace YStory.Admin.Helpers
{
    /// <summary>
    /// Select list helper
    /// </summary>
    public static class SelectListHelper
    {
        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="categoryService">Category service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category list</returns>
        public static List<SelectListItem> GetCategoryList(ICategoryService categoryService, ICacheManager cacheManager, bool showHidden = false)
        {
            if (categoryService == null)
                throw new ArgumentNullException("categoryService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORIES_LIST_KEY, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {
                var categories = categoryService.GetAllCategories(showHidden: showHidden);
                return categories.Select(c => new SelectListItem
                {
                    Text = c.GetFormattedBreadCrumb(categories),
                    Value = c.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get publisher list
        /// </summary>
        /// <param name="publisherService">Publisher service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Publisher list</returns>
        public static List<SelectListItem> GetPublisherList(IPublisherService publisherService, ICacheManager cacheManager, bool showHidden = false)
        {
            if (publisherService == null)
                throw new ArgumentNullException("publisherService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.PUBLISHERS_LIST_KEY, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {
                var publishers = publisherService.GetAllPublishers(showHidden: showHidden);
                return publishers.Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get contributor list
        /// </summary>
        /// <param name="contributorService">Contributor service</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Contributor list</returns>
        public static List<SelectListItem> GetContributorList(IContributorService contributorService, ICacheManager cacheManager, bool showHidden = false)
        {
            if (contributorService == null)
                throw new ArgumentNullException("contributorService");

            if (cacheManager == null)
                throw new ArgumentNullException("cacheManager");

            string cacheKey = string.Format(ModelCacheEventConsumer.CONTRIBUTORS_LIST_KEY, showHidden);
            var listItems = cacheManager.Get(cacheKey, () =>
            {
                var contributors = contributorService.GetAllContributors(showHidden: showHidden);
                return contributors.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }
    }
}