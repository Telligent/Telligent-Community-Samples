using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.Scoring.Version1;

namespace Telligent.BigSocial.Polling.Plugins
{
	public class PollVotesMetric : IPlugin, IMetric
	{
		private readonly Guid _metricId = new Guid("3bbd666589a34b9787f1d10f614d32b4");
		IMetricController _metricController;

		#region IPlugin Members

		public string Name
		{
			get { return "Poll Votes Metric"; }
		}

		public string Description
		{
			get { return "Enables scores to use voting information from polls."; }
		}

		public void Initialize()
		{
			PublicApi.PollVotes.Events.AfterCreate += new PublicApi.PollVoteAfterCreateEventHandler(Events_AfterCreate);
			PublicApi.PollVotes.Events.AfterUpdate += new PublicApi.PollVoteAfterUpdateEventHandler(Events_AfterUpdate);
			PublicApi.PollVotes.Events.AfterDelete += new PublicApi.PollVoteAfterDeleteEventHandler(Events_AfterDelete);
		}

		void Events_AfterDelete(PublicApi.PollVoteAfterDeleteEventArgs e)
		{
			_metricController.QueueForCalculation(e.PollId, PublicApi.Polls.ContentTypeId);
		}

		void Events_AfterUpdate(PublicApi.PollVoteAfterUpdateEventArgs e)
		{
			_metricController.QueueForCalculation(e.PollId, PublicApi.Polls.ContentTypeId);
		}

		void Events_AfterCreate(PublicApi.PollVoteAfterCreateEventArgs e)
		{
			_metricController.QueueForCalculation(e.PollId, PublicApi.Polls.ContentTypeId);
		}

		#endregion

		#region IMetric Members

		public double Calculate(Guid contentId, Guid contentTypeId)
		{
			var poll = InternalApi.PollingService.GetPoll(contentId);
			var count = poll != null ? poll.Answers.Sum(x => x.VoteCount) : 0;

			return Math.Min(1.0, count / 10.0);
		}

		public Guid Id
		{
			get { return _metricId; }
		}

		public string MetricDescription
		{
			get { return "Increases with the number of votes made on a poll."; }
		}

		public string MetricName
		{
			get { return "Poll Votes"; }
		}

		public void SetController(IMetricController controller)
		{
			_metricController = controller;
		}

		#endregion
	}
}
