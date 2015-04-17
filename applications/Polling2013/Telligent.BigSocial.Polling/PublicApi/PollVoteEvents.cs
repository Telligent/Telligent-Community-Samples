using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InternalEntity = Telligent.BigSocial.Polling.InternalApi.PollVote;
using Telligent.Evolution.Extensibility.Api.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;
using Telligent.Evolution.Extensibility.Content.Version1;

namespace Telligent.BigSocial.Polling.PublicApi
{
	public delegate void PollVoteBeforeCreateEventHandler(PollVoteBeforeCreateEventArgs e);
	public delegate void PollVoteAfterCreateEventHandler(PollVoteAfterCreateEventArgs e);
	public delegate void PollVoteBeforeUpdateEventHandler(PollVoteBeforeUpdateEventArgs e);
	public delegate void PollVoteAfterUpdateEventHandler(PollVoteAfterUpdateEventArgs e);
	public delegate void PollVoteBeforeDeleteEventHandler(PollVoteBeforeDeleteEventArgs e);
	public delegate void PollVoteAfterDeleteEventHandler(PollVoteAfterDeleteEventArgs e);

	public class PollVoteEvents : Telligent.Evolution.Extensibility.Events.Version1.EventsBase
	{
		#region Create

		private readonly object BeforeCreateEvent = new object();

		public event PollVoteBeforeCreateEventHandler BeforeCreate
		{
			add { Add(BeforeCreateEvent, value); }
			remove { Remove(BeforeCreateEvent, value); }
		}

		internal void OnBeforeCreate(InternalEntity pollVote)
		{
			var handlers = Get<PollVoteBeforeCreateEventHandler>(BeforeCreateEvent);
			if (handlers != null)
			{
				var args = new PollVoteBeforeCreateEventArgs(pollVote);
				handlers(args);
			}
		}

		private readonly object AfterCreateEvent = new object();

		public event PollVoteAfterCreateEventHandler AfterCreate
		{
			add { Add(AfterCreateEvent, value); }
			remove { Remove(AfterCreateEvent, value); }
		}

		internal void OnAfterCreate(InternalEntity pollVote)
		{
			var handlers = Get<PollVoteAfterCreateEventHandler>(AfterCreateEvent);
			if (handlers != null)
				handlers(new PollVoteAfterCreateEventArgs(pollVote));
		}

		#endregion

		#region Update

		private readonly object BeforeUpdateEvent = new object();

		public event PollVoteBeforeUpdateEventHandler BeforeUpdate
		{
			add { Add(BeforeUpdateEvent, value); }
			remove { Remove(BeforeUpdateEvent, value); }
		}

		internal void OnBeforeUpdate(InternalEntity pollVote)
		{
			var handlers = Get<PollVoteBeforeUpdateEventHandler>(BeforeUpdateEvent);
			if (handlers != null)
			{
				var args = new PollVoteBeforeUpdateEventArgs(pollVote);
				handlers(args);
			}
		}

		private readonly object AfterUpdateEvent = new object();

		public event PollVoteAfterUpdateEventHandler AfterUpdate
		{
			add { Add(AfterUpdateEvent, value); }
			remove { Remove(AfterUpdateEvent, value); }
		}

		internal void OnAfterUpdate(InternalEntity pollVote)
		{
			var handlers = Get<PollVoteAfterUpdateEventHandler>(AfterUpdateEvent);
			if (handlers != null)
				handlers(new PollVoteAfterUpdateEventArgs(pollVote));
		}

		#endregion

		#region Delete

		private readonly object BeforeDeleteEvent = new object();

		public event PollVoteBeforeDeleteEventHandler BeforeDelete
		{
			add { Add(BeforeDeleteEvent, value); }
			remove { Remove(BeforeDeleteEvent, value); }
		}

		internal void OnBeforeDelete(InternalEntity pollVote)
		{
			var handlers = Get<PollVoteBeforeDeleteEventHandler>(BeforeDeleteEvent);
			if (handlers != null)
				handlers(new PollVoteBeforeDeleteEventArgs(pollVote));
		}

		private readonly object AfterDeleteEvent = new object();

		public event PollVoteAfterDeleteEventHandler AfterDelete
		{
			add { Add(AfterDeleteEvent, value); }
			remove { Remove(AfterDeleteEvent, value); }
		}

		internal void OnAfterDelete(InternalEntity pollVote)
		{
			var handlers = Get<PollVoteAfterDeleteEventHandler>(AfterDeleteEvent);
			if (handlers != null)
				handlers(new PollVoteAfterDeleteEventArgs(pollVote));
		}

		#endregion
	}


	public abstract class ReadOnlyPollVoteEventArgsBase
	{
		internal ReadOnlyPollVoteEventArgsBase(InternalEntity pollVote)
		{
			InternalEntity = pollVote;
		}

		internal InternalEntity InternalEntity { get; private set; }

		public Guid PollId { get { return InternalEntity.PollId; } }
		public Guid PollAnswerId { get { return InternalEntity.PollAnswerId; } }
		public int UserId { get { return InternalEntity.UserId; } }
		public DateTime CreatedDate { get { return InternalApi.Formatting.FromUtcToUserTime(InternalEntity.CreatedDateUtc); } }
		public DateTime LastUpdatedDate { get { return InternalApi.Formatting.FromUtcToUserTime(InternalEntity.LastUpdatedDateUtc); } }
	}

	public class PollVoteBeforeCreateEventArgs : ReadOnlyPollVoteEventArgsBase
    {
        internal PollVoteBeforeCreateEventArgs(InternalEntity pollVote)
            : base(pollVote)
        {
        }
    }

    public class PollVoteAfterCreateEventArgs : ReadOnlyPollVoteEventArgsBase
    {
        internal PollVoteAfterCreateEventArgs(InternalEntity pollVote)
            : base(pollVote)
        {
        }
    }

	public class PollVoteBeforeUpdateEventArgs : ReadOnlyPollVoteEventArgsBase
    {
        internal PollVoteBeforeUpdateEventArgs(InternalEntity pollVote)
            : base(pollVote)
        {
        }
    }

    public class PollVoteAfterUpdateEventArgs : ReadOnlyPollVoteEventArgsBase
    {
        internal PollVoteAfterUpdateEventArgs(InternalEntity pollVote)
            : base(pollVote)
        {
        }
    }

    public class PollVoteBeforeDeleteEventArgs : ReadOnlyPollVoteEventArgsBase
    {
        internal PollVoteBeforeDeleteEventArgs(InternalEntity pollVote)
            : base(pollVote)
        {
        }
    }

    public class PollVoteAfterDeleteEventArgs : ReadOnlyPollVoteEventArgsBase
    {
        internal PollVoteAfterDeleteEventArgs(InternalEntity pollVote)
            : base(pollVote)
        {
        }
    }
}
