using System;

namespace Telligent.BigSocial.Polling.InternalApi
{
	[Serializable]
	internal class PollVote
	{
		internal Guid PollId { get; set; }
		internal int UserId { get; set; }
		internal Guid PollAnswerId { get; set; }
		internal DateTime CreatedDateUtc { get; set; }
		internal DateTime LastUpdatedDateUtc { get; set; }
	}
}
