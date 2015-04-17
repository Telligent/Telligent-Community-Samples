using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InternalEntity = Telligent.BigSocial.Polling.InternalApi.Poll;
using Telligent.Evolution.Extensibility.Api.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;
using Telligent.Evolution.Extensibility.Content.Version1;

namespace Telligent.BigSocial.Polling.PublicApi
{
	public delegate void PollBeforeCreateEventHandler(PollBeforeCreateEventArgs e);
	public delegate void PollAfterCreateEventHandler(PollAfterCreateEventArgs e);
	public delegate void PollBeforeUpdateEventHandler(PollBeforeUpdateEventArgs e);
	public delegate void PollAfterUpdateEventHandler(PollAfterUpdateEventArgs e);
	public delegate void PollBeforeDeleteEventHandler(PollBeforeDeleteEventArgs e);
	public delegate void PollAfterDeleteEventHandler(PollAfterDeleteEventArgs e);
	public delegate void PollRenderEventHandler(PollRenderEventArgs e);

	public class PollEvents : Telligent.Evolution.Extensibility.Events.Version1.EventsBase
	{
		#region Create

		private readonly object BeforeCreateEvent = new object();

		public event PollBeforeCreateEventHandler BeforeCreate
		{
			add { Add(BeforeCreateEvent, value); }
			remove { Remove(BeforeCreateEvent, value); }
		}

		internal void OnBeforeCreate(InternalEntity poll)
		{
			var handlers = Get<PollBeforeCreateEventHandler>(BeforeCreateEvent);
			if (handlers != null)
			{
				var args = new PollBeforeCreateEventArgs(poll);
				handlers(args);
			}

			TEApi.Html.Events.OnBeforeCreate(GetHtmlProperties(poll));
			TEApi.Content.Events.OnBeforeCreate(GetContent(poll));
		}

		private readonly object AfterCreateEvent = new object();

		public event PollAfterCreateEventHandler AfterCreate
		{
			add { Add(AfterCreateEvent, value); }
			remove { Remove(AfterCreateEvent, value); }
		}

		internal void OnAfterCreate(InternalEntity poll)
		{
			var handlers = Get<PollAfterCreateEventHandler>(AfterCreateEvent);
			if (handlers != null)
				handlers(new PollAfterCreateEventArgs(poll));

			TEApi.Content.Events.OnAfterCreate(GetContent(poll));
		}

		#endregion

		#region Update

		private readonly object BeforeUpdateEvent = new object();

		public event PollBeforeUpdateEventHandler BeforeUpdate
		{
			add { Add(BeforeUpdateEvent, value); }
			remove { Remove(BeforeUpdateEvent, value); }
		}

		internal void OnBeforeUpdate(InternalEntity poll)
		{
			var handlers = Get<PollBeforeUpdateEventHandler>(BeforeUpdateEvent);
			if (handlers != null)
			{
				var args = new PollBeforeUpdateEventArgs(poll);
				handlers(args);
			}

			TEApi.Html.Events.OnBeforeUpdate(GetHtmlProperties(poll));
			TEApi.Content.Events.OnBeforeUpdate(GetContent(poll));
		}

		private readonly object AfterUpdateEvent = new object();

		public event PollAfterUpdateEventHandler AfterUpdate
		{
			add { Add(AfterUpdateEvent, value); }
			remove { Remove(AfterUpdateEvent, value); }
		}

		internal void OnAfterUpdate(InternalEntity poll)
		{
			var handlers = Get<PollAfterUpdateEventHandler>(AfterUpdateEvent);
			if (handlers != null)
				handlers(new PollAfterUpdateEventArgs(poll));

			TEApi.Content.Events.OnAfterUpdate(GetContent(poll));
		}

		#endregion

		#region Delete

		private readonly object BeforeDeleteEvent = new object();

		public event PollBeforeDeleteEventHandler BeforeDelete
		{
			add { Add(BeforeDeleteEvent, value); }
			remove { Remove(BeforeDeleteEvent, value); }
		}

