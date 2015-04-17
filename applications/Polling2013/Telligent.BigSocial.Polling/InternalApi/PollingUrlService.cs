using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telligent.Common.QueryModel;
using Telligent.Evolution.Extensibility.Caching.Version1;
using Telligent.Evolution.Extensibility.Api.Version1;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;
namespace Telligent.BigSocial.Polling.InternalApi
{
    internal static class PollingUrlService
    {
        internal static string PollUrl(Guid pollId)
        {
            var poll = InternalApi.PollingService.GetPoll(pollId);
            if (poll == null)
                return null;

            var group = TEApi.Groups.Get(poll.ApplicationId);
            if (group == null)
                return null;

            return TEApi.Url.BuildUrl("polls_view", group.Id.Value, new Dictionary<string, string>() {{ "PollId", poll.Id.ToString() }});
        }

        internal static string CreatePollUrl(int groupId, bool checkPermissions)
        {
            var group = TEApi.Groups.Get(new GroupsGetOptions { Id = groupId });
            if (group == null)
                return null;

            if (checkPermissions && !InternalApi.PollingPermissionService.CanCreatePolls(TEApi.Groups.ContentTypeId, group.ApplicationId))
                return null;

            return TEApi.Url.BuildUrl("polls_create", groupId, null);
        }

        internal static string EditPollUrl(Guid pollId, bool checkPermissions)
        {
            var poll = InternalApi.PollingService.GetPoll(pollId);
            if (poll == null)
                return null;

            var group = TEApi.Groups.Get(poll.ApplicationId);
            if (group == null)
                return null;

            if (checkPermissions && (poll.AuthorUserId != TEApi.Users.AccessingUser.Id.Value || !InternalApi.PollingPermissionService.CanModeratePolls(PublicApi.Polls.ContentTypeId, pollId)))
                return null;

            return TEApi.Url.BuildUrl("polls_edit", group.Id.Value, new Dictionary<string, string>() { { "PollId", poll.Id.ToString() } });
        }

        internal static string PollListUrl(int groupId, bool checkPermissions = true)
        {
            if (checkPermissions)
            {
                var group = TEApi.Groups.Get(new GroupsGetOptions {Id = groupId});
                if (group == null)
                    return null;

                if (!InternalApi.PollingPermissionService.CanReadPolls(TEApi.Groups.ContentTypeId, group.ApplicationId))
                    return null;
            }

            return TEApi.Url.BuildUrl("polls_list", groupId, null);
        }


        internal static string PollAnswerVoteUrl(Guid pollAnswerId)
        {
            var pollAnswer = InternalApi.PollingService.GetPollAnswer(pollAnswerId);
            if (pollAnswer == null)
                return null;

            var poll = InternalApi.PollingService.GetPoll(pollAnswer.PollId);
            if (poll == null)
                return null;

            var group = TEApi.Groups.Get(poll.ApplicationId);
            if (group == null)
                return null;

            return TEApi.Url.BuildUrl("polls_vote", group.Id.Value, new Dictionary<string, string>() { { "PollId", poll.Id.ToString() }, { "PollAnswerId", pollAnswer.Id.ToString() } });
        }


        internal static string UserPolls(int userId)
        {
            var user = TEApi.Users.Get(new UsersGetOptions() { Id = userId });

            if (user == null)
                return null;

            return TEApi.Url.BuildUrl("poll_userpolls", new Dictionary<string, string>() {{ "UserName", user.Username }});
        }

    }
}
