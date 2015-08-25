using NUnit.Framework;
using System;
using Zimbra.Community.Extensions.TableOfContents.Tests.Mocks;

namespace Zimbra.Community.Extensions.TableOfContents.Tests.Service
{
	[TestFixture]
	public class InsertAnchorTests
	{
		private TableOfContentsService _tableOfContentsService;

		[TestFixtureSetUp]
		public void SetUp()
		{
			_tableOfContentsService = new TableOfContentsService(new DummyHtmlStripper());
		}

		[Test]
		public void Test_Throws_Exception_If_Invalid_Heading_Provided()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => _tableOfContentsService.InsertAnchor("Invalid Heading"));
		}

		[Test]
		public void Test_Doesnt_Insert_Anchor_If_Empty_Heading_Provided()
		{
			const string input = "<h2> </h2>";
			Assert.AreEqual(input, _tableOfContentsService.InsertAnchor(input));
		}

		[Test]
		public void Test_Inserts_Anchor_At_Begining_Of_Heading_Tag()
		{
			Assert.AreEqual("<h3><a id=\"Heading\" name=\"Heading\"></a>Heading</h3>", _tableOfContentsService.InsertAnchor("<h3>Heading</h3>"));
		}

	}
}