		internal void OnBeforeDelete(InternalEntity poll)
		{
			var handlers = Get<PollBeforeDeleteEventHandler>(BeforeDeleteEvent);
			if (handlers != null)
				handlers(new PollBeforeDeleteEventArgs(poll));

			TEApi.Content.Events.OnBeforeDelete(GetContent(poll));
		}

		private readonly object AfterDeleteEvent = new object();

		public event PollAfterDeleteEventHandler AfterDelete
		{
			add { Add(AfterDeleteEvent, value); }
			remove { Remove(AfterDeleteEvent, value); }
		}

		internal void OnAfterDelete(InternalEntity poll)
		{
			var handlers = Get<PollAfterDeleteEventHandler>(AfterDeleteEvent);
			if (handlers != null)
				handlers(new PollAfterDeleteEventArgs(poll));

			TEApi.Content.Events.OnAfterDelete(GetContent(poll));
		}

		#endregion

		#region Render

		private readonly object RenderEvent = new object();

		public event PollRenderEventHandler Render
		{
			add { Add(RenderEvent, value); }
			remove { Remove(RenderEvent, value); }
		}

		internal string OnRender(InternalEntity poll, string propertyName, string propertyHtml, string target)
		{
			var handlers = Get<PollRenderEventHandler>(RenderEvent);
			if (handlers != null)
			{
				var args = new PollRenderEventArgs(poll, propertyName, propertyHtml, target);
				handlers(args);
				propertyHtml = args.RenderedHtml;
			}

			return TEApi.Html.Events.OnRender(propertyName, propertyHtml, target);
		}

		#endregion

		private HtmlProperties GetHtmlProperties(InternalEntity internalEntity)
		{
			return new HtmlProperties()
				.Add("Description", () => internalEntity.Description, (string html) => internalEntity.Description = html, true)
				.Add("Name", () => internalEntity.Name, (string html) => internalEntity.Name = html, false);
		}

		private IContent GetContent(InternalEntity internalEntity)
		{
			return new Poll(internalEntity);
		}
	}

	public abstract class ReadOnlyUnrenderablePollEventArgsBase
	{
		internal ReadOnlyUnrenderablePollEventArgsBase(InternalEntity poll)
		{
			InternalEntity = poll;
		}

		internal InternalEntity InternalEntity { get; private set; }

		public Guid ContentId { get { return InternalEntity.Id; } }
		public Guid ContentTypeId { get { return PublicApi.Polls.ContentTypeId; } }
		public Guid Id { get { return InternalEntity.Id; } }
		public string Name { get { return InternalEntity.Name; } }
		public int AuthorUserId { get { return InternalEntity.AuthorUserId; } }
		public Guid ApplicationId { get { return InternalEntity.ApplicationId; } }
		public bool IsEnabled { get { return InternalEntity.IsEnabled; } }
		public DateTime CreatedDate { get { return InternalApi.Formatting.FromUtcToUserTime(InternalEntity.CreatedDateUtc); } }
		public DateTime LastUpdatedDate { get { return InternalApi.Formatting.FromUtcToUserTime(InternalEntity.LastUpdatedDateUtc); } }
		public DateTime? VotingEndDate { get { return InternalEntity.VotingEndDateUtc.HasValue ? (DateTime?) InternalApi.Formatting.FromUtcToUserTime(InternalEntity.VotingEndDateUtc.Value) : null; } }
		public bool HideResultsUntilVotingComplete { get { return InternalEntity.HideResultsUntilVotingComplete; } }

		List<PollAnswer> _answers = null;
		public IList<PollAnswer> Answers
		{
			get
			{
				if (_answers == null)
				{
					if (InternalEntity.Answers != null)
						_answers = new List<PollAnswer>(InternalEntity.Answers.Select(x => new PollAnswer(x, InternalEntity)));
					else
						_answers = new List<PollAnswer>();
				}

				return _answers;
			}
		}
	}

	public abstract class ReadOnlyPollEventArgsBase : ReadOnlyUnrenderablePollEventArgsBase
	{
		internal ReadOnlyPollEventArgsBase(InternalEntity poll)
			: base(poll)
		{
		}

		public string Description()
		{
			return Description("web");
		}

