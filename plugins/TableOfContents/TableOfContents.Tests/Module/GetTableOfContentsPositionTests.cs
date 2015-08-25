using NUnit.Framework;

namespace Zimbra.Community.Extensions.TableOfContents.Tests.Module
{
	[TestFixture]
	public class GetTableOfContentsPositionTests
	{

		[Test]
		public void Test_Returns_Negative1_If_Input_Does_Not_Contain_TOC_Tag()
		{
			var input = "<p>Content</p>";
			Assert.AreEqual(-1, TableOfContentsPlugin.GetTableOfContentsPosition(ref input));
		}

		[Test]
		public void Test_Returns_Index_Of_TOC_Tag()
		{
			var input = "<p>Content[toc]</p>";
			Assert.AreEqual(10, TableOfContentsPlugin.GetTableOfContentsPosition(ref input));
		}

		[Test]
		public void Test_Removes_TOC_Tag_From_Input()
		{
			var input = "[toc]";
			TableOfContentsPlugin.GetTableOfContentsPosition(ref input);
			StringAssert.DoesNotContain("[toc]", input);
		}

		[Test]
		public void Test_Removes_TOC_Tag_And_Surrounding_Paragraph_From_Input_When_Paragraph_Only_Contains_TOC_Tag()
		{
			var input = "<p>[toc]</p>";
			TableOfContentsPlugin.GetTableOfContentsPosition(ref input);
			StringAssert.DoesNotContain("[toc]", input);
			StringAssert.DoesNotContain("<p></p>", input);
		}


	}
}
