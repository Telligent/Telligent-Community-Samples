using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Caching.Version1;
using Telligent.Evolution.Extensibility.Api.Version1;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;
using PollsApi = Telligent.BigSocial.Polling.PublicApi;

namespace Telligent.BigSocial.Polling.InternalApi
{
	internal static class PollingService
	{
		internal static void AddUpdatePoll(Poll poll)
		{
			bool isCreate = poll.Id == Guid.Empty;

			ValidatePoll(poll);

			if (isCreate)
				PublicApi.Polls.Events.OnBeforeCreate(poll);
			else
				PublicApi.Polls.Events.OnBeforeUpdate(poll);

			PollingDataService.AddUpdatePoll(poll);
            ExpirePolls(poll.ApplicationId);

			if (isCreate)
				PublicApi.Polls.Events.OnAfterCreate(poll);
			else
				PublicApi.Polls.Events.OnAfterUpdate(poll);
		}

		internal static void DeletePoll(Poll poll)
		{
			ValidatePoll(poll);

			PublicApi.Polls.Events.OnBeforeDelete(poll);
			PollingDataService.DeletePoll(poll.Id);
            ExpirePolls(poll.ApplicationId);
			PublicApi.Polls.Events.OnAfterDelete(poll);
		}

		internal static void ReassignPolls(int oldUserId, int newUserId)
		{
			if (oldUserId != newUserId)
			{
				PollingDataService.ReassigneUser(oldUserId, newUserId);
				// We don't expire the cache here because it would be potentially too large of an expiration
			}
		}

        internal static void DeletePolls(int groupId)
        {
            var group = TEApi.Groups.Get(new GroupsGetOptions() { Id = groupId });
            if (group != null)
            {
                PollingDataService.DeletePollsByGroup(group.ApplicationId);
                ExpirePolls(group.ApplicationId);
            }
        }

		internal static Poll GetPoll(Guid pollId)
		{
            Poll poll = (Poll) CacheService.Get(PollCacheKey(pollId), CacheScope.All);
			
            if (poll == null)
			{
				poll = PollingDataService.GetPoll(pollId);
				if (poll != null)
					CacheService.Put(PollCacheKey(pollId), poll, CacheScope.All, new string[] { PollTag(poll.ApplicationId) });
			}

            if (poll != null && PollingPermissionService.CanReadPolls(TEApi.Groups.ContentTypeId, poll.ApplicationId))
                return poll;

            return null;
		}

        internal static PagedList<Poll> ListPolls(int groupId, PollsApi.PollsListOptions options)
		{
            var group = TEApi.Groups.Get(new GroupsGetOptions() { Id = groupId });
            if (group == null)
                return new PagedList<Poll>();

            Guid[] filterGroupIds = null;
            if (options.IncludeSubGroups == true)
            {
                var groupsToQuery = TEApi.Groups.List(new GroupsListOptions() { IncludeAllSubGroups = true, ParentGroupId = groupId, PageSize = 9999 });
                groupsToQuery.Add(group);

                IEnumerable<Guid> ids;
                ids = from g1 in groupsToQuery
                        select g1.ApplicationId;

                filterGroupIds = ids.ToArray();

                if (filterGroupIds.Length == 0)
                    return new PagedList<Poll>();
            }

            //PagedList<Poll> polls = (PagedList<Poll>)CacheService.Get(PollsCacheKey(group.ApplicationId, options), CacheScope.Context | CacheScope.Process);
            //if (polls == null)
            //{
            var polls = PollingDataService.ListPolls(group, TEApi.Users.AccessingUser, filterGroupIds, options);
                //CacheService.Put(PollsCacheKey(group.ApplicationId, options), polls, CacheScope.Context | CacheScope.Process, new string[] { PollTag(group.ApplicationId) });
			//}

			return polls;
		}

		internal static PagedList<Poll> ListPollsToReindex(int pageSize, int pageIndex)
		{
			return PollingDataService.ListPollsToReindex(pageSize, pageIndex);
		}

		internal static void SetPollsAsIndexed(IEnumerable<Guid> pollIds)
		{
			PollingDataService.SetPollsAsIndexed(pollIds);
		}

