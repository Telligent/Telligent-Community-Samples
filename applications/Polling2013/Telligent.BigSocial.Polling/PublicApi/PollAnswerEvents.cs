using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InternalEntity = Telligent.BigSocial.Polling.InternalApi.PollAnswer;
using Telligent.Evolution.Extensibility.Api.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;
using Telligent.Evolution.Extensibility.Content.Version1;

namespace Telligent.BigSocial.Polling.PublicApi
{
	public delegate void PollAnswerBeforeCreateEventHandler(PollAnswerBeforeCreateEventArgs e);
	public delegate void PollAnswerAfterCreateEventHandler(PollAnswerAfterCreateEventArgs e);
	public delegate void PollAnswerBeforeUpdateEventHandler(PollAnswerBeforeUpdateEventArgs e);
	public delegate void PollAnswerAfterUpdateEventHandler(PollAnswerAfterUpdateEventArgs e);
	public delegate void PollAnswerBeforeDeleteEventHandler(PollAnswerBeforeDeleteEventArgs e);
	public delegate void PollAnswerAfterDeleteEventHandler(PollAnswerAfterDeleteEventArgs e);

	public class PollAnswerEvents : Telligent.Evolution.Extensibility.Events.Version1.EventsBase
	{
		#region Create

		private readonly object BeforeCreateEvent = new object();

		public event PollAnswerBeforeCreateEventHandler BeforeCreate
		{
			add { Add(BeforeCreateEvent, value); }
			remove { Remove(BeforeCreateEvent, value); }
		}

		internal void OnBeforeCreate(InternalEntity pollAnswer)
		{
			var handlers = Get<PollAnswerBeforeCreateEventHandler>(BeforeCreateEvent);
			if (handlers != null)
			{
				var args = new PollAnswerBeforeCreateEventArgs(pollAnswer);
				handlers(args);
			}

			TEApi.Html.Events.OnBeforeCreate(GetHtmlProperties(pollAnswer));
		}

		private readonly object AfterCreateEvent = new object();

		public event PollAnswerAfterCreateEventHandler AfterCreate
		{
			add { Add(AfterCreateEvent, value); }
			remove { Remove(AfterCreateEvent, value); }
		}

		internal void OnAfterCreate(InternalEntity pollAnswer)
		{
			var handlers = Get<PollAnswerAfterCreateEventHandler>(AfterCreateEvent);
			if (handlers != null)
				handlers(new PollAnswerAfterCreateEventArgs(pollAnswer));
		}

		#endregion

		#region Update

		private readonly object BeforeUpdateEvent = new object();

		public event PollAnswerBeforeUpdateEventHandler BeforeUpdate
		{
			add { Add(BeforeUpdateEvent, value); }
			remove { Remove(BeforeUpdateEvent, value); }
		}

		internal void OnBeforeUpdate(InternalEntity pollAnswer)
		{
			var handlers = Get<PollAnswerBeforeUpdateEventHandler>(BeforeUpdateEvent);
			if (handlers != null)
			{
				var args = new PollAnswerBeforeUpdateEventArgs(pollAnswer);
				handlers(args);
			}

			TEApi.Html.Events.OnBeforeUpdate(GetHtmlProperties(pollAnswer));
		}

		private readonly object AfterUpdateEvent = new object();

		public event PollAnswerAfterUpdateEventHandler AfterUpdate
		{
			add { Add(AfterUpdateEvent, value); }
			remove { Remove(AfterUpdateEvent, value); }
		}

		internal void OnAfterUpdate(InternalEntity pollAnswer)
		{
			var handlers = Get<PollAnswerAfterUpdateEventHandler>(AfterUpdateEvent);
			if (handlers != null)
				handlers(new PollAnswerAfterUpdateEventArgs(pollAnswer));
		}

		#endregion

		#region Delete

		private readonly object BeforeDeleteEvent = new object();

