using System;
using System.Collections.Generic;

namespace Telligent.BigSocial.Polling.InternalApi
{
	[Serializable]
	internal class Poll
	{
		internal Guid Id { get; set; }
		internal string Name { get; set; }
		internal string Description { get; set; }
		internal int AuthorUserId { get; set; }
		internal Guid ApplicationId { get; set; }
		internal bool IsEnabled { get; set; }
		internal DateTime CreatedDateUtc { get; set; }
		internal DateTime LastUpdatedDateUtc { get; set; }
		internal DateTime? VotingEndDateUtc { get; set; }
		internal bool HideResultsUntilVotingComplete { get; set; }
        internal bool NewPollEmailSent { get; set; }
        internal bool IsSuspectedAbusive { get; set; }

		internal List<PollAnswer> Answers { get; set; }
	}
}
