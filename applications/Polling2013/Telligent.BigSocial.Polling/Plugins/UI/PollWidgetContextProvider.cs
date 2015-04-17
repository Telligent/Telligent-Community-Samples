using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.UI.Version1;

namespace Telligent.BigSocial.Polling.Plugins
{
	public class PollWidgetContextProvider : IPlugin, IScriptedContentFragmentContextProvider
	{
		private readonly Guid PollContextId = new Guid("d80d27d4e20240009de310ec6e513785");

		#region IPlugin Members

		public string Name
		{
			get { return "Poll Widget Context Provider."; }
		}

		public string Description
		{
			get { return "Enables Studio Widgets to depend on poll-related contexts."; }
		}

		public void Initialize()
		{
		}

		#endregion

		#region IScriptedContentFragmentContextProvider Members

		public IEnumerable<ContextItem> GetSupportedContextItems()
		{
			return new ContextItem[] {
				new ContextItem("Poll", PollContextId)
			};
		}

		public bool HasContextItem(System.Web.UI.Page page, Guid contextItemId)
		{
			if (PollContextId != contextItemId)
				return false;

            var pollIdObject = page.RouteData.Values["PollId"];
            if (pollIdObject == null)
                return false;

			Guid pollId;
            if (!Guid.TryParse(pollIdObject.ToString(), out pollId))
				return false;

			return InternalApi.PollingService.GetPoll(pollId) != null;
		}

		#endregion
	}
}
