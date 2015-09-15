using System;
using System.Xml;
using System.Collections;

namespace Telligent.Community
{
	/*
	 * 
	 * This class is not a complete representation of the thread object. 
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
		string threadSubject = string.Empty;
		string threadBody = string.Empty;

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
			t.threadUserId = int.Parse (thread["Author"].SelectSingleNode("Id").InnerText);

			return t;
		}
	}
}

