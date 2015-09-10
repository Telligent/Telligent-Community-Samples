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
			
		}

		public static void SearchExamples () {
			string searchPhrase = "test";

			// Get a count of search results for the search phrase
			Util.WriteLine ("Search count for search term '" + searchPhrase + "': " + Search.ByKeyword (searchPhrase).ToString(), ConsoleColor.Red);

			// Get a count of user results for the search phrase
			Util.WriteLine ("User count for search term '" + searchPhrase + "': " + Search.ForUsers (searchPhrase).ToString(), ConsoleColor.Red);

		}
	}

}
