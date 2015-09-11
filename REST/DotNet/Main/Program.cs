using System;
using Telligent;
using Telligent.Community;

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
			//ForumExamples();

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
			int forumId = 1;

//			Util.WriteLine("Count
	
		}

		public static void UserExamples() {
			Util.WriteLine ("User Id for '" + username + "' is: " + User.GetUserByName(username).UserId, ConsoleColor.Red);

			Util.WriteLine ("User Id for '" + email + "' is: " + User.GetUserByEmail(email).UserId, ConsoleColor.Red);
		}
	}

}
