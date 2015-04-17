using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using System.Collections;
using Telligent.Evolution.Extensibility.Version1;

namespace Telligent.BigSocial.Polling.WidgetApi
{
	[Documentation(Category = "Polling")]
	public class PollVotes
	{
		[Documentation("Get the accessing user's vote on a poll.")]
		public PublicApi.PollVote Get(
			[Documentation("The identifier of the poll.")]
			Guid pollId
			)
		{
			return PublicApi.PollVotes.Get(pollId);
		}

		[Documentation("Delete the accessing user's vote on a poll.")]
		public AdditionalInfo Delete(
			[Documentation("The identifier of the poll.")]
			Guid pollId
			)
		{
			return PublicApi.PollVotes.Delete(pollId);
		}

		[Documentation("Update the accessing user's vote on a poll.")]
		public PublicApi.PollVote Update(
			[Documentation("The identifier of the poll.")]
			Guid pollId, 
			[Documentation("The identifier of the selected answer.")]
			Guid pollAnswerId
			)
		{
			return PublicApi.PollVotes.Update(pollId, pollAnswerId);
		}
	}
}
