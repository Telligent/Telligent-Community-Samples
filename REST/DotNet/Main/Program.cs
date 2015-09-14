using System;
using Telligent;
using Telligent.Community;
using System.Collections;

namespace Main
{
	class MainClass
	{
		static string username = "rhoward";
		static string email = "rob@telligent.com";

		public static void Main (string[] args)
		{
			// Search examples
			//SearchExamples();

			// Forum examples
			ForumExamples();

			// User examples
			UserExamples();
			
		}

		public static void SearchExamples () {
			string searchPhrase = "test";


			// Get a count of search results for the search phrase
			Util.WriteLine ("Search count for search term '" + searchPhrase + "': " + Search.ByKeyword (searchPhrase).ToString(), ConsoleColor.Red);

			// Get a count of user results for the search phrase
			Util.WriteLine ("User count for search term '" + searchPhrase + "': " + Search.ForUsers (searchPhrase).ToString(), ConsoleColor.Red);

			// Does the user exist?
			Util.WriteLine ("Does user '" + username + "' exist: " + Search.ForUserByName (username).ToString(), ConsoleColor.Red);
		}

		public static void ForumExamples() {
			int forumId = 554;

			Forum forum = Forum.GetForum (forumId);
			Util.WriteLine ("Displaying threads for forum '" + forum.Name + "':", ConsoleColor.Cyan);
			ArrayList threads = forum.GetForumThreads ();

			foreach (Thread t in threads) {
				Util.WriteLine (t.Subject, ConsoleColor.Red);
			}
	
		}

		public static void UserExamples() {
			User u = null;

			try {
				u = User.GetUserByName(username);
				Util.WriteLine ("User Id for '" + username + "' is: " + u.UserId, ConsoleColor.Red);
				Util.WriteLine ("Email address for '" + username + "' is: " + u.Email, ConsoleColor.Red);
				Util.WriteLine ("User '" + username + "' is an approved community user: " + u.IsApprovedUser, ConsoleColor.Red);

			} catch {
				Util.WriteLine ("Cannot find community user for username: '" + username + "'", ConsoleColor.Red);
			}

			try {
				u = User.GetUserByEmail(email);
				Util.WriteLine ("User Id for '" + email + "' is: " + u.UserId, ConsoleColor.Red);
			} catch {
				Util.WriteLine ("Cannot find community user for email: '" + email + "'", ConsoleColor.Red);
			}
				
		}
	}

}
