using System;
using System.Xml;
using System.Net;
using System.Collections;

namespace Telligent.Community
{
	public class Forum
	{
		ArrayList threads = new ArrayList();
		int forumId = 0;
		string forumName = string.Empty;
		string forumUrl = string.Empty;

		public ArrayList Threads {
			get { return threads; }
		}

		public int Id {
			get { return forumId; }
		}


		public string Name {
			get { return forumName; }
		}

		public string Url {
			get { return forumUrl; }
		}


		public static Forum ForumFromXml (XmlDocument forum) {
			Forum f = new Forum ();

			f.forumId = int.Parse (forum.SelectSingleNode ("/Response/Forum/Id").InnerText);
			f.forumName = forum.SelectSingleNode ("/Response/Forum/Name").InnerText;
			f.forumUrl = forum.SelectSingleNode ("/Response/Forum/Url").InnerText;

			return f;
		}

		public static Forum GetForum (int forumId) {
			Forum f;
			string restEndPoint = "forums/{0}.xml";

			// Build URL
			Config c = Util.GetConfig();
			string url = c.ApiUrl + string.Format(restEndPoint, forumId.ToString(), Util.GetConfig().PageSize);
			XmlDocument doc = new XmlDocument ();

			// Get the first set of data to determine how many pages exist
			using (WebClient webClient = Util.GetWebClient ()) {
				doc.LoadXml (webClient.DownloadString (url));
			}

			try {
				f = ForumFromXml(doc);
			} catch {
				throw new Exception (string.Format("Unable to find forum for id {0}", forumId));
			}

			return f;
		}


		/// <summary>
		/// Returns an XmlNodeList containing threads in the requested forum
		/// </summary>
		/// <returns>Forum threads.</returns>
		public ArrayList GetForumThreads ()
		{
			// Check if we have a valid forum

			string restEndPoint = "forums/{0}/threads/active.xml";

			// Build URL
			Config c = Util.GetConfig();
			string url = c.ApiUrl + string.Format(restEndPoint, forumId.ToString());
			XmlDocument doc = new XmlDocument ();

			// Get the first set of data to determine how many pages exist
			using (WebClient webClient = Util.GetWebClient ()) {
				doc.LoadXml (webClient.DownloadString (url));
			}

			try {
				threads = Thread.ThreadsFromXml(doc);

			} catch {

				throw new Exception (string.Format ("Unable to load threads for forum '{0}'", forumName));
			}

			return threads;
		}
	}
}