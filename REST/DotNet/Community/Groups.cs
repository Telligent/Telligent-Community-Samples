using System;
using System.Xml;
using System.Net;

namespace Telligent.Community
{
	public class Groups
	{
		/// <summary>
		/// Looks up a group by id and returns the Xml representation
		/// </summary>
		/// <param name="groupId">Group id to lookup</param>
		public static XmlNodeList GetGroup (int groupId) {
			string restEndPoint = "groups/{0}.xml";

			// Build URL
			Config c = Util.GetConfig();
			string url = c.ApiUrl + string.Format(restEndPoint, groupId.ToString());
			XmlDocument doc = new XmlDocument ();

			// Get the first set of data to determine how many pages exist
			using (WebClient webClient = Util.GetWebClient ()) {
				doc.LoadXml (webClient.DownloadString (url));
			}

			return doc.GetElementsByTagName ("Group");
		}
	}
}

