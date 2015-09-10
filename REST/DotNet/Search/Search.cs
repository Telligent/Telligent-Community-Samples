using System;
using System.Xml;
using System.Net;
using Telligent;

namespace Telligent.Search
{
	public class Search
	{
		int pageSize = 25;

		/// <summary>
		/// Simple search example that returns the count of a given search term
		/// </summary>
		/// <returns>The keyword.</returns>
		/// <param name="keyword">Keyword.</param>
		public static int ByKeyword (string keyword) {
			string restEndPoint = "search.xml?query" +
				"=" + keyword;
			int totalCount = 0;
			int totalPages = 0;

			// Build URL
			Config c = Util.GetConfig();
			string search = c.ApiUrl + restEndPoint;
			XmlDocument doc = new XmlDocument ();

			// Get the first set of data to determine how many pages exist
			using (WebClient webClient = Util.GetWebClient ()) {

				doc.LoadXml (webClient.DownloadString (search));
			}

			// Get the total count of results
			totalCount = int.Parse (doc.SelectSingleNode ("/Response/SearchResults").Attributes ["TotalCount"].InnerText);

			return totalCount;
		}


		public int GetPageCount (XmlDocument doc) {
			int totalPages = 0;
			int totalCount = 0;

			// Get the total count of results
			totalCount = int.Parse (doc.SelectSingleNode ("/Response/SearchResults").Attributes ["TotalCount"].InnerText);

			// Calculate how many pages of data we have
			if (totalCount < pageSize)
				totalPages = 1;
			else
				totalPages = totalCount / pageSize;

			return totalPages;
		}

		/*
		public void Clean(string word) {
			string searchRestEndPoint = "search.xml?query={0}+AND+(type:user)&pagesize={1}&pageindex={2}";
			int totalCount;
			int totalPages;
			XmlDocument doc = new XmlDocument ();
			string search = config.ApiUrl + string.Format (searchRestEndPoint, word, 1, 0) + "&startdate=" + config.SearchStart + " &enddate=" + config.SearchEnd;

			// Get the first set of data to determine how many pages exist
			using (WebClient webClient = Util.GetWebClient (config)) {
				doc.LoadXml (webClient.DownloadString (search));
			}

			// Get the total count of results
			totalCount = int.Parse (doc.SelectSingleNode ("/Response/SearchResults").Attributes ["TotalCount"].InnerText);

			if (totalCount > 0)
				Util.WriteUpdate ("Found " + totalCount + " results for " + word);

			// Calculate how many pages of data we have
			if (totalCount < pageSize)
				totalPages = 1;
			else
				totalPages = totalCount / pageSize;

			// Now loop through all the pages
			for (int i = 0; i < totalPages; i++) {
				SetSpamUsersToDelete (i, word);

				if (UserDeleteList.Count > 0)
					DeleteUsers ();

			}
		}	
		*/
	}
}
