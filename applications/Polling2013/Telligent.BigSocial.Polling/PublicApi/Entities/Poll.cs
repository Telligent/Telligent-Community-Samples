using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Internal = Telligent.BigSocial.Polling.InternalApi;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using Telligent.Evolution.Extensibility.Api.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;
using Telligent.Evolution.Extensibility.Content.Version1;

namespace Telligent.BigSocial.Polling.PublicApi
{
	public class Poll : ApiEntity, IContent
	{
		Internal.Poll _poll = null;

		public Poll()
			: base()
		{
		}

		public Poll(AdditionalInfo additionalInfo)
			: base(additionalInfo)
		{
		}

		public Poll(IList<Warning> warnings, IList<Error> errors)
			: base(warnings, errors)
		{
		}

		internal Poll(Internal.Poll poll)
			: base()
		{
			_poll = poll;
		}

		public Guid Id
		{
			get { return _poll == null ? Guid.Empty : _poll.Id; }
		}

		public Guid ContentId
		{
			get { return Id; }
		}

		public string Name
		{
			get { return _poll == null ? string.Empty : _poll.Name; }
		}

		public string Description()
		{
			return Description("web");
		}

		public string Description(string target)
		{
			if (_poll != null)
				return InternalApi.PollingService.RenderPollDescription(_poll, target);
			else
				return string.Empty;
		}

		User _author;
		public User Author
		{
			get
			{
				if (_author == null && _poll != null)
					_author = TEApi.Users.Get(new UsersGetOptions() { Id = _poll.AuthorUserId });

				return _author;
			}
		}

		Group _group;
		public Group Group
		{
			get
			{
				if (_group == null && _poll != null)
					_group = TEApi.Groups.Get(_poll.ApplicationId);

				return _group;
			}
		}

		public bool IsEnabled
		{
			get { return _poll == null ? false : _poll.IsEnabled; }
		}

		public DateTime CreatedDate
		{
			get { return _poll == null ? DateTime.Now : InternalApi.Formatting.FromUtcToUserTime(_poll.CreatedDateUtc); }
		}

		public DateTime LastUpdatedDate
		{
			get { return _poll == null ? DateTime.Now : InternalApi.Formatting.FromUtcToUserTime(_poll.LastUpdatedDateUtc); }
		}

		public string Url
		{
            get { return _poll == null ? null : InternalApi.PollingUrlService.PollUrl(_poll.Id); }
		}

		public bool HideResultsUntilVotingComplete
		{
			get { return _poll == null ? false : _poll.HideResultsUntilVotingComplete; }
		}

		public DateTime? VotingEndDate
		{
			get { return _poll == null || !_poll.VotingEndDateUtc.HasValue ? null : (DateTime?) InternalApi.Formatting.FromUtcToUserTime(_poll.VotingEndDateUtc.Value); }
		}

		public int TotalVotes
		{
			get { return _poll == null ? 0 : _poll.Answers.Sum(x => x.VoteCount); }
		}

		ApiList<PollAnswer> _answers;
		public ApiList<PollAnswer> Answers
		{
			get
			{
				if (_answers == null && _poll != null && _poll.Answers != null)
					_answers = new ApiList<PollAnswer>(_poll.Answers.Select(x => new PollAnswer(x, _poll)));

				return _answers;
			}
		}

		#region IContent Members

		IApplication IContent.Application
		{
			get { return Group; }
		}

		string IContent.AvatarUrl
		{
			get { return null; }
		}

		Guid IContent.ContentTypeId
		{
			get { return Polls.ContentTypeId; }
		}

		int? IContent.CreatedByUserId
		{
			get { return _poll == null ? (int?) null : _poll.AuthorUserId; }
		}

		string IContent.HtmlDescription(string target)
		{
			return Description(target);
		}

		string IContent.HtmlName(string target)
		{
			return Name;
		}		

		#endregion
	}
}
