using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using Zimbra.Community.Extensions.TableOfContents.Tests.Mocks;

namespace Zimbra.Community.Extensions.TableOfContents.Tests.Module
{
	[TestFixture]
	public class GenerateTableOfContentsTests
	{
		private TableOfContentsPlugin _tableOfContentsModule;
		private string _tableOfContentsOutput;

		[TestFixtureSetUp]
		public void SetUp()
		{
			var builderMock = new Mock<ITableOfContentsBuilder>();

			builderMock.Setup(x => x.BuildTableOfContents(It.IsAny<HierarchyCollection<Heading>>()))
				.Returns((ICollection<HierarchyItem<Heading>> headings) =>
				{
					return _tableOfContentsOutput;
				});

			_tableOfContentsModule = new TableOfContentsPlugin(new TableOfContentsService(new DummyHtmlStripper()), builderMock.Object);	
		}

		[Test]
		public void Test_Leaves_Html_Alone_If_Does_Not_Contain_TOC_Tag()
		{
			const string input = "<h2><a name=\"Content\"></a>Content</h2><p>Here Is Come Content</p>";

			Assert.AreEqual(input, _tableOfContentsModule.InsertTableOfContents(input));
		}

		[Test]
		public void Test_Removes_TOC_Tag_When_No_TableOfContents_To_Render()
		{
			_tableOfContentsOutput = string.Empty;
			const string input = "[toc]<h2><a name=\"Content\"></a>Content</h2><p>Here Is Come Content</p>";
			var result = _tableOfContentsModule.InsertTableOfContents(input);
			Assert.AreEqual("<h2><a name=\"Content\"></a>Content</h2><p>Here Is Come Content</p>", result);
		}

		[Test]
		public void Test_Replaces_TOC_Tag_With_TableOfContents()
		{
			_tableOfContentsOutput = "[[TABLE-OF-CONTENTS]]";
			const string input = "[toc]<h2><a name=\"Content\"></a>Content</h2><p>Here Is Come Content</p>";
			var result = _tableOfContentsModule.InsertTableOfContents(input);
			Assert.AreEqual("[[TABLE-OF-CONTENTS]]<h2><a name=\"Content\"></a>Content</h2><p>Here Is Come Content</p>", result);
		}

	}
}