		public event PollAnswerBeforeDeleteEventHandler BeforeDelete
		{
			add { Add(BeforeDeleteEvent, value); }
			remove { Remove(BeforeDeleteEvent, value); }
		}

		internal void OnBeforeDelete(InternalEntity pollAnswer)
		{
			var handlers = Get<PollAnswerBeforeDeleteEventHandler>(BeforeDeleteEvent);
			if (handlers != null)
				handlers(new PollAnswerBeforeDeleteEventArgs(pollAnswer));
		}

		private readonly object AfterDeleteEvent = new object();

		public event PollAnswerAfterDeleteEventHandler AfterDelete
		{
			add { Add(AfterDeleteEvent, value); }
			remove { Remove(AfterDeleteEvent, value); }
		}

		internal void OnAfterDelete(InternalEntity pollAnswer)
		{
			var handlers = Get<PollAnswerAfterDeleteEventHandler>(AfterDeleteEvent);
			if (handlers != null)
				handlers(new PollAnswerAfterDeleteEventArgs(pollAnswer));
		}

		#endregion

		private HtmlProperties GetHtmlProperties(InternalEntity internalEntity)
		{
			return new HtmlProperties()
				.Add("Name", () => internalEntity.Name, (string html) => internalEntity.Name = html, false);
		}
	}


	public abstract class ReadOnlyPollAnswerEventArgsBase
	{
		internal ReadOnlyPollAnswerEventArgsBase(InternalEntity pollAnswer)
		{
			InternalEntity = pollAnswer;
		}

		internal InternalEntity InternalEntity { get; private set; }

		public Guid PollId { get { return InternalEntity.PollId; } }
		public Guid Id { get { return InternalEntity.Id; } }
		public string Name { get { return InternalEntity.Name; } }
		public int VoteCount { get { return InternalEntity.VoteCount; } }
	}

	public abstract class EditablePollAnswerEventArgsBase
	{
		internal EditablePollAnswerEventArgsBase(InternalEntity pollAnswer)
		{
			InternalEntity = pollAnswer;
		}

		internal InternalEntity InternalEntity { get; private set; }

		public Guid PollId { get { return InternalEntity.PollId; } }
		public Guid Id { get { return InternalEntity.Id; } }
		public string Name { get { return InternalEntity.Name; } set { InternalEntity.Name = value; } }
		public int VoteCount { get { return InternalEntity.VoteCount; } }
	}

	public class PollAnswerBeforeCreateEventArgs : EditablePollAnswerEventArgsBase
    {
        internal PollAnswerBeforeCreateEventArgs(InternalEntity pollAnswer)
            : base(pollAnswer)
        {
        }
    }

    public class PollAnswerAfterCreateEventArgs : ReadOnlyPollAnswerEventArgsBase
    {
        internal PollAnswerAfterCreateEventArgs(InternalEntity pollAnswer)
            : base(pollAnswer)
        {
        }
    }

    public class PollAnswerBeforeUpdateEventArgs : EditablePollAnswerEventArgsBase
    {
        internal PollAnswerBeforeUpdateEventArgs(InternalEntity pollAnswer)
            : base(pollAnswer)
        {
        }
    }

    public class PollAnswerAfterUpdateEventArgs : ReadOnlyPollAnswerEventArgsBase
    {
        internal PollAnswerAfterUpdateEventArgs(InternalEntity pollAnswer)
            : base(pollAnswer)
        {
        }
    }

    public class PollAnswerBeforeDeleteEventArgs : ReadOnlyPollAnswerEventArgsBase
    {
        internal PollAnswerBeforeDeleteEventArgs(InternalEntity pollAnswer)
            : base(pollAnswer)
        {
        }
    }

    public class PollAnswerAfterDeleteEventArgs : ReadOnlyPollAnswerEventArgsBase
    {
        internal PollAnswerAfterDeleteEventArgs(InternalEntity pollAnswer)
            : base(pollAnswer)
        {
        }
    }
}
