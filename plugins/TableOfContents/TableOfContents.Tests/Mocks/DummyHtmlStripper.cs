using System;
using System.Text.RegularExpressions;

namespace Zimbra.Community.Extensions.TableOfContents.Tests.Mocks
{
	public class DummyHtmlStripper : IHtmlStripper
	{
		private static readonly Regex HtmlTagRegex = new Regex("<[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public string RemoveHtml(string html)
		{
			return HtmlTagRegex.Replace(html, String.Empty);
		}
	}
}
