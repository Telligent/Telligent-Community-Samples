using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.UI.Version1;

namespace Telligent.BigSocial.Polling.Plugins
{
	public class PollViewer : IPlugin, IFileViewer
	{
		#region IPlugin Members
		
		public string Name
		{
			get { return "Poll Viewer"; }
		}

		public string Description
		{
			get { return "Enables polls to be embedded in other content"; }
		}

		public void Initialize()
		{
		}

		#endregion

		#region IFileViewer Members

		public int DefaultOrderNumber
		{
			get { return 100; }
		}

		public FileViewerMediaType GetMediaType(Uri url, IFileViewerOptions options)
		{
			return FileViewerMediaType.Video;
		}

		public FileViewerMediaType GetMediaType(Evolution.Extensibility.Storage.Version1.ICentralizedFile file, IFileViewerOptions options)
		{
			throw new FileViewerNotSupportedException();
		}

		public string Render(Uri url, IFileViewerOptions options)
		{
            var pollIdString = url.Segments[url.Segments.Length - 1];
			Guid pollId;
            if (!Guid.TryParse(pollIdString, out pollId))
				throw new FileViewerNotSupportedException();

			var poll = InternalApi.PollingService.GetPoll(pollId);
			if (poll != null)
				return string.Format("<div style=\"width:{1}\"><div class=\"ui-poll\" data-pollid=\"{0}\" data-showname=\"true\" data-readonly=\"{2}\"></div></div>", 
					poll.Id.ToString(),
					options.Width.HasValue && options.Width.Value > 0 ? options.Width.Value.ToString() + "px" : "inherit",
					options.ViewType == FileViewerViewType.Preview ? "true" : "false"
					);
			else
				return string.Empty;
		}

		public string Render(Evolution.Extensibility.Storage.Version1.ICentralizedFile file, IFileViewerOptions options)
		{
			throw new FileViewerNotSupportedException();
		}

		public string[] SupportedFileExtensions
		{
			get { return null; }
		}

		public string SupportedUrlPattern
		{
			get { return @"/polls/[0-9a-f]{8,8}\-?[0-9a-f]{4,4}\-?[0-9a-f]{4,4}\-?[0-9a-f]{4,4}\-?[0-9a-f]{12,12}"; }
		}

		#endregion
	}
}