		public string Description(string target)
		{
			return InternalApi.PollingService.RenderPollDescription(InternalEntity, target);
		}
	}

	public abstract class EditablePollEventArgsBase
	{
		internal EditablePollEventArgsBase(InternalEntity poll)
		{
			InternalEntity = poll;
		}

		internal InternalEntity InternalEntity { get; private set; }

		public Guid ContentId { get { return InternalEntity.Id; } }
		public Guid ContentTypeId { get { return PublicApi.Polls.ContentTypeId; } }
		public Guid Id { get { return InternalEntity.Id; } }
		public string Name { get { return InternalEntity.Name; } set { InternalEntity.Name = value; } }
		public string Description { get { return InternalEntity.Description; } set { InternalEntity.Description = value; } }
		public int AuthorUserId { get { return InternalEntity.AuthorUserId; } }
        public Guid ApplicationId { get { return InternalEntity.ApplicationId; } }
        public bool IsEnabled { get { return InternalEntity.IsEnabled; } set { InternalEntity.IsEnabled = value; } }
		public DateTime CreatedDate { get { return InternalApi.Formatting.FromUtcToUserTime(InternalEntity.CreatedDateUtc); } }
		public DateTime LastUpdatedDate { get { return InternalApi.Formatting.FromUtcToUserTime(InternalEntity.LastUpdatedDateUtc); } }
		public DateTime? VotingEndDate { get { return InternalEntity.VotingEndDateUtc.HasValue ? (DateTime?) InternalApi.Formatting.FromUtcToUserTime(InternalEntity.VotingEndDateUtc.Value) : null; } set { InternalEntity.VotingEndDateUtc = value.HasValue ? (DateTime?) InternalApi.Formatting.FromUserTimeToUtc(value.Value) : null; } }
		public bool HideResultsUntilVotingComplete { get { return InternalEntity.HideResultsUntilVotingComplete; } set { InternalEntity.HideResultsUntilVotingComplete = value; } }

		List<PollAnswer> _answers = null;
		public IList<PollAnswer> Answers
		{
			get
			{
				if (_answers == null)
				{
					if (InternalEntity.Answers != null)
						_answers = new List<PollAnswer>(InternalEntity.Answers.Select(x => new PollAnswer(x, InternalEntity)));
					else
						_answers = new List<PollAnswer>();
				}

				return _answers;
			}
		}
	}

	public class PollBeforeCreateEventArgs : EditablePollEventArgsBase
    {
        internal PollBeforeCreateEventArgs(InternalEntity poll)
            : base(poll)
        {
        }
    }

    public class PollAfterCreateEventArgs : ReadOnlyPollEventArgsBase
    {
        internal PollAfterCreateEventArgs(InternalEntity poll)
            : base(poll)
        {
        }
    }

    public class PollBeforeUpdateEventArgs : EditablePollEventArgsBase
    {
        internal PollBeforeUpdateEventArgs(InternalEntity poll)
            : base(poll)
        {
        }
    }

    public class PollAfterUpdateEventArgs : ReadOnlyPollEventArgsBase
    {
        internal PollAfterUpdateEventArgs(InternalEntity poll)
            : base(poll)
        {
        }
    }

    public class PollBeforeDeleteEventArgs : ReadOnlyPollEventArgsBase
    {
        internal PollBeforeDeleteEventArgs(InternalEntity poll)
            : base(poll)
        {
        }
    }

    public class PollAfterDeleteEventArgs : ReadOnlyPollEventArgsBase
    {
        internal PollAfterDeleteEventArgs(InternalEntity poll)
            : base(poll)
        {
        }
    }

    public class PollRenderEventArgs : ReadOnlyUnrenderablePollEventArgsBase
    {
        internal PollRenderEventArgs(InternalEntity poll, string renderedProperty, string renderedHtml, string target)
            : base(poll)
        {
            RenderedHtml = renderedHtml;
            RenderedProperty = renderedProperty;
            RenderTarget = target;
        }

        public string RenderedProperty { get; private set; }
        public string RenderedHtml { get; set; }
        public string RenderTarget { get; private set; }
    }
}