        internal static List<Poll> ListPollsToSendNewPollEmail()
        {
            return PollingDataService.ListPollsToSendNewPollEmail();
        }

        #region PollAnswer
        internal static void AddUpdatePollAnswer(PollAnswer answer)
		{
			bool isCreate = answer.Id == Guid.Empty;

			ValidatePollAnswer(answer);

			if (isCreate)
				PublicApi.PollAnswers.Events.OnBeforeCreate(answer);
			else
				PublicApi.PollAnswers.Events.OnBeforeUpdate(answer);

			PollingDataService.AddUpdatePollAnswer(answer);
			ExpirePoll(answer.PollId);

			if (isCreate)
				PublicApi.PollAnswers.Events.OnAfterCreate(answer);
			else
				PublicApi.PollAnswers.Events.OnAfterUpdate(answer);
		}

		internal static void DeletePollAnswer(PollAnswer answer)
		{
			ValidatePollAnswer(answer);
			PublicApi.PollAnswers.Events.OnBeforeDelete(answer);
			PollingDataService.DeletePollAnswer(answer.Id);
			ExpirePoll(answer.PollId);
			PublicApi.PollAnswers.Events.OnAfterDelete(answer);
		}

		internal static PollAnswer GetPollAnswer(Guid pollAnswerId)
		{
			var pollAnswer = PollingDataService.GetPollAnswer(pollAnswerId);
			if (pollAnswer != null && GetPoll(pollAnswer.PollId) != null)
				return pollAnswer;

			return null;
		}
        #endregion

        #region PollVote
        internal static void AddUpdatePollVote(PollVote vote)
		{
			ValidatePollVote(vote);

			var existingVote = GetPollVote(vote.PollId, vote.UserId);
			if (existingVote != null && existingVote.PollAnswerId == vote.PollAnswerId)
				return;

			bool isCreate = existingVote == null;
			if (isCreate)
				PublicApi.PollVotes.Events.OnBeforeCreate(vote);
			else
				PublicApi.PollVotes.Events.OnBeforeUpdate(vote);

			PollingDataService.AddUpdatePollVote(vote);
			ExpirePoll(vote.PollId);

			if (isCreate)
				PublicApi.PollVotes.Events.OnAfterCreate(vote);
			else
				PublicApi.PollVotes.Events.OnAfterUpdate(vote);
		}

		internal static void DeletePollVote(PollVote vote)
		{
			ValidatePollVote(vote);
			PublicApi.PollVotes.Events.OnBeforeDelete(vote);
			PollingDataService.DeletePollVote(vote.PollId, vote.UserId);
			ExpirePoll(vote.PollId);
			PublicApi.PollVotes.Events.OnAfterDelete(vote);
		}

		internal static PollVote GetPollVote(Guid pollId, int userId)
		{
			var pollVote = PollingDataService.GetPollVote(pollId, userId);
			if (pollVote != null && GetPoll(pollVote.PollId) != null)
				return pollVote;

			return null;
		}
        #endregion

        internal static string RenderPollDescription(Poll poll, string target)
		{
			if (string.IsNullOrEmpty(target))
				target = "web";
			else
				target = target.ToLowerInvariant();
			
			if (target == "raw")
				return poll.Description ?? string.Empty;
			else
				return PublicApi.Polls.Events.OnRender(poll, "Description", poll.Description ?? string.Empty, target);
		}

		#region Validation

		private static void ValidatePoll(Poll poll)
		{
			if (poll.Id == Guid.Empty)
			{
				poll.CreatedDateUtc = DateTime.UtcNow;
				poll.Id = Guid.NewGuid();
			}

			poll.LastUpdatedDateUtc = DateTime.UtcNow;
			poll.Name = TEApi.Html.Sanitize(TEApi.Html.EnsureEncoded(poll.Name));
			poll.Description = TEApi.Html.Sanitize(poll.Description ?? string.Empty);

			if (poll.HideResultsUntilVotingComplete && !poll.VotingEndDateUtc.HasValue)
				poll.HideResultsUntilVotingComplete = false;

			if (string.IsNullOrEmpty(poll.Name))
				throw new PollException("The name of the poll must be defined.");

			var group = TEApi.Groups.Get(poll.ApplicationId);
			if (group == null || group.HasErrors())
				throw new PollException("The group identified on the poll is invalid.");

			if (!PollingPermissionService.CanCreatePolls(TEApi.Groups.ContentTypeId, group.ApplicationId))
				throw new PollException("The user does not have permission to create polls in this group.");

			if (poll.AuthorUserId <= 0)
				poll.AuthorUserId = TEApi.Users.AccessingUser.Id.Value;
			else if (poll.AuthorUserId != TEApi.Users.AccessingUser.Id.Value && !PollingPermissionService.CanCreatePolls(TEApi.Groups.ContentTypeId, group.ApplicationId))
				throw new PollException("The user does not have permission to create/edit this poll. The user must be the original creator or an admin in the group.");
		}

