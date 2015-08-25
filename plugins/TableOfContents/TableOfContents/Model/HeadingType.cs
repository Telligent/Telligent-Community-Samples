
namespace Zimbra.Community.Extensions.TableOfContents
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification="A heading must be of exactly one type.  A none option doesn't make sense")]
	public enum HeadingType
	{
		H1 = 1,
		H2 = 2,
		H3 = 3,
		H4 = 4,
		H5 = 5,
		H6 = 6
	}
}
