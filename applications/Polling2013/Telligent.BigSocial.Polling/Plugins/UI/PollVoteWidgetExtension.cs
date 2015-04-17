using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.UI.Version1;

namespace Telligent.BigSocial.Polling.Plugins
{
	public class PollVoteWidgetExtension : IPlugin, IScriptedContentFragmentExtension
	{
		#region IPlugin Members

		public string Name
		{
			get { return "Poll Vote Widget Extension (telligent_v1_pollVote)"; }
		}

		public string Description
		{
			get { return "Exposes poll votes to Studio Widgets."; }
		}

		public void Initialize()
		{
		}

		#endregion

		#region IExtension Members

		public object Extension
		{
			get { return new WidgetApi.PollVotes(); }
		}

		public string ExtensionName
		{
			get { return "telligent_v1_pollVote"; }
		}

		#endregion
	}
}
