using NUnit.Framework;
using System.Linq;
using Zimbra.Community.Extensions.TableOfContents.Tests.Mocks;

namespace Zimbra.Community.Extensions.TableOfContents.Tests.Service
{
	[TestFixture]
	public class GetHeadingHierarchyTests
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
			var hierarchy = _tableOfContentsService.GetHeadingHierarchy("<h4><a name=\"h4-anchor\"></a>Heading Contents</h4>");
			Assert.AreEqual(1, hierarchy.Count);

			Assert.AreEqual("h4-anchor", hierarchy.First().Item.AnchorName);
		}
		[Test]
		public void Test_Heading_HeadingType_Persisted()
		{
			var hierarchy = _tableOfContentsService.GetHeadingHierarchy("<h4><a name=\"h4-anchor\"></a>Heading Contents</h4>");
			Assert.AreEqual(1, hierarchy.Count);

			Assert.AreEqual(HeadingType.H4, hierarchy.First().Item.HeadingType);
		}

		[Test]
		public void Test_Heading_Contents_Persisted()
		{
			var hierarchy = _tableOfContentsService.GetHeadingHierarchy("<h4><a name=\"h4-anchor\"></a>Heading Contents</h4>");
			Assert.AreEqual(1, hierarchy.Count);

			Assert.AreEqual("Heading Contents", hierarchy.First().Item.Title);
		}


		[Test]
		public void Test_Lesser_Headings_AreChildren_Of_Higher_Headings()
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

			var hiearachy = _tableOfContentsService.GetHeadingHierarchy(html);

			Assert.AreEqual(1, hiearachy.Count);
			var secondLevel = hiearachy.First().Children;
			Assert.AreEqual(1, secondLevel.Count);
			var thirdLevel = secondLevel.First();
			Assert.AreEqual(1, thirdLevel.Children.Count);
			var fourthLevel = thirdLevel.Children.First();
			Assert.AreEqual(1, fourthLevel.Children.Count);
			var fifthLevel = fourthLevel.Children.First();
			Assert.AreEqual(1, fifthLevel.Children.Count);
			var sixthLevel = fifthLevel.Children.First();
			Assert.AreEqual(1, sixthLevel.Children.Count);
		}

		[Test]
		public void Test_Multiple_Lower_Headings_Of_Same_Type_Are_Siblings()
		{
			const string html = @"<h2><a name=""h2""></a>Heading2</h2>
<h3><a name=""h3-1""></a>Heading 3 - 1</h3>
<h3><a name=""h3-1""></a>Heading 3 - 1</h3>";

			var hierarchy = _tableOfContentsService.GetHeadingHierarchy(html);
			Assert.AreEqual(1, hierarchy.Count);
			var secondLevelChildren = hierarchy.First().Children;
			Assert.AreEqual(2, secondLevelChildren.Count);
		}

		[Test]
		public void Test_Much_Lower_Headings_Are_Children_Of_Much_Higher_Heading()
		{
			const string html = @"<h1><a name=""h1""></a>Heading 6</h1>
<h6><a name=""h6-1""></a>Heading 6 - 1</h6>
<h6><a name=""h6-1""></a>Heading 6 - 1</h6>";

			var hierarchy = _tableOfContentsService.GetHeadingHierarchy(html);
			Assert.AreEqual(1, hierarchy.Count);
			var secondLevelChildren = hierarchy.First().Children;
			Assert.AreEqual(2, secondLevelChildren.Count);
		}

		[Test]
		public void Test_Initial_Heading_Is_Top_Level_Even_With_Subsequent_Higher_Headings()
		{
			const string html = @"<h6><a name=""h6""></a>Heading6</h6>
<h1><a name=""h1""></a>Heading 2</h1>";

			var hierarchy = _tableOfContentsService.GetHeadingHierarchy(html);
			Assert.AreEqual(2, hierarchy.Count);
		}

	}
}
