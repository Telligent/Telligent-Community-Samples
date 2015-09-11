using System;
using System.Xml;
using System.Net;
using Telligent;

namespace Telligent.Community
{
	/// <summary>
	/// Internal functions are designed to be standalone.
	/// https://community.telligent.com/developers/w/developer85/46965.search-result-rest-endpoints
	/// </summary>
	public class Search
	{
		/// <summary>
		/// Simple search example that returns the count of a given search term
		/// </summary>
		/// <returns>Count of results found</returns>
		/// <param name="term">Search term to find results for</param>
		public static int ByKeyword (string term) {
			string restEndPoint = "search.xml?query={0}";
			int totalCount = 0;

			// Build URL
			Config c = Util.GetConfig();
			string url = c.ApiUrl + string.Format(restEndPoint, term);
			XmlDocument doc = new XmlDocument ();

			// Get the first set of data to determine how many pages exist
			using (WebClient webClient = Util.GetWebClient ()) {

				doc.LoadXml (webClient.DownloadString (url));
			}

			// Get the total count of results
			totalCount = int.Parse (doc.SelectSingleNode ("/Response/SearchResults").Attributes ["TotalCount"].InnerText);

			return totalCount;
		}

		/// <summary>
		/// Searches for users by search term, e.g. content that user created
		/// </summary>
		/// <returns>Count of results found</returns>
		/// <param name="term">Search term to find results for</param>
		public static int ForUsers (string term) {
			string restEndPoint = "search.xml?query={0}+AND+(type:user)";

			int totalCount = 0;

			// Build URL
			Config c = Util.GetConfig();
			string url = c.ApiUrl + string.Format(restEndPoint, term);
			XmlDocument doc = new XmlDocument ();

			// Get the first set of data to determine how many pages exist
			using (WebClient webClient = Util.GetWebClient ()) {

				doc.LoadXml (webClient.DownloadString (url));
			}

			// Get the total count of results
			totalCount = int.Parse (doc.SelectSingleNode ("/Response/SearchResults").Attributes ["TotalCount"].InnerText);

			return totalCount;

		}

		/// <summary>
		/// Searches for a single user by username
		/// </summary>
		/// <returns>Count of results found</returns>
		/// <param name="term">Search term to find results for</param>
		public static int ForUserByName (string username) {
			string restEndPoint = "search.xml?query=username:{0}+AND+(type:user)";

			int totalCount = 0;

			// Build URL
			Config c = Util.GetConfig();
			string url = c.ApiUrl + string.Format(restEndPoint, username);
			XmlDocument doc = new XmlDocument ();

			// Get the first set of data to determine how many pages exist
			using (WebClient webClient = Util.GetWebClient ()) {

				doc.LoadXml (webClient.DownloadString (url));
			}

			// Get the total count of results
			totalCount = int.Parse (doc.SelectSingleNode ("/Response/SearchResults").Attributes ["TotalCount"].InnerText);

			return totalCount;

		}

		/// <summary>
		/// Gets the page count for a returned search
		/// </summary>
		/// <returns>The page count.</returns>
		/// <param name="doc">Document.</param>
		static int GetPageCount (XmlDocument doc) {
			int totalPages = 0;
			int totalCount = 0;

			// Get the total count of results
			totalCount = int.Parse (doc.SelectSingleNode ("/Response/SearchResults").Attributes ["TotalCount"].InnerText);

			// Calculate how many pages of data we have
			if (totalCount < Util.GetConfig().PageSize)
				totalPages = 1;
			else
				totalPages = totalCount / Util.GetConfig().PageSize;

			return totalPages;
		}
	}
}
