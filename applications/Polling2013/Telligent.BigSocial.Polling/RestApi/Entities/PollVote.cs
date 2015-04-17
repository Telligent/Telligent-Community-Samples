using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telligent.BigSocial.Polling.RestApi
{
	public class PollVote
	{
		public PollVote()
		{
		}

		internal PollVote(InternalApi.PollVote pollVote)
		{
			PollId = pollVote.PollId;
			Answer = new PollAnswer(InternalApi.PollingService.GetPollAnswer(pollVote.PollAnswerId), InternalApi.PollingService.GetPoll(pollVote.PollId));
			CreatedDate = InternalApi.Formatting.FromUtcToUserTime(pollVote.CreatedDateUtc);
			LastUpdatedDate = InternalApi.Formatting.FromUtcToUserTime(pollVote.LastUpdatedDateUtc);
			User = new User(pollVote.UserId);
		}

		public Guid PollId { get; set; }
		public PollAnswer Answer { get; set; }
		public User User { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime LastUpdatedDate { get; set; }
	}
}
