
namespace Zimbra.Community.Extensions.TableOfContents
{
	public interface ITableOfContentsBuilder
	{
		string BuildTableOfContents(HierarchyCollection<Heading> headings);
	}
}
