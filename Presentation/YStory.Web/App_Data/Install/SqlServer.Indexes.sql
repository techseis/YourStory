CREATE NONCLUSTERED INDEX [IX_LocaleStringResource] ON [LocaleStringResource] ([ResourceName] ASC,  [LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Article_PriceDatesEtc] ON [Article]  ([Price] ASC, [AvailableStartDateTimeUtc] ASC, [AvailableEndDateTimeUtc] ASC, [Published] ASC, [Deleted] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Country_DisplaySubscription] ON [Country] ([DisplaySubscription] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_StateProvince_CountryId] ON [StateProvince] ([CountryId]) INCLUDE ([DisplaySubscription])
GO

CREATE NONCLUSTERED INDEX [IX_Currency_DisplaySubscription] ON [Currency] ( [DisplaySubscription] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Log_CreatedOnUtc] ON [Log] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Email] ON [Customer] ([Email] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_Username] ON [Customer] ([Username] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_CustomerGuid] ON [Customer] ([CustomerGuid] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Customer_SystemName] ON [Customer] ([SystemName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_GenericAttribute_EntityId_and_KeyGroup] ON [GenericAttribute] ([EntityId] ASC, [KeyGroup] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_QueuedEmail_CreatedOnUtc] ON [QueuedEmail] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Subscription_CustomerId] ON [Subscription] ([CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Language_DisplaySubscription] ON [Language] ([DisplaySubscription] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_BlogPost_LanguageId] ON [BlogPost] ([LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_BlogComment_BlogPostId] ON [BlogComment] ([BlogPostId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_News_LanguageId] ON [News] ([LanguageId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_NewsComment_NewsItemId] ON [NewsComment] ([NewsItemId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_NewsletterSubscription_Email_StoreId] ON [NewsLetterSubscription] ([Email] ASC, [StoreId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PollAnswer_PollId] ON [PollAnswer] ([PollId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ArticleReview_ArticleId] ON [ArticleReview] ([ArticleId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_SubscriptionItem_SubscriptionId] ON [SubscriptionItem] ([SubscriptionId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_SubscriptionNote_SubscriptionId] ON [SubscriptionNote] ([SubscriptionId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_TierPrice_ArticleId] ON [TierPrice] ([ArticleId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ShoppingCartItem_ShoppingCartTypeId_CustomerId] ON [ShoppingCartItem] ([ShoppingCartTypeId] ASC, [CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_RelatedArticle_ArticleId1] ON [RelatedArticle] ([ArticleId1] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ArticleAttributeValue_ArticleAttributeMappingId_DisplaySubscription] ON [ArticleAttributeValue] ([ArticleAttributeMappingId] ASC, [DisplaySubscription] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Article_ArticleAttribute_Mapping_ArticleId_DisplaySubscription] ON [Article_ArticleAttribute_Mapping] ([ArticleId] ASC, [DisplaySubscription] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Publisher_DisplaySubscription] ON [Publisher] ([DisplaySubscription] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_DisplaySubscription] ON [Category] ([DisplaySubscription] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_ParentCategoryId] ON [Category] ([ParentCategoryId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Group_DisplaySubscription] ON [Forums_Group] ([DisplaySubscription] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Forum_DisplaySubscription] ON [Forums_Forum] ([DisplaySubscription] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Forum_ForumGroupId] ON [Forums_Forum] ([ForumGroupId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Topic_ForumId] ON [Forums_Topic] ([ForumId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Post_TopicId] ON [Forums_Post] ([TopicId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Post_CustomerId] ON [Forums_Post] ([CustomerId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_ForumId] ON [Forums_Subscription] ([ForumId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Forums_Subscription_TopicId] ON [Forums_Subscription] ([TopicId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Article_Deleted_and_Published] ON [Article] ([Published] ASC, [Deleted] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Article_Published] ON [Article] ([Published] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Article_ShowOnHomepage] ON [Article] ([ShowOnHomePage] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Article_ParentGroupedArticleId] ON [Article] ([ParentGroupedArticleId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Article_VisibleIndividually] ON [Article] ([VisibleIndividually] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PCM_Article_and_Category] ON [Article_Category_Mapping] ([CategoryId] ASC, [ArticleId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PMM_Article_and_Publisher] ON [Article_Publisher_Mapping] ([PublisherId] ASC, [ArticleId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_PSAM_AllowFiltering] ON [Article_SpecificationAttribute_Mapping] ([AllowFiltering] ASC) INCLUDE ([ArticleId],[SpecificationAttributeOptionId])
GO

CREATE NONCLUSTERED INDEX [IX_PSAM_SpecificationAttributeOptionId_AllowFiltering] ON [Article_SpecificationAttribute_Mapping] ([SpecificationAttributeOptionId] ASC, [AllowFiltering] ASC) INCLUDE ([ArticleId])
GO

CREATE NONCLUSTERED INDEX [IX_PSAM_ArticleId] ON [Article_SpecificationAttribute_Mapping] ([ArticleId] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ArticleTag_Name] ON [ArticleTag] ([Name] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ActivityLog_CreatedOnUtc] ON [ActivityLog] ([CreatedOnUtc] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_UrlRecord_Slug] ON [UrlRecord] ([Slug] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_UrlRecord_Custom_1] ON [UrlRecord] ([EntityId] ASC, [EntityName] ASC, [LanguageId] ASC, [IsActive] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_AclRecord_EntityId_EntityName] ON [AclRecord] ([EntityId] ASC, [EntityName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_StoreMapping_EntityId_EntityName] ON [StoreMapping] ([EntityId] ASC, [EntityName] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_LimitedToStores] ON [Category] ([LimitedToStores] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Publisher_LimitedToStores] ON [Publisher] ([LimitedToStores] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Article_LimitedToStores] ON [Article] ([LimitedToStores] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Category_SubjectToAcl] ON [Category] ([SubjectToAcl] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Publisher_SubjectToAcl] ON [Publisher] ([SubjectToAcl] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_Article_SubjectToAcl] ON [Article] ([SubjectToAcl] ASC)
GO