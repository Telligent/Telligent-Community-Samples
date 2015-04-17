using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.UI.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.Plugins
{
	public class PollHeaderExtension : IPlugin, ITranslatablePlugin, IHtmlHeaderExtension, IHttpCallback
	{
		ITranslatablePluginController _translation;
		IHttpCallbackController _callbackController;
		DateTime? _lastModifiedDateUtc = null;

		#region IPlugin Members
		
		public string Name
		{
			get { return "Poll Header Extension"; }
		}

		public string Description
		{
			get { return "Registers scripts to support rendering polls."; }
		}

		public void Initialize()
		{
		}

		#endregion

		#region ITranslatablePlugin Members

		public Translation[] DefaultTranslations
		{
			get 
			{
				var translation = new Translation("en-US");
				translation.Set("poll_vote_delete", "Delete your vote.");
				translation.Set("poll_vote", "You voted for '{AnswerName}' {Date}.");
				translation.Set("poll_voting_ends", "Voting ends {Date}.");
				translation.Set("poll_voting_ends_results_hidden", "Voting ends {Date}. Results will be shown at that time.");
				translation.Set("poll_voting_ended", "Voting ended {Date}.");

				return new Translation[] { translation };
			}
		}

		public void SetController(ITranslatablePluginController controller)
		{
			_translation = controller;
		}

		#endregion

		#region IHttpCallback Members

		public void ProcessRequest(System.Web.HttpContextBase httpContext)
		{
			string file;
			string contentType;
			if (httpContext.Request.QueryString["file"] == "js")
			{
				file = "JavaScript.ui-poll.js";
				contentType = "text/javascript";
			}
			else if (httpContext.Request.QueryString["file"] == "css")
			{
				file = "Css.poll.css";
				contentType = "text/css";
			}
			else
			{
				httpContext.Response.StatusCode = 404;
				return;
			}

			if (_lastModifiedDateUtc == null)
			{
				_lastModifiedDateUtc = System.IO.File.GetLastWriteTimeUtc(this.GetType().Assembly.Location);
				if (_lastModifiedDateUtc.Value > DateTime.UtcNow)
					_lastModifiedDateUtc = DateTime.UtcNow;
			}

			httpContext.Response.Cache.SetAllowResponseInBrowserHistory(true);
			httpContext.Response.Cache.SetLastModified(_lastModifiedDateUtc.Value);
			httpContext.Response.Cache.SetETag(_lastModifiedDateUtc.Value.Ticks.ToString());
			httpContext.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
			httpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(7));
			httpContext.Response.Cache.SetValidUntilExpires(false);
			httpContext.Response.ContentType = contentType;
			httpContext.Response.Write(InternalApi.EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources." + file));
		}

		public void SetController(IHttpCallbackController controller)
		{
			_callbackController = controller;
		}

		#endregion

		#region IHtmlHeaderExtension Members

		public string GetHeader(RenderTarget target)
		{
			var jsQuery = new NameValueCollection();
			jsQuery["file"] = "js";

			var cssQuery = new NameValueCollection();
			cssQuery["file"] = "css";

			return string.Format(@"
<script src=""{0}""></script>
<link rel=""stylesheet"" href=""{1}"" type=""text/css"" />
<script type=""text/javascript"">
(function($){{
	$.telligent.evolution.ui.components.poll.configure({{deleteMessage:'{2}',voteMessage:'{3}',votingEndsMessage:'{4}',votingEndsResultsHiddenMessage:'{5}',votingEndedMessage:'{6}'}});
}}(jQuery));
</script>
",
				_callbackController.GetUrl(jsQuery),
				_callbackController.GetUrl(cssQuery),
				TEApi.Javascript.Encode(_translation.GetLanguageResourceValue("poll_vote_delete")),
				TEApi.Javascript.Encode(_translation.GetLanguageResourceValue("poll_vote")),
				TEApi.Javascript.Encode(_translation.GetLanguageResourceValue("poll_voting_ends")),
				TEApi.Javascript.Encode(_translation.GetLanguageResourceValue("poll_voting_ends_results_hidden")),
				TEApi.Javascript.Encode(_translation.GetLanguageResourceValue("poll_voting_ended"))
				);
		}

		public bool IsCacheable
		{
			get { return true; }
		}

		public bool VaryCacheByUser
		{
			get { return true; }
		}

		#endregion
	}
}
