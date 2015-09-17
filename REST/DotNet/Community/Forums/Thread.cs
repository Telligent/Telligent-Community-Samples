using System;
using System.Xml;
using System.Collections;

namespace Telligent.Community
{
	/*
	 * 
	 * This class is a light representation of the thread object.
	 * 
	 * More details here:
	 * https://community.telligent.com/developers/w/developer85/46811.forum-thread-rest-endpoints
	 * 
	 */ 

	/// <summary>
	/// The thread class is used to represent a thread from a forum discussion. 
	/// </summary>
	public class Thread
	{
		int threadId = 0;
		int threadUserId = 0;
		int replyCount = 0;
		string threadSubject = string.Empty;
		string threadBody = string.Empty;
		ArrayList postList;

		/// <summary>
		/// Gets the Id of the Thread
		/// </summary>
		/// <value>The identifier.</value>
		public int Id {
			get { return threadId; }
		}

		/// <summary>
		/// Gets the UserId of the Thread
		/// </summary>
		/// <value>The user identifier.</value>
		public int UserId {
			get { return threadUserId; }
		}

		/// <summary>
		/// Gets the subject of the thread
		/// </summary>
		/// <value>The subject.</value>
		public string Subject {
			get { return threadSubject; }
		}

		/// <summary>
		/// Gets the body of the thread
		/// </summary>
		/// <value>The body.</value>
		public string Body {
			get { return threadBody; }
		}

		/// <summary>
		/// Does the thread have replies
		/// </summary>
		/// <value><c>true</c> if this thread has replies; otherwise, <c>false</c>.</value>
		public bool HasReplies {
			get { 
				if (replyCount > 0)
					return true;
				return false;
			}
		}

		/// <summary>
		/// Returns the count of post replies of the thread
		/// </summary>
		/// <value>The reply count.</value>
		public int ReplyCount {
			get { return replyCount; }
		}

		/// <summary>
		/// Gets the posts associated with this thread
		/// </summary>
		/// <returns>The posts.</returns>
		public ArrayList GetPosts() {
			postList = Post.GetPosts (threadId);

			return postList;
		}

		/// <summary>
		/// Returns and ArrayList of Threads for a forum
		/// </summary>
		/// <returns>Arraylist of Threads</returns>
		/// <param name="threads">XmlDocument containing a list of threads for a forum</param>
		public static ArrayList ThreadsFromXml (XmlDocument threads) {

			ArrayList threadList = new ArrayList ();


			foreach (XmlNode t in threads.SelectNodes("/Response/Threads/Thread")) {
				Thread thread = ThreadFromXml (t);

				threadList.Add (thread);
			}

			return threadList;
		}

		/// <summary>
		/// Returns a thread populated from an XmlNode
		/// </summary>
		/// <returns>Thread instance</returns>
		/// <param name="thread">XmlNode representation of a forum thread</param>
		public static Thread ThreadFromXml (XmlNode thread) {

			Thread t = new Thread();

			t.threadId = int.Parse(thread["Id"].InnerText);
			t.threadSubject = thread["Subject"].InnerText;
			t.threadBody = thread["Body"].InnerText;
			t.replyCount = int.Parse(thread ["ReplyCount"].InnerText);
			t.threadUserId = int.Parse (thread["Author"].SelectSingleNode("Id").InnerText);

			return t;
		}
	}
}

