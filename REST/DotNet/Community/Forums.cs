using System;
using System.Xml;
using System.Net;

namespace Telligent.Community
{
	public class Forums
	{
		/// <summary>
		/// Returns an XmlNodeList containing threads in the requested forum
		/// </summary>
		/// <returns>Forum threads.</returns>
		/// <param name="forumId">Forum to fetch threads for</param>
		public static XmlNodeList GetForumThreads (int forumId)
		{
			string restEndPoint = "forums/{0}/threads/active.xml?PageSize={1}";

			// Build URL
			Config c = Util.GetConfig();
			string url = c.ApiUrl + string.Format(restEndPoint, forumId.ToString(), Util.GetConfig().PageSize);
			XmlDocument doc = new XmlDocument ();

			// Get the first set of data to determine how many pages exist
			using (WebClient webClient = Util.GetWebClient ()) {
				doc.LoadXml (webClient.DownloadString (url));
			}

			return doc.GetElementsByTagName ("Thread");
		}
	}
}