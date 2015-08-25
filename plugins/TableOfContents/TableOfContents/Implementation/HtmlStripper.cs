using Telligent.Evolution.Extensibility.Api.Version1;
namespace Zimbra.Community.Extensions.TableOfContents
{
	public class HtmlStripper : IHtmlStripper
	{
		public string RemoveHtml(string html)
		{
			return PublicApi.Language.RemoveHtml(html);
		}
	}
}
