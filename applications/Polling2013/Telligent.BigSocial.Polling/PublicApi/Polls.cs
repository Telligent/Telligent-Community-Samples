using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using Internal = Telligent.BigSocial.Polling.InternalApi;
using Telligent.Evolution.Extensibility.Api.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.PublicApi
{

    public class PollsListOptions
    {
        private string _sortBy = "date";
        private string _sortOrder = "descending";
        private bool? _isEnabled = true;
        private bool? _isSuspectedAbusive = false;
        private bool _includeSubGroups = false;
        public int? AuthorUserId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        private bool _ignorePermissions = false;
        private Guid _permission = InternalApi.PollPermissions.ReadPolls;

        public bool? IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public bool? IsSuspectedAbusive
        {
            get { return _isSuspectedAbusive; }
            set { _isSuspectedAbusive = value; }
        }

        public bool IncludeSubGroups
        {
            get { return _includeSubGroups; }
            set { _includeSubGroups = value; }
        }

        public string SortBy
        {
            get { return string.IsNullOrEmpty(_sortBy) ? "date" : _sortBy; }
            set { _sortBy = value; }
        }

        public string SortOrder
        {
            get { return string.IsNullOrEmpty(_sortOrder) ? "descending" : _sortOrder; }
            set { _sortOrder = value; }
        }

        public bool IgnorePermissions
        {
            get { return _ignorePermissions; }
            set { _ignorePermissions = value; }
        }

        public Guid Permission
        {
            get { return _permission; }
            set { _permission = value; }
        }
    }

	public static class Polls
	{
        private static readonly Guid _contentTypeId = new Guid("DDD33242-6936-4EC6-8484-6A80C18160D7");
		private static readonly PollEvents _events = new PollEvents();

		public static Guid ContentTypeId { get { return _contentTypeId; } }

		public static PollEvents Events
		{
			get { return _events; }
		}

		public static Poll Get(Guid id)
		{
			try
			{
				var poll = Internal.PollingService.GetPoll(id);
				if (poll == null)
					return null;

				return new Poll(poll);
			}
			catch (Exception ex)
			{
				return new Poll(new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message)));
			}
		}

		public static AdditionalInfo Delete(Guid id)
		{
			try
			{
				var poll = Internal.PollingService.GetPoll(id);
				if (poll != null)
					Internal.PollingService.DeletePoll(poll);

				return new AdditionalInfo();
			}
			catch (Exception ex)
			{
				return new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message));
			}
		}

		public static Poll Create(int groupId, string name, string description = null, DateTime? votingEndDate = null, bool? hideResultsUntilVotingComplete = false)
		{
            var group = TEApi.Groups.Get(new GroupsGetOptions() { Id = groupId });
            
			try
			{
				var poll = new Internal.Poll();
				poll.ApplicationId = group.ApplicationId;
				poll.AuthorUserId = TEApi.Users.AccessingUser.Id.Value;
				poll.Name = name;
				poll.Description = description;
				poll.IsEnabled = true;
				poll.VotingEndDateUtc = votingEndDate;
				poll.HideResultsUntilVotingComplete = hideResultsUntilVotingComplete.HasValue && hideResultsUntilVotingComplete.Value;

				Internal.PollingService.AddUpdatePoll(poll);

				return Get(poll.Id);
			}
			catch (Exception ex)
			{
				return new Poll(new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message)));
			}
		}

		public static Poll Update(Guid id, string name = null, string description = null, DateTime? votingEndDate = null, bool? hideResultsUntilVotingComplete = null, bool? clearVotingEndDate = false)
		{
			try
			{
				var poll = Internal.PollingService.GetPoll(id);
				if (poll != null)
				{
					if (name != null)
						poll.Name = name;

					if (description != null)
						poll.Description = description;

					if (votingEndDate != null)
						poll.VotingEndDateUtc = (DateTime?) Internal.Formatting.FromUserTimeToUtc(votingEndDate.Value);
					else if (clearVotingEndDate.HasValue && clearVotingEndDate.Value)
						poll.VotingEndDateUtc = null;

					if (hideResultsUntilVotingComplete.HasValue)
						poll.HideResultsUntilVotingComplete = hideResultsUntilVotingComplete.Value;

					Internal.PollingService.AddUpdatePoll(poll);
				}

				return Get(id);
			}
			catch (Exception ex)
			{
				return new Poll(new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message)));
			}
		}

		public static PagedList<Poll> List(int groupId, PollsListOptions options)
		{
            if (options == null)
                options = new PollsListOptions();

            if (options.PageSize > 100)
                options.PageSize = 100;
            else if (options.PageSize < 1)
                options.PageSize = 1;

            if (options.PageIndex < 0)
                options.PageIndex = 0;

			try
			{
				if (string.Equals(options.SortBy, "TopPollsScore", StringComparison.OrdinalIgnoreCase))
				{
					var group = TEApi.Groups.Get(new GroupsGetOptions { Id = groupId });
					if (group == null || group.HasErrors())
						return new PagedList<Poll>();

					var scores = TEApi.CalculatedScores.List(Plugins.TopPollsScore.ScoreId, new CalculatedScoreListOptions { ApplicationId = group.ApplicationId, ContentTypeId = ContentTypeId, PageIndex = options.PageIndex, PageSize = options.PageSize, SortOrder = "Descending" });

					var polls = new List<Poll>();
					foreach (var score in scores)
					{
						if (score.Content != null)
						{
							var poll = Get(score.Content.ContentId);
							if (poll != null)
								polls.Add(poll);
						}
					}

					return new PagedList<Poll>(polls, scores.PageSize, scores.PageIndex, scores.TotalCount);
				}
				else
				{
					var polls = InternalApi.PollingService.ListPolls(groupId, options);
					return new PagedList<Poll>(polls.Select(x => new Poll(x)), polls.PageSize, polls.PageIndex, polls.TotalCount);
				}				
			}
			catch (Exception ex)
			{
				return new PagedList<Poll>(new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message)));
			}
		}

		public static bool CanCreate(int groupId)
		{
            var group = TEApi.Groups.Get(new GroupsGetOptions() { Id = groupId });

            if (group == null)
                return false;

			return InternalApi.PollingPermissionService.CanCreatePolls(TEApi.Groups.ContentTypeId, group.ApplicationId);
		}

		public static bool CanVote(Guid pollId)
		{
            return InternalApi.PollingPermissionService.CanVoteOnPolls(PublicApi.Polls.ContentTypeId, pollId);
		}

		public static bool CanEdit(Guid pollId)
		{
            return InternalApi.PollingPermissionService.CanModeratePolls(PublicApi.Polls.ContentTypeId, pollId);
		}

		public static bool CanDelete(Guid pollId)
		{
            return InternalApi.PollingPermissionService.CanModeratePolls(PublicApi.Polls.ContentTypeId, pollId);
		}

		public static string UI(Guid pollId, bool readOnly = false, bool showNameAndDescription = true)
		{
			return string.Concat(
				"<div class=\"ui-poll\" data-pollid=\"",
				pollId.ToString(),
				"\" data-readonly=\"",
				(readOnly || !CanVote(pollId)).ToString().ToLower(),
				"\" data-showname=\"",
				showNameAndDescription.ToString().ToLower(),
				"\"></div>");
		}
	}
}
