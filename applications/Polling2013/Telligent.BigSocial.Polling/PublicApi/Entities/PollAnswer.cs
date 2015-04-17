using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Internal = Telligent.BigSocial.Polling.InternalApi;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;

namespace Telligent.BigSocial.Polling.PublicApi
{
	public class PollAnswer : ApiEntity
	{
		Internal.PollAnswer _pollAnswer = null;
		bool _hideVoteCounts = false;

		public PollAnswer()
			: base()
		{
		}

		public PollAnswer(AdditionalInfo additionalInfo)
			: base(additionalInfo)
		{
		}

		public PollAnswer(IList<Warning> warnings, IList<Error> errors)
			: base(warnings, errors)
		{
		}

		internal PollAnswer(Internal.PollAnswer pollAnswer, InternalApi.Poll poll)
			: base()
		{
			_pollAnswer = pollAnswer;
			_hideVoteCounts = poll.HideResultsUntilVotingComplete && poll.VotingEndDateUtc.HasValue && DateTime.UtcNow > poll.VotingEndDateUtc.Value;
		}

		Poll _poll;
		public Poll Poll
		{
			get
			{
				if (_poll == null && _pollAnswer != null)
					_poll = Polls.Get(_pollAnswer.PollId);

				return _poll;
			}
		}

		public Guid Id
		{
			get { return _pollAnswer == null ? Guid.Empty : _pollAnswer.Id; }
		}

		public string Name
		{
			get { return _pollAnswer == null ? string.Empty : _pollAnswer.Name; }
		}

		public int VoteCount
		{
			get { return _pollAnswer == null || _hideVoteCounts ? 0 : _pollAnswer.VoteCount; }
		}
	}
}
