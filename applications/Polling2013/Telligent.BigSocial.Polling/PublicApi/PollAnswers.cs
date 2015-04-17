using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using Internal = Telligent.BigSocial.Polling.InternalApi;

namespace Telligent.BigSocial.Polling.PublicApi
{
	public static class PollAnswers
	{
		private static readonly PollAnswerEvents _events = new PollAnswerEvents();

		public static PollAnswerEvents Events
		{
			get { return _events; }
		}

		public static PollAnswer Get(Guid id)
		{
			try
			{
				var answer = Internal.PollingService.GetPollAnswer(id);
				if (answer == null)
					return null;

				var poll = Internal.PollingService.GetPoll(answer.PollId);
				if (poll == null)
					return null;

				return new PollAnswer(answer, poll);
			}
			catch (Exception ex)
			{
				return new PollAnswer(new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message)));
			}
		}

		public static AdditionalInfo Delete(Guid id)
		{
			try
			{
				var pollAnswer = Internal.PollingService.GetPollAnswer(id);
				if (pollAnswer != null)
					Internal.PollingService.DeletePollAnswer(pollAnswer);

				return new AdditionalInfo();
			}
			catch (Exception ex)
			{
				return new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message));
			}
		}

		public static PollAnswer Create(Guid pollId, string name)
		{
			try
			{
				var pollAnswer = new Internal.PollAnswer();
				pollAnswer.PollId = pollId;
				pollAnswer.Name = name;

				Internal.PollingService.AddUpdatePollAnswer(pollAnswer);

				return Get(pollAnswer.Id);
			}
			catch (Exception ex)
			{
				return new PollAnswer(new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message)));
			}
		}

		public static PollAnswer Update(Guid id, string name = null)
		{
			try
			{
				var pollAnswer = Internal.PollingService.GetPollAnswer(id);
				if (pollAnswer != null)
				{
					if (name != null)
						pollAnswer.Name = name;

					Internal.PollingService.AddUpdatePollAnswer(pollAnswer);
				}

				return Get(id);
			}
			catch (Exception ex)
			{
				return new PollAnswer(new AdditionalInfo(new Error(ex.GetType().FullName, ex.Message)));
			}
		}
	}
}
