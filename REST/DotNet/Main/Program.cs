using System;
using Telligent;
using Telligent.Search;

namespace Main
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// Search examples
			SearchExamples();

			// Forum examples
			ForumExamples();
			
		}

		public static void SearchExamples () {
			string searchPhrase = "test";
			string searchUser = "rhoward";

			// Get a count of search results for the search phrase
			Util.WriteLine ("Search count for search term '" + searchPhrase + "': " + Search.ByKeyword (searchPhrase).ToString(), ConsoleColor.Red);

			// Get a count of user results for the search phrase
			Util.WriteLine ("User count for search term '" + searchPhrase + "': " + Search.ForUsers (searchPhrase).ToString(), ConsoleColor.Red);

			// Does the user exist?
			Util.WriteLine ("Does user '" + searchUser + "' exist: " + Search.ForUserByName (searchUser).ToString(), ConsoleColor.Red);
		}

		public static void ForumExamples() {
			int forumId = 1;

//			Util.WriteLine("Count
	
		}
	}

}
