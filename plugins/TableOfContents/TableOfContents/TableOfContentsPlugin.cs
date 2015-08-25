using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Telligent.Evolution.Extensibility.Api.Version1;
using Telligent.Evolution.Extensibility.UI.Version1;
using Telligent.Evolution.Extensibility.Version1;

namespace Zimbra.Community.Extensions.TableOfContents
{
	public class TableOfContentsPlugin : ISingletonPlugin, ITranslatablePlugin, IInstallablePlugin
	{
		private static readonly Version _emptyVersion = new Version(0, 0, 0, 0);
		private readonly ITableOfContentsService _tableOfContentsService;
		private readonly ITableOfContentsBuilder _tableOfContentsBuilder;
		private ITranslatablePluginController _translations;

		public TableOfContentsPlugin()
		{
			_tableOfContentsService = new TableOfContentsService(new HtmlStripper());
			_tableOfContentsBuilder = new TableOfContentsBuilder(this);
		}

		internal TableOfContentsPlugin(ITableOfContentsService tableOfContentsService, ITableOfContentsBuilder tableOfContentsBuilder)
		{
			_tableOfContentsService = tableOfContentsService;
			_tableOfContentsBuilder = tableOfContentsBuilder;
		}

		public string Description { get { return "Toc"; } }
		public string Name { get { return "Table of Contents"; } }
		public string Title { get { return _translations.GetLanguageResourceValue("title"); } }
		public Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

		public void Initialize()
		{
			PublicApi.Html.Events.Render += Events_Render;
			PublicApi.Html.Events.BeforeCreate += Html_Modified;
			PublicApi.Html.Events.BeforeUpdate += Html_Modified;

			PublicApi.WikiPages.Events.BeforeCreate += Events_BeforeCreate;
#if DEBUG
			Install(_emptyVersion);
#endif
		}

		void Events_BeforeCreate(WikiPageBeforeCreateEventArgs e)
		{
			e.Body = _tableOfContentsService.EnsureHeadersHaveAnchors(e.Body);
		}

		private void Html_Modified(EditableHtmlEventArgsBase e)
		{
			foreach (var property in e.Properties.PropertyNames.Where(x => x.Equals("Body", StringComparison.OrdinalIgnoreCase)))
				e.Properties[property] = _tableOfContentsService.EnsureHeadersHaveAnchors(e.Properties[property]);
		}


		private void Events_Render(HtmlRenderEventArgs e)
		{
			if (!e.RenderedProperty.Equals("Body", StringComparison.OrdinalIgnoreCase))
				return;

			switch(e.RenderTarget)
			{
				//TODO: Should we display for email, any other targets?
				case "Web":
					e.RenderedHtml = InsertTableOfContents(e.RenderedHtml);
					break;
				default:
					e.RenderedHtml = e.RenderedHtml.Replace("[toc]", "");
					break;
			}
		}

		public void SetController(ITranslatablePluginController controller)
		{
			_translations = controller;
		}

		public Translation[] DefaultTranslations
		{
			get
			{
				var en = new Translation("en-us");
				en.Set("title", "Table of Contents");

				return new[] { en };
			}
		}


		/// <summary>
		/// Takes an HTML string and inserts the Table of Contents into
		/// the Html
		/// </summary>
		/// <param name="html">The html to insert the Table of Contents into</param>
		/// <returns>An Html string containing the original html along with a table of contents</returns>
		internal string InsertTableOfContents(string html)
		{
			var position = GetTableOfContentsPosition(ref html);

			if (position == -1)
				return html;

			var hierarchy = _tableOfContentsService.GetHeadingHierarchy(html);

			if (hierarchy == null || !hierarchy.Any())
				return html;

			var tableOfContents = _tableOfContentsBuilder.BuildTableOfContents(hierarchy);

			if (String.IsNullOrEmpty(tableOfContents))
				return html;

			var newHtml = new StringBuilder();
			newHtml.Append(html.Substring(0, position));
			newHtml.Append(tableOfContents);
			newHtml.Append(html.Substring(position));

			return newHtml.ToString();
		}

		/// <summary>
		/// Calculates the index at which the Table of Contents should be inserted into a string.
		/// </summary>
		/// <param name="html">The html to insert the Table of Contents into</param>
		/// <returns>The index where the table of contents should be inserted</returns>
		internal static int GetTableOfContentsPosition(ref string html)
		{
			var index = html.IndexOf("[toc]", StringComparison.OrdinalIgnoreCase);

			if (index >= 3 && index <= html.Length - 9
				&& html.Substring(index - 3, 12).Equals("<p>[toc]</p>", StringComparison.OrdinalIgnoreCase))
			{
				html = html.Remove(index - 3, 12);
				index = index - 3;
			}
			else if (index >= 0)
				html = html.Remove(index, 5);

			return index;
		}


		public void Install(Version lastInstalledVersion)
		{
			if (lastInstalledVersion == _emptyVersion)
			{
				var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Zimbra.Community.Extensions.TableOfContents.Resources.styles.css");
				foreach(var theme in Themes.List(ThemeTypes.Site)){
					ThemeFiles.AddUpdateFactoryDefault(theme, "cssFiles", "TableofContents.css", stream, (int)stream.Length);
				}
			}
		}

		public void Uninstall()
		{
		}

	}
}
