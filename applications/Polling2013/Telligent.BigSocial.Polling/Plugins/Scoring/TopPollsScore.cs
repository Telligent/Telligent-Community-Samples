using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.Scoring.Version1;

namespace Telligent.BigSocial.Polling.Plugins
{
	public class TopPollsScore : IPlugin, IScore, IContentTypeLimitedScore
	{
		private static readonly Guid _scoreId = new Guid("dc3e0309c36640aaa51105d9cc8223c6");
		IScoreController _scoreController;

		#region IPlugin Members

		public string Name
		{
			get { return "Top Polls Score"; }
		}

		public string Description
		{
			get { return "This score is used to determine top polls within a group."; }
		}

		public void Initialize()
		{
		}

		#endregion

		#region IScore Members

		public bool AreDecayOverridesEnabledByDefault
		{
			get { return true; }
		}

		public int DefaultHalfLife
		{
			get { return 10; }
		}

		public IEnumerable<Guid> DefaultMetricIds
		{
			get 
			{
				return new Guid[] {
					new Guid("3bbd666589a34b9787f1d10f614d32b4"), // poll votes metric
					new Guid("E979FA18-1443-4829-AA7E-2CBEDFDEB9F8"), // likes metric
					new Guid("4C8EEC0E-09CF-40A2-8929-25088F3A6130"), // tags metric
					new Guid("DA14FD10-7EC9-45F1-8416-C2D3ECBBA1E7") // comments metric
				};
			}
		}

		public Guid Id
		{
			get { return _scoreId; }
		}

		public bool IsDecayEnabledByDefault
		{
			get { return true; }
		}

		public string ScoreDescription
		{
			get { return "Used to identify top polls within a group."; }
		}

		public string ScoreName
		{
			get { return "Top Polls"; }
		}

		public void SetController(IScoreController controller)
		{
			_scoreController = controller;
		}

		#endregion

		#region IContentTypeLimitedScore Members

		public IEnumerable<Guid> SupportedContentTypes
		{
			get { return new Guid[] { PublicApi.Polls.ContentTypeId }; }
		}

		#endregion

		internal static Guid ScoreId { get { return _scoreId; } }
	}
}
