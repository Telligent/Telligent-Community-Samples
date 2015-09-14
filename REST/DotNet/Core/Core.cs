using System;
using System.Net;
using System.Text;

namespace Telligent
{
	public class Util
	{
		static Config config;

		public static Config GetConfig () {

			if (null == config) {
				config = new Config ();
				config.Load ();
			}

			return config;

		}

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

		public static void WriteSecurityToken(Config config) {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine (config.ApiKey);
			Console.ResetColor ();
		}

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

		public static void WriteUpdate (string update) {
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine ("");
			Console.WriteLine ("*** " + update + " ***");
			Console.ResetColor ();
		}

		public static void WriteLine (string text, ConsoleColor color) {
			Console.ForegroundColor = color;
			Console.WriteLine (text);
			Console.ResetColor ();
		}

		public static void Write (string text, ConsoleColor color) {
			Console.ForegroundColor = color;
			Console.Write (text);
			Console.ResetColor ();
		}

	}
}

