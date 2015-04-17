using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.RestApi
{
	public class Poll
	{
		public Poll()
		{
		}

		internal Poll(InternalApi.Poll poll)
		{
			Id = poll.Id;
			ContentId = poll.Id;
			Name = poll.Name;
			Description = InternalApi.PollingService.RenderPollDescription(poll, "webservices");
			IsEnabled = poll.IsEnabled;
			CreatedDate = InternalApi.Formatting.FromUtcToUserTime(poll.CreatedDateUtc);
			LastUpdatedDate = InternalApi.Formatting.FromUtcToUserTime(poll.LastUpdatedDateUtc);
            Url = TEApi.Url.Absolute(InternalApi.PollingUrlService.PollUrl(poll.Id));
			Answers = new List<PollAnswer>(poll.Answers.Select(x => new PollAnswer(x, poll)));
			Group = new Group(poll.ApplicationId);
			AuthorUser = new User(poll.AuthorUserId);
			HideResultsUntilVotingComplete = poll.HideResultsUntilVotingComplete;
			VotingEndDate = !poll.VotingEndDateUtc.HasValue ? null : (DateTime?) InternalApi.Formatting.FromUtcToUserTime(poll.VotingEndDateUtc.Value);
			TotalVotes = poll.Answers.Sum(x => x.VoteCount);
		}

		public Guid Id { get; set; }
		public Guid ContentId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public User AuthorUser { get; set; }
		public Group Group { get; set; }
		public bool IsEnabled { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime LastUpdatedDate { get; set; }
		public string Url { get; set; }
		public List<PollAnswer> Answers { get; set; }
		public bool HideResultsUntilVotingComplete { get; set; }
		public DateTime? VotingEndDate { get; set; }
		public int TotalVotes { get; set; }
	}
}
