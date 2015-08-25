using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Zimbra.Community.Extensions.TableOfContents
{
	public class TableOfContentsService : ITableOfContentsService
	{
		private static readonly Regex HeaderTagRegex = new Regex("<h([1-6])[^>]*>(.*?)<\\/h\\1>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex AnchorRegex = new Regex("<a (?:[^>]* )?(?:name|id)=\"([^\"]*)\"[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private readonly Random _random = new Random();
		private readonly IHtmlStripper _htmlStripper;

		public TableOfContentsService(IHtmlStripper htmlStripper)
		{
			_htmlStripper = htmlStripper;
		}

		public string EnsureHeadersHaveAnchors(string html)
		{
			if (String.IsNullOrEmpty(html))
				return html;

			var resultingHtml = new StringBuilder();
			var lastHeadingCloseIndex = 0;
			foreach (Match match in HeaderTagRegex.Matches(html))
			{
				//Insert all HTML since the last match
				resultingHtml.Append(html.Substring(lastHeadingCloseIndex, match.Index - lastHeadingCloseIndex));

				var headingHtml = match.Value;
				// If we don't have an anchor, add one
				if (!AnchorRegex.IsMatch(headingHtml))
					headingHtml = InsertAnchor(headingHtml);

				resultingHtml.Append(headingHtml);
				lastHeadingCloseIndex = match.Index + match.Length;
			}

			resultingHtml.Append(html.Substring(lastHeadingCloseIndex));

			return resultingHtml.ToString();
		}

		public IEnumerable<Heading> GetHeadings(string html)
		{
			foreach (Match match in HeaderTagRegex.Matches(html))
			{
				var contents = match.Groups[2].Value;
				var anchor = AnchorRegex.Match(contents);
				if (anchor.Success)
					yield return new Heading
					{
						HeadingType = (HeadingType)byte.Parse(match.Groups[1].Value),
						Title = _htmlStripper.RemoveHtml(contents),
						AnchorName = anchor.Groups[1].Value
					};
			}
		}

		public HierarchyCollection<Heading> GetHeadingHierarchy(string html)
		{
			var hierarchy = new HierarchyCollection<Heading>();
			var headingsStack = new Stack<HierarchyItem<Heading>>();

			foreach (var heading in GetHeadings(html))
			{
				var hiearchyItem = new HierarchyItem<Heading>(heading);

				var addedItem = false;
				while (headingsStack.Count > 0)
				{
					// Keep looking up the heading hierarchy until we find the 
					// first item that is a higher heading type than the current
					// heading
					var parentHeading = headingsStack.Peek();
					if (parentHeading.Item.HeadingType < heading.HeadingType)
					{
						headingsStack.Push(hiearchyItem);
						parentHeading.Children.Add(hiearchyItem);
						addedItem = true;
						break;
					}
					headingsStack.Pop();
				}

				if (!addedItem)
				{
					headingsStack.Push(hiearchyItem);
					hierarchy.Add(hiearchyItem);
				}
			}

			return hierarchy;
		}

		internal string InsertAnchor(string heading)
		{
			var match = HeaderTagRegex.Match(heading);
			if (!match.Success)
				throw new ArgumentOutOfRangeException("heading", heading, "Heading is not valid heading tag");

			// If we can't generate an anchor name, don't insert the anchor
			var headingContents = match.Groups[2].Value;
			var anchorName = MakeAnchorName(headingContents);
			if (String.IsNullOrEmpty(anchorName))
				return heading;

			var anchorPosition = match.Groups[2].Index;
			var anchor = String.Concat("<a id=\"", anchorName, "\" name=\"", anchorName, "\"></a>");
			return heading.Insert(anchorPosition, anchor);
		}

		private static bool IsRomanLetter(char c)
		{
			return (((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z')));
		}

		internal string MakeAnchorName(string heading)
		{
			/* To be a valid anchor name, first character *must*
			 * be a letter.  Subsequent charaters may additionally include:
			 * numbers, '-', '_', ':' and '.'
			 * */
			var plainText = _htmlStripper.RemoveHtml(heading).Normalize(NormalizationForm.FormD);

			if (String.IsNullOrWhiteSpace(plainText))
				return String.Empty;

			var anchorName = new StringBuilder();

			var enumerator = plainText.GetEnumerator();
			enumerator.Reset();
			// First character must be a letter
			while (enumerator.MoveNext())
			{
				if (IsRomanLetter(enumerator.Current))
				{
					anchorName.Append(enumerator.Current);
					break;
				}
			}

			while (enumerator.MoveNext())
			{
				char c = enumerator.Current;
				var lastCharacter = anchorName[anchorName.Length - 1];

				switch (CharUnicodeInfo.GetUnicodeCategory(c))
				{
					case UnicodeCategory.UppercaseLetter:
					case UnicodeCategory.LowercaseLetter:
						if (IsRomanLetter(c))
							anchorName.Append(c);
						//TODO: Replace non roman characters with transliterated version
						break;

					case UnicodeCategory.DecimalDigitNumber:
					case UnicodeCategory.OtherNumber:
						double number = CharUnicodeInfo.GetNumericValue(c);
						if (number != -1.0)
							anchorName.Append(number);
						break;

					case UnicodeCategory.ConnectorPunctuation:
					case UnicodeCategory.DashPunctuation:
						if (lastCharacter == '_')
							anchorName[anchorName.Length - 1] = '-';
						else if (lastCharacter != '-')
							anchorName.Append('-');
						break;

					case UnicodeCategory.OtherPunctuation:
					case UnicodeCategory.SpaceSeparator:
						if ((lastCharacter != '_') && (lastCharacter != '-'))
							anchorName.Append('_');
						break;
				}
			}

			while (anchorName.Length != 0 && !char.IsLetterOrDigit(anchorName[anchorName.Length - 1]))
			{
				anchorName.Remove(anchorName.Length - 1, 1);
			}

			return anchorName.Length == 0 ?
				"T_" + _random.Next(1000, int.MaxValue)
				: anchorName.ToString();

		}
	}
}