		private static void ValidatePollAnswer(PollAnswer answer)
		{
			if (answer.Id == Guid.Empty)
				answer.Id = Guid.NewGuid();

			answer.Name = TEApi.Html.Sanitize(TEApi.Html.EnsureEncoded(answer.Name));
			if (string.IsNullOrEmpty(answer.Name))
				throw new PollException("The name of the poll answer must be defined.");

			Poll poll = GetPoll(answer.PollId);
			if (poll == null)
				throw new PollException("The poll associated to the answer does not exist.");

            var group = TEApi.Groups.Get(poll.ApplicationId);
			if (group == null || group.HasErrors())
				throw new PollException("The group identified on the poll is invalid.");

            if (poll.AuthorUserId != TEApi.Users.AccessingUser.Id.Value && !PollingPermissionService.CanCreatePolls(TEApi.Groups.ContentTypeId, group.ApplicationId))
				throw new PollException("The user does not have permission to create/edit this poll. The user must be the original creator or an have create poll permissions in the group.");
		}

		private static void ValidatePollVote(PollVote vote)
		{
			if (vote.CreatedDateUtc == DateTime.MinValue)
				vote.CreatedDateUtc = DateTime.UtcNow;

			vote.LastUpdatedDateUtc = DateTime.UtcNow;

			Poll poll = GetPoll(vote.PollId);
			if (poll == null)
				throw new PollException("The poll associated to the vote does not exist.");

			if (poll.VotingEndDateUtc.HasValue && poll.VotingEndDateUtc.Value < DateTime.UtcNow)
				throw new PollException("Voting has ended. Votes cannot be added or changed.");

			if (!poll.Answers.Any(x => x.Id == vote.PollAnswerId))
				throw new PollException("The poll answer doesn't exist on this poll.");

            var group = TEApi.Groups.Get(poll.ApplicationId);
			if (group == null || group.HasErrors())
				throw new PollException("The group identified on the poll is invalid.");

            if (TEApi.Users.AccessingUser.IsSystemAccount.Value)
                throw new PollException("You must be logged in to vote on a poll");

            if (!PollingPermissionService.CanVoteOnPolls(TEApi.Groups.ContentTypeId, poll.ApplicationId))
				throw new PollException("The user does not have permission to vote on polls in this group.");
		}

		#endregion

		#region Cache-related Methods

		private static void ExpirePoll(Guid pollId)
		{
			CacheService.Remove(PollCacheKey(pollId), CacheScope.All);
		}

        private static void ExpirePolls(Guid applicationId)
		{
            CacheService.RemoveByTags(new string[] { PollTag(applicationId) }, CacheScope.All);
		}

		private static string PollCacheKey(Guid pollId)
		{
			return string.Concat("Polling_PK_Poll:", pollId.ToString("N"));
		}

		private static string PollsCacheKey(Guid applicationId, PollsApi.PollsListOptions options)
		{
			return string.Concat("Polling_PK_Polls:"
                , TEApi.Users.AccessingUser.Id.Value
                , ":", applicationId
                , ":", options.IsEnabled
                , ":", options.IsSuspectedAbusive
                , ":", options.AuthorUserId
                , ":", options.PageIndex
                , ":", options.PageSize
                , ":", options.SortBy
                , ":", options.SortOrder);
		}

        private static string PollTag(Guid applicationId)
		{
            return string.Concat("Polling_TAG_Group:", applicationId);
		}

		#endregion
	}
}
