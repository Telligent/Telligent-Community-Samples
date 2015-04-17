using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.Content.Version1;
using Telligent.Evolution.Extensibility.Api.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.Plugins
{
	public class PollActivityStoryType : IPlugin, ITranslatablePlugin, IActivityStoryType, ICategorizedPlugin
	{
		private readonly Guid _storyTypeId = new Guid("03532a5baee6425cafb365f3c1aab459");
		IActivityStoryController _storyController;
		ITranslatablePluginController _translation;

		#region IPlugin Members

		public string Name
		{
            get { return "Big Social Polling - Poll Activity"; }
		}

		public string Description
		{
			get { return "Logs polling activity to the activity stream.";  }
		}

		public void Initialize()
		{
			PublicApi.Polls.Events.AfterCreate += new PublicApi.PollAfterCreateEventHandler(Events_AfterCreate);
			PublicApi.PollVotes.Events.AfterCreate += new PublicApi.PollVoteAfterCreateEventHandler(Events_AfterCreate);
		}

		void Events_AfterCreate(PublicApi.PollVoteAfterCreateEventArgs e)
		{
			var story = GetStory(e.PollId);
			if (story != null)
			{
				_storyController.Update(story.StoryId, new ActivityStoryUpdateOptions
				{
					LastUpdate = DateTime.UtcNow
				});
			}
		}

		void Events_AfterCreate(PublicApi.PollAfterCreateEventArgs e)
		{
			if (e.IsEnabled)
			{
				var story = GetStory(e.Id);
				if (story == null)
				{
					_storyController.Create(new ActivityStoryCreateOptions
					{
						ContentId = e.ContentId,
						ContentTypeId = PublicApi.Polls.ContentTypeId,
						LastUpdate = DateTime.UtcNow,
						Actors = new List<ActivityStoryActor>
						{
							new ActivityStoryActor
							{
								UserId = e.AuthorUserId,
								Verb = "Add",
								Date = DateTime.UtcNow
							}
						}
					});
				}
			}
		}

		private IActivityStory GetStory(Guid pollId)
		{
			return TEApi.ActivityStories.List(new ActivityStoryListOptions()
			{
				ContentId = pollId,
				ContentTypeId = PublicApi.Polls.ContentTypeId,
				Filters = new List<ActivityStoryFilter> {
						new ActivityStoryFilter
						{
							StoryTypeId = StoryTypeId,
							IncludedUsers = "All"
						}
					},
				PageSize = 1
			}).FirstOrDefault();
		}

		#endregion

		#region ITranslatablePlugin Members

		public Translation[] DefaultTranslations
		{
			get
			{
				var translation = new Translation("en-US");
				translation.Set("activity_preview_format", @"<a href=""{1}"">{0}</a> created the poll <a href=""{3}"">{2}</a>.");
				translation.Set("activity_view_in", " in ");
				translation.Set("activity_view_withvotesformat", " with {0} votes.");
				translation.Set("activity_type_name", "Poll Activity");
				translation.Set("activity_type_description", "Logs new polls and votes on polls.");
				translation.Set("activity_nopermission", "You don't have permission to view this poll or the poll no longer exists.");

				return new Translation[] { translation };
			}
		}

		public void SetController(ITranslatablePluginController controller)
		{
			_translation = controller;
		}

		#endregion

		#region IActivityStoryType Members

		public bool CanDeleteStory(Guid storyId, int userId)
		{
			var story = TEApi.ActivityStories.Get(storyId);
			if (story == null || !story.ContentId.HasValue)
				return false;

            return InternalApi.PollingPermissionService.CanModeratePolls(PublicApi.Polls.ContentTypeId, story.ContentId.Value);
		}

		public Guid[] ContentTypeIds
		{
			get { return new Guid[] { PublicApi.Polls.ContentTypeId }; }
		}

		public string GetPreviewHtml(IActivityStory story, Target target)
		{
			var poll = InternalApi.PollingService.GetPoll(story.ContentId.Value);
			if (poll == null)
				return _translation.GetLanguageResourceValue("activity_nopermission");

			var userId = GetPrimaryUser(story);
			if (userId == null)
				return null;

			var user = TEApi.Users.Get(new UsersGetOptions { Id = userId });
			if (user == null || user.HasErrors())
				return null;

			return string.Format(_translation.GetLanguageResourceValue("activity_preview_format"),
				user.DisplayName,
				TEApi.Html.EncodeAttribute(user.ProfileUrl),
				poll.Name,
                TEApi.Html.EncodeAttribute(InternalApi.PollingUrlService.PollUrl(poll.Id))
				);			
		}

		public int? GetPrimaryUser(IActivityStory story)
		{
			return story.Actors.Where(x => x.Verb == "Add").Select(x => (int?) x.UserId).FirstOrDefault();
		}

		public string GetViewHtml(IActivityStory story, Target target)
		{
			var poll = InternalApi.PollingService.GetPoll(story.ContentId.Value);
			if (poll == null)
				return _translation.GetLanguageResourceValue("activity_nopermission");

			var userId = GetPrimaryUser(story);
			if (userId == null)
				return null;

			var user = TEApi.Users.Get(new UsersGetOptions { Id = userId });
			if (user == null || user.HasErrors())
				return null;

			var group = TEApi.Groups.Get(poll.ApplicationId);
			if (group == null || group.HasErrors())
				return _translation.GetLanguageResourceValue("activity_nopermission");

			StringBuilder html = new StringBuilder();
			html.Append(@"<div class=""activity-summary"">
	<a href=""");
			html.Append(TEApi.Html.EncodeAttribute(user.ProfileUrl));
			html.Append(@""">");
			html.Append(user.DisplayName);
			html.Append("</a>");
			html.Append(_translation.GetLanguageResourceValue("activity_view_in"));
			html.Append(@"<a href=""");
			html.Append(TEApi.Html.EncodeAttribute(group.Url));
			html.Append(@"""><span></span>");
			html.Append(group.Name);
			html.Append(@"</a>");
			html.Append(string.Format(_translation.GetLanguageResourceValue("activity_view_withvotesformat"), TEApi.Language.FormatNumber(poll.Answers.Sum(x => x.VoteCount))));
			html.Append(@"
</div>
<span class=""activity-title"">
	<a href=""");
            html.Append(TEApi.Html.EncodeAttribute(InternalApi.PollingUrlService.PollUrl(poll.Id)));
			html.Append(@""" class=""internal-link view-poll"" title=""");
			html.Append(poll.Name);
			html.Append(@""">");
			html.Append(poll.Name);
			html.Append(@"</a>
</span>
<div class=""activity-description"">");
			html.Append(PublicApi.Polls.UI(poll.Id, false, false));
			html.Append(@"
</div>");

			return html.ToString();	
		}

		public bool IsCacheable
		{
			get { return true; }
		}

		public void SetController(IActivityStoryController controller)
		{
			_storyController = controller;
		}

		public Guid StoryTypeId
		{
			get { return _storyTypeId; }
		}

		public bool VaryCacheByUser
		{
			get { return true; }
		}
		
		public string StoryTypeDescription
		{
			get { return _translation.GetLanguageResourceValue("activity_type_description"); }
		}

		public string StoryTypeName
		{
			get { return _translation.GetLanguageResourceValue("activity_type_name"); }
		}

		#endregion

        #region ICategorizedPlugin
        string[] ICategorizedPlugin.Categories
        {
            get { return new[] { "Polling", "Translatable" }; }
        }
        #endregion
	}
}
