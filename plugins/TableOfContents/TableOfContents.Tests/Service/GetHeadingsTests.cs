using NUnit.Framework;
using System.Linq;
using Zimbra.Community.Extensions.TableOfContents.Tests.Mocks;

namespace Zimbra.Community.Extensions.TableOfContents.Tests.Service
{
	[TestFixture]
	public class GetHeadingsTests
	{
		private ITableOfContentsService _tableOfContentsService;

		[TestFixtureSetUp]
		public void SetUp()
		{
			_tableOfContentsService = new TableOfContentsService(new DummyHtmlStripper());
		}

		[Test]
		public void Test_Heading_AnchorName_Persisted()
		{
			var headings = _tableOfContentsService.GetHeadings("<h4><a name=\"h4-anchor\"></a>Heading Contents</h4>");
			Assert.AreEqual(1, headings.Count());

			Assert.AreEqual("h4-anchor", headings.First().AnchorName);
		}
		[Test]
		public void Test_Heading_HeadingType_Persisted()
		{
			var headings = _tableOfContentsService.GetHeadings("<h4><a name=\"h4-anchor\"></a>Heading Contents</h4>");
			Assert.AreEqual(1, headings.Count());

			Assert.AreEqual(HeadingType.H4, headings.First().HeadingType);
		}

		[Test]
		public void Test_Heading_Contents_Persisted()
		{
			var headings = _tableOfContentsService.GetHeadings("<h4><a name=\"h4-anchor\"></a>Heading Contents</h4>");
			Assert.AreEqual(1, headings.Count());

			Assert.AreEqual("Heading Contents", headings.First().Title);
		}

		[Test]
		public void Test_Finds_Heading_With_Anchor_At_Begining()
		{
			var headings = _tableOfContentsService.GetHeadings("<h2><a name=\"Anchor-Name\"></a>Heading Text</h2>");
			Assert.AreEqual(1, headings.Count());
			var heading = headings.First();
			Assert.AreEqual("Anchor-Name", heading.AnchorName);
			Assert.AreEqual("Heading Text", heading.Title);
			Assert.AreEqual(HeadingType.H2, heading.HeadingType);
		}

		[Test]
		public void Test_Finds_Heading_With_Anchor_At_End()
		{
			var headings = _tableOfContentsService.GetHeadings("<h3>Some Text<a name=\"EndAnchor\"></a></h3>");
			Assert.AreEqual(1, headings.Count());
			var heading = headings.First();
			Assert.AreEqual("EndAnchor", heading.AnchorName);
			Assert.AreEqual("Some Text", heading.Title);
			Assert.AreEqual(HeadingType.H3, heading.HeadingType);
		}

		[Test]
		public void Test_Finds_Heading_With_Anchor_In_Middle()
		{
			var headings = _tableOfContentsService.GetHeadings("<h4>Split <a name=\"Anchor\"></a>Heading</h4>");
			Assert.AreEqual(1, headings.Count());
			var heading = headings.First();
			Assert.AreEqual("Anchor", heading.AnchorName);
			Assert.AreEqual("Split Heading", heading.Title);
			Assert.AreEqual(HeadingType.H4, heading.HeadingType);
		}

		[Test]
		public void Test_Finds_H1()
		{
			GetHeadingsFindsHeading(HeadingType.H1);
		}

		[Test]
		public void Test_Finds_H2()
		{
			GetHeadingsFindsHeading(HeadingType.H2);
		}

		[Test]
		public void Test_Finds_H3()
		{
			GetHeadingsFindsHeading(HeadingType.H3);
		}

		[Test]
		public void Test_Finds_H4()
		{
			GetHeadingsFindsHeading(HeadingType.H4);
		}

		[Test]
		public void Test_Finds_H5()
		{
			GetHeadingsFindsHeading(HeadingType.H5);
		}

		[Test]
		public void Test_Finds_H6()
		{
			GetHeadingsFindsHeading(HeadingType.H6);
		}

		[Test]
		public void Test_Does_Not_Find_Mismatched_Heading()
		{
			var headings = _tableOfContentsService.GetHeadings("<h1><a name=\"test\"></a>Here Is A Mismatched Heading</h4>");
			Assert.AreEqual(0, headings.Count());
		}

		private void GetHeadingsFindsHeading(HeadingType headingType)
		{
			const string html = @"
				<h1><a name=""h1""></a>Heading1</h1>
				<p>Heading 1 contents</p>
				<h2><a name=""h2""></a>Heading2</h2>
				<p>Heading 2 contents</p>
				<h3><a name=""h3""></a>Heading3</h3>
				<p>Heading 3 contents</p>
				<h4><a name=""h4""></a>Heading4</h4>
				<p>Heading 4 contents</p>
				<h5><a name=""h5""></a>Heading5</h5>
				<p>Heading 5 contents</p>
				<h6><a name=""h6""></a>Heading6</h6>
				<p>Heading 6 contents</p>
			";

			var headings = _tableOfContentsService.GetHeadings(html);

			Assert.AreEqual(1, headings.Where(x => x.HeadingType == headingType).Count(), "More than one heading of type found");
		}

	}
}
