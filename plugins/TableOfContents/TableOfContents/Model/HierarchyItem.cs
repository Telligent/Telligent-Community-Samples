
namespace Zimbra.Community.Extensions.TableOfContents
{
	public class HierarchyItem<T>
	{
		public HierarchyItem(T item)
		{
			Item = item;
			Children = new HierarchyCollection<Heading>();
		}

		public T Item { get; private set; }
		public HierarchyCollection<Heading> Children { get; private set; }
	}


}
