using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.UI.Version1;

namespace Telligent.BigSocial.Polling.Plugins
{
	public class PollFactoryDefaultWidgetProvider : IPlugin, IScriptedContentFragmentFactoryDefaultProvider
	{
		public readonly Guid _id = new Guid("f6b3da55235b468ba88f399735f826a3");

		#region IPlugin Members

		public string Name
		{
			get { return "Poll Factory Default Widget Provider"; }
		}

		public string Description
		{
			get { return "Defines the default widget set for polls."; }
		}

		public void Initialize()
		{
		}

		#endregion

		#region IScriptedContentFragmentFactoryDefaultProvider Members

		public Guid ScriptedContentFragmentFactoryDefaultIdentifier
		{
			get { return _id; }
		}

		#endregion
	}
}
