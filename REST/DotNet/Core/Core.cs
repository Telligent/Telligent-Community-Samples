using System;
using System.Net;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace Telligent
{
	public class Util
	{
		static Config config;

		/// <summary>
		/// Loads an instance of Config from the Rules.config xml file
		/// </summary>
		/// <returns>The config.</returns>
		public static Config GetConfig () {

			if (null == config) {
				config = new Config ();
				config.Load ();
			}

			return config;

		}

		/// <summary>
		/// Strips the HTML from a string
		/// </summary>
		/// <returns>The clean html</returns>
		/// <param name="html">The html to clean</param>
		static string StripHtml (string html) {

			// decode html
			html = System.Web.HttpUtility.HtmlDecode(html);

			// string anything else
			return Regex.Replace(html, @"<[^>]*>", String.Empty);
		}

		/// <summary>
		/// Returns an instance of a WebClient with the Rest-User-Token header
		/// </summary>
		/// <returns>The web client.</returns>
		public static WebClient GetWebClient() {

			// Ensure config is loaded
			GetConfig ();

			// new WebClient
			WebClient webClient = new WebClient ();

			// Set the header
			webClient.Headers.Add("Rest-User-Token", config.ApiKey);

			return webClient;
		}

		/// <summary>
		/// Function that sets up a WebClient with the proper headers for delete
		/// </summary>
		/// <returns>The web client.</returns>
		public static WebClient DeleteWebClient() {
			WebClient webClient = GetWebClient ();

			// Set the header
			webClient.Headers.Add("Rest-Method", "DELETE");

			return webClient;
		}

		/// <summary>
		/// Helper function to output the security token
		/// </summary>
		/// <param name="config">Config.</param>
		public static void WriteSecurityToken(Config config) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine (config.ApiKey);
			Console.ResetColor ();
		}

		/// <summary>
		/// Helper function to output an error message
		/// </summary>
		/// <param name="msg">Message.</param>
		/// <param name="extra">Extra.</param>
		public static void WriteError (string msg, string extra) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine ("");
			Console.WriteLine ("!!! WARNING !!!");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine ("--------------------------------------");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine (msg);
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine (extra);
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine ("--------------------------------------");
			Console.ResetColor ();
		}

		/// <summary>
		/// Helper function to Write a line to the Console with a certain color and line terminator
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="color">Color.</param>
		public static void WriteLine (string text, ConsoleColor color) {
			Console.ForegroundColor = color;
			Console.WriteLine (text);
			Console.ResetColor ();
		}

		/// <summary>
		/// Helper function to Write a line to the Console with a certain color
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="color">Color.</param>
		public static void Write (string text, ConsoleColor color) {
			Console.ForegroundColor = color;
			Console.Write (text);
			Console.ResetColor ();
		}

	}
}

