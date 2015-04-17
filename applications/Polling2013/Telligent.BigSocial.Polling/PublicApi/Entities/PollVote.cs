using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Internal = Telligent.BigSocial.Polling.InternalApi;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using Telligent.Evolution.Extensibility.Api.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.PublicApi
{
	public class PollVote : ApiEntity
	{
		Internal.PollVote _pollVote = null;

		public PollVote()
			: base()
		{
		}

		public PollVote(AdditionalInfo additionalInfo)
			: base(additionalInfo)
		{
		}

		public PollVote(IList<Warning> warnings, IList<Error> errors)
			: base(warnings, errors)
		{
		}

		internal PollVote(Internal.PollVote pollVote)
			: base()
		{
			_pollVote = pollVote;
		}

		Poll _poll;
		public Poll Poll
		{
			get 
			{ 
				if (_poll == null && _pollVote != null)
					_poll = Polls.Get(_pollVote.PollId);

				return _poll;
			}
		}

		PollAnswer _answer;
		public PollAnswer Answer
		{
			get
			{
				if(_answer == null && _pollVote != null)
					_answer = PollAnswers.Get(_pollVote.PollAnswerId);

				return _answer;
			}
		}

		User _user;
		public User User
		{
			get
			{
				if (_user == null && _pollVote != null)
					_user = TEApi.Users.Get(new UsersGetOptions() { Id = _pollVote.UserId });

				return _user;
			}
		}

		public DateTime CreatedDate
		{
			get { return _pollVote == null ? DateTime.Now : InternalApi.Formatting.FromUtcToUserTime(_pollVote.CreatedDateUtc); }
		}

		public DateTime LastUpdatedDate
		{
			get { return _pollVote == null ? DateTime.Now : InternalApi.Formatting.FromUtcToUserTime(_pollVote.LastUpdatedDateUtc); }
		}
	}
}
