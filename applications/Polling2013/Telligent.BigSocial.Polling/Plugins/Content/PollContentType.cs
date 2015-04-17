using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Api.Content;
using Telligent.Evolution.Extensibility.Content.Version1;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using Telligent.Evolution.Extensibility.Api.Version1;
using Telligent.Evolution.Extensibility.UI.Version1;
using Telligent.Evolution.Urls.Routing;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.Plugins
{
	public class PollContentType : IPlugin, IContentType, ITranslatablePlugin, ICommentableContentType, IAbuseCheckingContentType, ITaggableContentType, IHashTaggableContentType, IMentionContainingContentType, ISearchableContentType
        , ISecuredLikeViewContentType, ISecuredMentionViewContentType, ISecuredTaggableViewContentType, ISecuredCommentViewContentType, IViewableContentType, IWebContextualContentType
	{
		ITranslatablePluginController _translation;
		IContentStateChanges _contentState;
		IHashTagController _hashTagController;
		IMentionController _mentionController;

		#region IPlugin Members

		public string Name
		{
			get { return "Poll Content Type"; }
		}

		public string Description
		{
			get { return "Exposes polls to the Evolution content model."; }
		}

		public void Initialize()
		{
			PublicApi.Polls.Events.AfterCreate += new PublicApi.PollAfterCreateEventHandler(Events_AfterCreate);
			PublicApi.Polls.Events.AfterUpdate += new PublicApi.PollAfterUpdateEventHandler(Events_AfterUpdate);
			PublicApi.Polls.Events.Render += new PublicApi.PollRenderEventHandler(Events_Render);
			PublicApi.Polls.Events.AfterDelete += new PublicApi.PollAfterDeleteEventHandler(Events_AfterDelete);

			TEApi.Groups.Events.AfterDelete += new GroupAfterDeleteEventHandler(Events_AfterDelete);
			TEApi.Users.Events.AfterDelete += new UserAfterDeleteEventHandler(Events_AfterDelete);
		}

		void Events_AfterDelete(UserAfterDeleteEventArgs e)
		{
			if (e.Id.HasValue && e.ReassignedUserId.HasValue)
				InternalApi.PollingService.ReassignPolls(e.Id.Value, e.ReassignedUserId.Value);
		}

		void Events_AfterDelete(GroupAfterDeleteEventArgs e)
		{
			if (e.Id.HasValue)
				InternalApi.PollingService.DeletePolls(e.Id.Value);
		}

		void Events_AfterCreate(PublicApi.PollAfterCreateEventArgs e)
		{
			if (_hashTagController != null)
			{
				_hashTagController.AddUpdateHashTags(e.ContentId, "Description", e.Description("raw"));

				var tags = _hashTagController.GetHashTagsInUnRenderedHtml(e.Name);
				if (tags != null && tags.Length > 0)
				{
					TEApi.Tags.Add(e.ContentId, e.ContentTypeId, null, string.Join(",", tags));
					var name = _hashTagController.FormatHashTagsInUnRenderedHtml(e.Name);
					PublicApi.Polls.Update(e.Id, name);
				}
			}

			if (_mentionController != null)
				_mentionController.AddUpdateLoggedMentions(e.ContentId, "Description", e.Description("raw"));
		}

		void Events_AfterUpdate(PublicApi.PollAfterUpdateEventArgs e)
		{
			if (_hashTagController != null)
			{
				_hashTagController.AddUpdateHashTags(e.ContentId, "Description", e.Description("raw"));

				var tags = _hashTagController.GetHashTagsInUnRenderedHtml(e.Name);
				if (tags != null && tags.Length > 0)
				{
					TEApi.Tags.Add(e.ContentId, e.ContentTypeId, null, string.Join(",", tags));
					var name = _hashTagController.FormatHashTagsInUnRenderedHtml(e.Name);
					PublicApi.Polls.Update(e.Id, name);
				}
			}

			if (_mentionController != null)
				_mentionController.AddUpdateLoggedMentions(e.ContentId, "Description", e.Description("raw"));
		}

		void Events_AfterDelete(PublicApi.PollAfterDeleteEventArgs e)
		{
			if (_contentState != null)
				_contentState.Deleted(e.ContentId);
		}

		void Events_Render(PublicApi.PollRenderEventArgs e)
		{
			if (e.RenderedProperty == "Description")
			{
				if (_hashTagController != null)
					e.RenderedHtml = _hashTagController.FormatHashTagsForRendering(Get(e.ContentId), e.RenderedHtml, e.RenderTarget);

				if (_mentionController != null)
					e.RenderedHtml = _mentionController.FormatMentionsForRendering(e.RenderedHtml, e.RenderTarget);
			}
		}

		#endregion

		#region IContentType Members

		public Guid[] ApplicationTypes
		{
			get { return new Guid[] { TEApi.Groups.ApplicationTypeId }; }
		}

		public void AttachChangeEvents(IContentStateChanges stateChanges)
		{
			_contentState = stateChanges;
		}

		public Guid ContentTypeId
		{
			get { return PublicApi.Polls.ContentTypeId; }
		}

		public string ContentTypeName
		{
			get { return _translation.GetLanguageResourceValue("content_type_name"); }
		}

		public IContent Get(Guid contentId)
		{
			return PublicApi.Polls.Get(contentId);
		}

		#endregion

        #region ISecuredContentType Members

        public Guid ContentPermissionId
        {
            get { return ContentTypeId; }
        }

        public Guid PermissionId
        {
            get { return InternalApi.PollPermissions.ReadPolls; }
        }

        public Guid GetSecurableId(IContent content)
        {
            return content.Application.ApplicationId;
        }

        public Guid GetContentPermissionId(IContent content)
        {
            return ContentTypeId;
        }

        public Guid DefaultContentPermissionId
        {
            get { return ContentTypeId; }
        }

        public Guid DefaultPermissionId
        {
            get { return InternalApi.PollPermissions.ReadPolls; }
        }

        #endregion

		#region ITranslatablePlugin Members

		public Translation[] DefaultTranslations
		{
			get 
			{
				var translation = new Translation("en-US");
				translation.Set("content_type_name", "Poll");
				translation.Set("poll_nopermission", "You don't have access to this poll or the poll has been deleted.");

				return new Translation[] { translation };
			}
		}

		public void SetController(ITranslatablePluginController controller)
		{
			_translation = controller;
		}

		#endregion

		#region ICommentableContentType Members

		public bool CanCreateComment(Guid contentId, int userId)
		{
            return InternalApi.PollingPermissionService.CanReadPolls(PublicApi.Polls.ContentTypeId, contentId);
		}

		public bool CanDeleteComment(Guid commentId, int userId)
		{
			var comment = TEApi.Comments.Get(new CommentGetOptions() { CommentIds = new Guid[] { commentId } }).FirstOrDefault();
			if (comment == null)
				return false;

			if (comment.UserId == userId)
				return true;

			var poll = InternalApi.PollingService.GetPoll(comment.ContentId);
			if (poll == null)
				return false;

            return InternalApi.PollingPermissionService.CanModeratePolls(PublicApi.Polls.ContentTypeId, poll.Id);
		}

		public bool CanModifyComment(Guid commentId, int userId)
		{
			return CanDeleteComment(commentId, userId);
		}

		public bool CanReadComment(Guid commentId, int userId)
		{
			var comment = TEApi.Comments.Get(new CommentGetOptions() { CommentIds = new Guid[] { commentId } }).FirstOrDefault();
			if (comment == null)
				return false;

			var poll = InternalApi.PollingService.GetPoll(comment.ContentId);
			if (poll == null)
				return false;

            return InternalApi.PollingPermissionService.CanReadPolls(PublicApi.Polls.ContentTypeId, poll.Id);
        }

		#endregion

		#region IAbuseCheckingContentType Members

		public bool CanUserReviewAppeals(Guid contentId, int userId)
		{
            return InternalApi.PollingPermissionService.CanModeratePolls(PublicApi.Polls.ContentTypeId, contentId);
		}

		public void ContentConfirmedAbusive(Guid abuseId, Guid contentId)
		{
			var poll = InternalApi.PollingService.GetPoll(contentId);
			if (poll != null)
				InternalApi.PollingService.DeletePoll(poll);
		}

		public void ContentFoundNotAbusive(Guid abuseId, Guid contentId)
		{
			var poll = InternalApi.PollingService.GetPoll(contentId);
			if (poll != null)
			{
				poll.IsEnabled = true;
				InternalApi.PollingService.AddUpdatePoll(poll);
			}
		}

		public void ContentSuspectedAbusive(Guid abuseId, Guid contentId)
		{
			var poll = InternalApi.PollingService.GetPoll(contentId);
			if (poll != null)
			{
				poll.IsEnabled = false;
				InternalApi.PollingService.AddUpdatePoll(poll);
			}
		}

		public IContent GetHiddenContent(Guid contentId)
		{
			var poll = InternalApi.PollingService.GetPoll(contentId);
			if (poll != null)
				return new PublicApi.Poll(poll);
			else
				return null;
		}

		public List<int> GetReviewBoardUsers(Guid contentId)
		{
			List<int> userIds = new List<int>();

			var poll = InternalApi.PollingService.GetPoll(contentId);
			if (poll == null)
				return userIds;

            var group = TEApi.Groups.Get(poll.ApplicationId);
            if (group == null)
                return userIds;

			var roles = TEApi.Roles.List(new RolesListOptions { PermissionId = "Group_ReviewAbuseAppeals", Application = "groups", Id = group.Id.Value, Include = "granted" });
			foreach (var role in roles)
				foreach (var user in TEApi.Users.List(new UsersListOptions { RoleId = role.Id, PageSize = 100, PageIndex = 0, IncludeHidden = false }))
					userIds.Add(user.Id.Value);

			return userIds;
		}

		#endregion

		#region ITaggableContentType Members

		public bool CanAddTags(Guid contentId, int userId)
		{
            return InternalApi.PollingPermissionService.CanCreatePolls(PublicApi.Polls.ContentTypeId, contentId);
		}

		public bool CanRemoveTags(Guid contentId, int userId)
		{
            return InternalApi.PollingPermissionService.CanCreatePolls(PublicApi.Polls.ContentTypeId, contentId);
		}

		#endregion

		#region IHashTaggableContentType Members

		public void SetController(IHashTagController controller)
		{
			_hashTagController = controller;
		}

		#endregion

		#region IMentionContainingContentType Members

		public void SetController(IMentionController controller)
		{
			_mentionController = controller;
		}

		#endregion

		#region ISearchableContentType Members

		public IList<SearchIndexDocument> GetContentToIndex()
		{
			var pollsToIndex = InternalApi.PollingService.ListPollsToReindex(500, 0);
			var searchDocuments = new List<SearchIndexDocument>();

			foreach (var poll in pollsToIndex)
			{
                var searchDocument = TEApi.SearchIndexing.NewDocument(poll.Id, PublicApi.Polls.ContentTypeId, "poll", InternalApi.PollingUrlService.PollUrl(poll.Id), poll.Name, InternalApi.PollingService.RenderPollDescription(poll, "unknown"));

				searchDocument.AddField(TEApi.SearchIndexing.Constants.IsContent, true.ToString());
                searchDocument.AddField(TEApi.SearchIndexing.Constants.ContentID, poll.Id.ToString());
				searchDocument.AddField(TEApi.SearchIndexing.Constants.Date, TEApi.SearchIndexing.FormatDate(poll.CreatedDateUtc));
				searchDocument.AddField(TEApi.SearchIndexing.Constants.Category, "polls");
                searchDocument.AddField(TEApi.SearchIndexing.Constants.CollapseField, "polls:" + poll.Id.ToString());

				var user = TEApi.Users.Get(new UsersGetOptions() { Id = poll.AuthorUserId });
				if (user != null && !user.HasErrors())
                {
                    searchDocument.AddField(TEApi.SearchIndexing.Constants.UserID, user.Id.ToString());
                    searchDocument.AddField(TEApi.SearchIndexing.Constants.UserDisplayName, user.DisplayName);
                    searchDocument.AddField(TEApi.SearchIndexing.Constants.Username, user.Username);
                    searchDocument.AddField(TEApi.SearchIndexing.Constants.CreatedBy, user.DisplayName);
                }

				var group = TEApi.Groups.Get(poll.ApplicationId);
				if (group != null && !group.HasErrors())
				{
					searchDocument.AddField(TEApi.SearchIndexing.Constants.ApplicationId, group.ApplicationId.ToString());
					searchDocument.AddField(TEApi.SearchIndexing.Constants.GroupID, group.Id.ToString());
					if (group.ParentGroupId.HasValue)
						searchDocument.AddField(TEApi.SearchIndexing.Constants.ParentGroupID, group.ParentGroupId.Value.ToString());
					searchDocument.AddField(TEApi.SearchIndexing.Constants.ContainerId, group.ContainerId.ToString());
				}

				var tags = TEApi.Tags.Get(poll.Id, PublicApi.Polls.ContentTypeId, null);
				if (tags != null)
				{
					foreach (var tag in tags)
					{
						searchDocument.AddField(TEApi.SearchIndexing.Constants.TagKeyword, tag.TagName.ToLower());
						searchDocument.AddField(TEApi.SearchIndexing.Constants.Tag, tag.TagName);
					}
				}

				searchDocuments.Add(searchDocument);
			}

			return searchDocuments;
		}

		public string GetViewHtml(IContent content, Target target)
		{
            if (content == null)
                return null;

            var poll = InternalApi.PollingService.GetPoll(content.ContentId);
            if (poll == null) return string.Empty;

            var user = TEApi.Users.Get(new UsersGetOptions { Id = poll.AuthorUserId });
            var group = TEApi.Groups.Get(poll.ApplicationId);

            var sb = new StringBuilder(@"<div class=""content abbreviated rendered poll"">");

            sb.Append(@"<div class=""author"">");
            sb.Append(@"<div class=""avatar"">");
            sb.AppendFormat(@"<a href=""{0}"" class=""internal-link view-user-profile"">{1}</a>", TEApi.Html.EncodeAttribute(user.Url), TEApi.UI.GetResizedImageHtml(user.AvatarUrl, 44, 44, new UiGetResizedImageHtmlOptions { OutputIsPersisted = false, ResizeMethod = "ZoomAndCrop" }));
            sb.Append(@"</div>");
            sb.AppendFormat(@"<span class=""user-name""><a href=""{0}"" class=""internal-link view-user-profile"">{1}</a></span>", TEApi.Html.EncodeAttribute(user.Url), user.DisplayName);
            sb.Append(@"</a></div>");

            sb.Append(@"<div class=""attributes""><ul class=""attribute-list"">");
            sb.AppendFormat(@"<li class=""attribute-item date"">{0}</li>", TEApi.Language.FormatAgoDate(poll.CreatedDateUtc.ToLocalTime()));
            sb.AppendFormat(@"<li class=""attribute-item container"">{0} <a href=""{1}"">{2}</a></li>", _translation.GetLanguageResourceValue("In"), TEApi.Html.EncodeAttribute(group.Url), group.Name);
            sb.Append(@"</ul></div>");

            sb.AppendFormat(
                @"<h3 class=""name""><a class=""internal-link"" title=""{0}"" href=""{1}"">{0}</a></h3>",
                content.HtmlName("Web"), TEApi.Html.EncodeAttribute((InternalApi.PollingUrlService.PollUrl(poll.Id))));

            sb.AppendFormat(@"<div class=""content empty"">{0}</div>", TEApi.Language.Truncate(content.HtmlDescription("Web"), 200, "..."));
            //PublicApi.Polls.UI(poll.Id, true, false)

            sb.Append("</div>");

            return sb.ToString();
		}

		public int[] GetViewSecurityRoles(Guid contentId)
		{
			var poll = InternalApi.PollingService.GetPoll(contentId);
			if (poll != null)
			{
				var group = TEApi.Groups.Get(poll.ApplicationId);
				if (!group.HasErrors())
				{
					var groupSearchType = PluginManager.Get<ISearchableContentType>().FirstOrDefault(x => x.ContentTypeId == TEApi.Groups.ContentTypeId);
					if (groupSearchType != null)
						return groupSearchType.GetViewSecurityRoles(group.ContainerId);
				}
			}

			return new int[0];
		}

		public bool IsCacheable
		{
			get { return true; }
		}

		public void SetIndexStatus(Guid[] contentIds, bool isIndexed)
		{
			if (isIndexed)
				InternalApi.PollingService.SetPollsAsIndexed(contentIds);
		}

		public bool VaryCacheByUser
		{
			get { return true; }
		}

		#endregion

        #region IWebContextualContentType
        IContent IWebContextualContentType.GetCurrentContent(IWebContext context)
        {
            //if (ec.RawUrl == context.Url)
            //{
            var item = TEApi.Url.CurrentContext.ContextItems.GetItemByContentType(ContentTypeId);

            if (item != null && item.ContentId.HasValue)
                return Polling.PublicApi.Polls.Get(item.ContentId.Value);
            //}

            return null;
        }
        #endregion

	}
}
