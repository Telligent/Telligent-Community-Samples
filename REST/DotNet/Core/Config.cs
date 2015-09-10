using System;
using System.Xml;
using System.Collections;
using System.Text;

namespace Telligent
{
	/*
	 * This is the class used to read the Rules.config XML file
	 */
	public class Config
	{
		XmlDocument config;
		string username;
		string apikey;
		string url;
		int setSize = 100;
		bool log;
		bool verbose;
		string apiKey = null;

		public void Load() {

			// Load the configuration
			config = new XmlDocument ();
			config.Load (@"..\..\Rules.config"); 

			// Get the url and logging info
			url = config.SelectSingleNode("config").Attributes["apiUrl"].InnerText;
			log = bool.Parse(config.SelectSingleNode("config").Attributes["log"].InnerText);
			verbose = bool.Parse(config.SelectSingleNode("config").Attributes["verbose"].InnerText);
			setSize = int.Parse(config.SelectSingleNode("config").Attributes["setSize"].InnerText);

			// Get the username and password
			username = config.SelectSingleNode ("/config/api/username").InnerText;
			apikey = config.SelectSingleNode ("/config/api/apikey").InnerText;

			// Get the api key information
			apiKey = String.Format("{0}:{1}", apikey, username);
			apiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(apiKey));
		}

		public string ApiUrl {
			get {
				if (url.EndsWith ("/"))
					return url;
				else {
					url = url + "/";
					return url;
				}
			}
		}

		public int SetSize {
			get { return setSize; }
		}

		public bool Log {
			get { return log; }
		}

		public bool Verbose {
			get { return verbose; }
		}

		public string ApiKey {
			get {
				return apiKey;
			}
		}

	}
}

