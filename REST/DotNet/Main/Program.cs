using System;
using Telligent;
using Telligent.Search;

namespace Main
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			int count = Search.ByKeyword ("test");

			Util.Write ("Search count for 'test': " + count.ToString (), ConsoleColor.Red);
			
		}
	}
}
