using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using Internal = Telligent.BigSocial.Polling.InternalApi;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.PublicApi
{
	public static class PollVotes
	{
		private static readonly PollVoteEvents _events = new PollVoteEvents();

		public static PollVoteEvents Events
		{
			get { return _events; }
		}

		public static PollVote Get(Guid pollId)
		{
			try
			{
				var vote = Internal.PollingService.GetPollVote(pollId, TEApi.Users.AccessingUser.Id.Value);
				if (vote == null)
					return null;

				return new PollVote(vote);
			}
			catch (Exception ex)
			{
				return new PollVote(new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message)));
			}
		}

		public static AdditionalInfo Delete(Guid pollId)
		{
			try
			{
				var pollVote = Internal.PollingService.GetPollVote(pollId, TEApi.Users.AccessingUser.Id.Value);
				if (pollVote != null)
					Internal.PollingService.DeletePollVote(pollVote);

				return new AdditionalInfo();
			}
			catch (Exception ex)
			{
				return new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message));
			}
		}

		public static PollVote Update(Guid pollId, Guid pollAnswerId)
		{
			try
			{
				var pollVote = new Internal.PollVote();
				pollVote.PollId = pollId;
				pollVote.PollAnswerId = pollAnswerId;
				pollVote.UserId = TEApi.Users.AccessingUser.Id.Value;

				Internal.PollingService.AddUpdatePollVote(pollVote);

				return Get(pollId);
			}
			catch (Exception ex)
			{
				return new PollVote(new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message)));
			}
		}
	}
}
