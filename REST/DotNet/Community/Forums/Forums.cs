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
		bool isValid = false;
		string forumName = string.Empty;
		string forumUrl = string.Empty;

		/// <summary>
		/// Gets the forum threads
		/// </summary>
		/// <value>The threads in a given forum</value>
		public ArrayList Threads {
			get { return threads; }
		}

		/// <summary>
		/// Gets the forum Id
		/// </summary>
		/// <value>The forum id</value>
		public int Id {
			get { return forumId; }
		}


		/// <summary>
		/// Gets the forum Name
		/// </summary>
		/// <value>The forum name</value>
		public string Name {
			get { return forumName; }
		}

		/// <summary>
		/// Gets the forum URL
		/// </summary>
		/// <value>The forum URL</value>
		public string Url {
			get { return forumUrl; }
		}

		/// <summary>
		/// Populates a forum instance based on the XML returned from an API call
		/// </summary>
		/// <returns>An instance of a forum</returns>
		/// <param name="user">XML representation of Forum</param>
		public static Forum ForumFromXml (XmlDocument forum) {
			Forum f = new Forum ();

			f.forumId = int.Parse (forum.SelectSingleNode ("/Response/Forum/Id").InnerText);
			f.forumName = forum.SelectSingleNode ("/Response/Forum/Name").InnerText;
			f.forumUrl = forum.SelectSingleNode ("/Response/Forum/Url").InnerText;
			f.isValid = true;

			return f;
		}

		/// <summary>
		/// Looks up a forum by id
		/// </summary>
		/// <returns>A forum instance</returns>
		/// <param name="forumId">The id of the forum to look up</param>
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
		public ArrayList GetActiveForumThreads ()
		{
			// Check if we have a valid forum to lookup
			if (!isValid)
				return null;
			
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