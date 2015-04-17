using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Content.Version1;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.Plugins
{
	public class PollSearchCategories : IPlugin, ISearchCategories, ITranslatablePlugin
	{
		ITranslatablePluginController _translation;

		#region IPlugin Members
		
		public string Name
		{
			get { return "Poll Search Categories"; }
		}

		public string Description
		{
			get { return "Adds search-related categories to support polls being indexed."; }
		}

		public void Initialize()
		{
		}

		#endregion

		#region ISearchCategories Members

		public SearchCategory[] GetCategories()
		{
			return new SearchCategory[] { 
				new SearchCategory ("polls", _translation.GetLanguageResourceValue("search_category_name"))
			};
		}

		#endregion

		#region ITranslatablePlugin Members

		public Translation[] DefaultTranslations
		{
			get
			{
				var translation = new Translation("en-US");
				translation.Set("search_category_name", "Polls");

				return new Translation[] { translation };
			}
		}

		public void SetController(ITranslatablePluginController controller)
		{
			_translation = controller;
		}

		#endregion
	}
}
