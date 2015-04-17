using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telligent.BigSocial.Polling.RestApi
{
	public class PollAnswer
	{
		public PollAnswer()
		{
		}

		internal PollAnswer(InternalApi.PollAnswer pollAnswer, InternalApi.Poll poll)
		{
			PollId = pollAnswer.PollId;
			Id = pollAnswer.Id;
			Name = pollAnswer.Name;
			VoteCount = poll.HideResultsUntilVotingComplete && poll.VotingEndDateUtc.HasValue && poll.VotingEndDateUtc > DateTime.UtcNow ? 0 : pollAnswer.VoteCount;
		}

		public Guid PollId { get; set; }
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int VoteCount { get; set; }
	}
}
