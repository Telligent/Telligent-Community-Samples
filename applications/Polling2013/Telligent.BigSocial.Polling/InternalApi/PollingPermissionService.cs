using System;
using System.Linq;
using Telligent.Evolution.Extensibility.Api.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.InternalApi
{
    public class PollPermissions
    {
        internal static Guid ReadPolls = new Guid("0ddb38bd-dd1c-4c98-8c43-9661496fcf1a");
        internal static Guid ModeratePolls = new Guid("5f0a40e0-f2ac-4d96-8cdb-9fd9bdd85ff2");
        internal static Guid CreatePolls = new Guid("af6daaa5-9552-4f66-9411-a21158b44276");
        internal static Guid VoteOnPolls = new Guid("5d6d42f6-6fbc-4b88-9221-9d3762622128");
    }

    internal static class PollingPermissionService
    {
        internal static bool CanReadPolls(Guid contentTypeId, Guid contentId)
        {
            if (contentTypeId == PublicApi.Polls.ContentTypeId)
                return CheckPermission(contentId, PollPermissions.ReadPolls);
            else if (contentTypeId == TEApi.Groups.ContentTypeId)
                return TEApi.NodePermissions.Get("groups", contentId, PollPermissions.ReadPolls.ToString()).IsAllowed;

            return false;
        }

        internal static bool CanModeratePolls(Guid contentTypeId, Guid contentId)
        {
            if (contentTypeId == PublicApi.Polls.ContentTypeId)
                return CheckPermission(contentId, PollPermissions.ModeratePolls);
            else if (contentTypeId == TEApi.Groups.ContentTypeId)
                return TEApi.NodePermissions.Get("groups", contentId, PollPermissions.ModeratePolls.ToString()).IsAllowed;

            return false;
        }

        internal static bool CanCreatePolls(Guid contentTypeId, Guid contentId)
        {
            if (contentTypeId == PublicApi.Polls.ContentTypeId)
                return CheckPermission(contentId, PollPermissions.CreatePolls);
            else if (contentTypeId == TEApi.Groups.ContentTypeId)
                return TEApi.NodePermissions.Get("groups", contentId, PollPermissions.CreatePolls.ToString()).IsAllowed;

            return false;
        }

        internal static bool CanVoteOnPolls(Guid contentTypeId, Guid contentId)
        {
            if (contentTypeId == PublicApi.Polls.ContentTypeId)
                return CheckPermission(contentId, PollPermissions.VoteOnPolls);
            else if (contentTypeId == TEApi.Groups.ContentTypeId)
                return TEApi.NodePermissions.Get("groups", contentId, PollPermissions.VoteOnPolls.ToString()).IsAllowed;

            return false;
        }

        internal static bool CheckPermission(Guid pollId, Guid permissionId)
        {
            var poll = InternalApi.PollingService.GetPoll(pollId);

            if (poll == null)
                return false;

            return TEApi.NodePermissions.Get("groups", poll.ApplicationId, permissionId.ToString()).IsAllowed;
        }
    
    
    }
}
