using System;

namespace Telligent.BigSocial.Polling.InternalApi
{
	[Serializable]
	internal class PollAnswer
	{
		internal Guid Id { get; set; }
		internal Guid PollId { get; set; }
		internal string Name { get; set; }
		internal int VoteCount { get; set; }
	}
}
