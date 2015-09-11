using System;
using System.Net;
using System.Xml;

namespace Telligent.Community
{
	public class User
	{
		int userId = 0;
		string username = "";
		public enum UserStatus {Approved, Banned, Disapproved, NotSet};
		UserStatus status = UserStatus.NotSet;

		public int UserId {
			get { return userId; }
		}

		public string Username {
			get { return username; }
		}

		public UserStatus AccountStatus {
			get { return status; }
		}

		/// <summary>
		/// Populates a user instance based on the XML returned from an API call
		/// </summary>
		/// <returns>An instance of a user</returns>
		/// <param name="user">XML representation of User</param>
		public static User UserFromXml (XmlDocument user) {
			User u = new User ();

			u.UserId = int.Parse (user.SelectSingleNode ("/Response/User/Id").InnerText);
			u.Username = user.SelectSingleNode ("/Response/User/Username").InnerText;

			string accountstatus = user.SelectSingleNode ("/Response/User/AccountStatus").InnerText;

			switch (accountstatus) {
				case "Approved":
					u.AccountStatus = UserStatus.Approved;
					break;
				case "Banned":
					u.AccountStatus = UserStatus.Banned;
					break;
				case "Disapproved":
					u.AccountStatus = UserStatus.Disapproved;
					break;
			}

			return u;
		}

		/// <summary>
		/// Finds a user by username
		/// </summary>
		/// <returns>User instance</returns>
		/// <param name="username">Username used to look up user</param>
		public static User GetUserByName (string username) {
			string restEndPoint = "users/{0}.xml";
			User u;

			// Build URL
			Config c = Util.GetConfig();
			string url = c.ApiUrl + string.Format(restEndPoint, username);
			XmlDocument doc = new XmlDocument ();

			// Get the first set of data to determine how many page	s exist
			using (WebClient webClient = Util.GetWebClient ()) {
					
				doc.LoadXml (webClient.DownloadString (url));
			}

			// Return the Id of the user
			try {
				u = UserFromXml(doc);
			} catch {
				throw new Exception (string.Format("Unable to find ID for user {0}", username));
			}

			return u;
		}

		/// <summary>
		/// Finds a user by email address
		/// </summary>
		/// <returns>User instance</returns>
		/// <param name="email">Email used to look up user</param>
		public static User GetUserByEmail (string email) {
			string restEndPoint = "users.xml?EmailAddress={0}";
			User u;

			// Build URL
			Config c = Util.GetConfig();
			string url = c.ApiUrl + string.Format(restEndPoint, email);
			XmlDocument doc = new XmlDocument ();

			// Get the first set of data to determine how many page	s exist
			using (WebClient webClient = Util.GetWebClient ()) {

				doc.LoadXml (webClient.DownloadString (url));
			}

			// Return the Id of the user
			try {
				u = UserFromXml(doc);
			} catch {
				throw new Exception (string.Format("Unable to find data for user {0}", email));
			}

			return u;
		}

	}
}

