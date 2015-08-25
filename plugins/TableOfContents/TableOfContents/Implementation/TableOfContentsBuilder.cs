using System;
using System.Text;

namespace Zimbra.Community.Extensions.TableOfContents
{
	public class TableOfContentsBuilder : TableOfContentsBuilderBase
	{
		private readonly TableOfContentsPlugin _tableOfContentsPlugin;
		public TableOfContentsBuilder(TableOfContentsPlugin tableOfContentsPlugin)
		{
			_tableOfContentsPlugin = tableOfContentsPlugin;
		}

		protected override void StartTableOfContents(StringBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException("builder");
			builder.Append("<div class=\"table-of-contents\">");
			builder.Append("<h2 class=\"toc-title\">");
			builder.Append(_tableOfContentsPlugin.Title);
			builder.Append("</h2>");
		}

		protected override void EndTableOfContents(StringBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException("builder");
			builder.Append("</div>");
		}

		protected override void StartHierarchyList(StringBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException("builder");
			builder.Append("<div class=\"hierarchy-list-header\"> </div>");
			builder.Append("<ul class=\"hierarchy-list\">");
		}

		protected override void EndHierarchyList(StringBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException("builder");
			builder.Append("</ul>");
			builder.Append("<div class=\"hierarchy-list-footer\"> </div>");
		}

		protected override void StartHierarchyItem(StringBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException("builder");
			builder.Append("<li class=\"hierarchy-item\">");
		}

		protected  override void EndHierarchyItem(StringBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException("builder");
			builder.Append("</li>");
		}
	}
}
