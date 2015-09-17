using System;
using System.Xml;
using System.Collections;
using System.Net;

namespace Telligent.Community
{

	/// <summary>
	/// The post class is used to represent a post from a forum discussion. 
	/// </summary>
	public class Post
	{
		int postId = 0;
		string subject = string.Empty;
		string body = string.Empty;

		/// <summary>
		/// The Id of the post
		/// </summary>
		/// <value>The identifier.</value>
		public int Id {
			get { return postId; }
		}

		/// <summary>
		/// The body of the post
		/// </summary>
		/// <value>The body.</value>
		public string Body {
			get { return body; }
		}

		/// <summary>
		/// The subject of the post
		/// </summary>
		/// <value>The subject.</value>
		public string Subject {
			get { return subject; }
		}

		/// <summary>
		/// Returns a list of posts associated with a given thread
		/// </summary>
		/// <returns>An ArrayList of post instances</returns>
		/// <param name="threadId">Thread identifier.</param>
		public static ArrayList GetPosts (int threadId) {
			ArrayList list = new ArrayList ();
			Post p = null;
			string restEndPoint = "forums/threads/{0}/replies.xml";

			// Build URL
			Config c = Util.GetConfig();
			string url = c.ApiUrl + string.Format(restEndPoint, threadId.ToString());
			XmlDocument doc = new XmlDocument ();

			// Get the first set of data to determine how many pages exist
			using (WebClient webClient = Util.GetWebClient ()) {
				doc.LoadXml (webClient.DownloadString (url));
			}

			XmlNodeList replies = doc.GetElementsByTagName("Reply");

			foreach (XmlNode reply in replies) {

				p = new Post ();

				p.postId = int.Parse(reply.SelectSingleNode("Id").InnerText);
				p.subject = reply.SelectSingleNode("Subject").InnerText;
				p.body = reply.SelectSingleNode("Body").InnerText;

				list.Add (p);
			}

			return list;
		}

	}
}