using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using Telligent.Evolution.Extensibility.Version1;
using System.Collections;
using Telligent.Evolution.Urls.Routing;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.WidgetApi
{
	[Documentation(Category="Polling")]
	public class Polls
	{
		[Documentation("The content type identifier for polls.")]
		public Guid ContentTypeId { get { return PublicApi.Polls.ContentTypeId; } }

		[Documentation("The current contextual poll.")]
		public PublicApi.Poll Current
		{
			get
			{
                PublicApi.Poll poll = null;

                var pollId = TEApi.Url.CurrentContext.GetTokenValue("PollId");

                if (pollId != null)
                {
                    Guid id;
                    if (Guid.TryParse(pollId.ToString(), out id))
                        poll = PublicApi.Polls.Get(id);
                }

                return poll;
			}
		}

		[Documentation("List polls within a group.")]
		public PagedList<PublicApi.Poll> List(
			[Documentation("The group identifier.")]
			int groupId
			)
		{
			return List(groupId, null);
		}

		[Documentation("List polls within a group.")]
		public PagedList<PublicApi.Poll> List(
			[Documentation("The group identifier.")]
			int groupId, 
			[
            Documentation(Name="IncludeSubGroups", Type = typeof(bool), Default = false),
            Documentation(Name="AuthorUserId", Type = typeof(int), Description = "UserId of Author"),
            Documentation(Name="PageIndex", Type = typeof(int), Default = 0, Description = "The page index."),
			Documentation(Name="PageSize", Type=typeof(int), Default=20, Description="The number of polls to return in a single page."),
			Documentation(Name="SortBy", Type=typeof(string), Default="Date", Description="The sorting mechanism. ToppPollsScore does not support IncludeSubGroups or AuthorUserId options.", Options=new string[] { "Date", "TopPollsScore" }),
            Documentation(Name="SortOrder", Type = typeof(string), Options = new string[] { "ascending", "descending" }, Default="descending")
            ]
			IDictionary options
			)
		{
            PublicApi.PollsListOptions query = new PublicApi.PollsListOptions();

			if (options != null)
			{
                if (options["IncludeSubGroups"] != null)
                    query.IncludeSubGroups = Convert.ToBoolean(options["IncludeSubGroups"]);
                
                if (options["AuthorUserId"] != null)
                    query.AuthorUserId = Convert.ToInt32(options["AuthorUserId"]);

				if (options["PageIndex"] != null)
					query.PageIndex = Convert.ToInt32(options["PageIndex"]);

				if (options["PageSize"] != null)
                    query.PageSize = Convert.ToInt32(options["PageSize"]);

				if (options["SortBy"] != null)
                    query.SortBy = options["SortBy"].ToString();

                if (options["SortOrder"] != null)
                    query.SortOrder = options["SortOrder"].ToString();
            }

            return PublicApi.Polls.List(groupId, query);
		}

		[Documentation("Get a poll.")]
		public PublicApi.Poll Get(
			[Documentation("The poll's identifier.")]
			Guid id
			)
		{
			return PublicApi.Polls.Get(id);
		}

		[Documentation("Delete a poll.")]
		public AdditionalInfo Delete(
			[Documentation("The poll's identifier.")]
			Guid id
			)
		{
			return PublicApi.Polls.Delete(id);
		}

		[Documentation("Create a new poll.")]
		public PublicApi.Poll Create(
			[Documentation("The identifier of the group in which to create the poll.")]
			int groupId, 
			[Documentation("The name of the new poll.")]
			string name
			)
		{
			return Create(groupId, name, null);
		}

		[Documentation("Create a new poll.")]
		public PublicApi.Poll Create(
			[Documentation("The identifier of the group in which to create the poll.")]
			int groupId, 
			[Documentation("The name of the new poll.")]
			string name, 
			[
				Documentation(Name="Description", Type=typeof(string), Description="The description of the poll."),
				Documentation(Name = "HideResultsUntilVotingComplete", Type = typeof(bool), Description = "True if results should not be shown until the voting end date (requires that voting end date is defined)."),
				Documentation(Name = "VotingEndDate", Type = typeof(DateTime), Description = "The date at which voting ends.")
			]
			IDictionary options)
		{
			string description = null;
			DateTime? votingEndDate = null;
			bool hideResultsUntilVotingComplete = false;

			if (options != null)
			{
				if (options["Description"] != null)
					description = options["Description"].ToString();

				if (options["VotingEndDate"] != null)
					votingEndDate = Convert.ToDateTime(options["VotingEndDate"]);

				if (options["HideResultsUntilVotingComplete"] != null)
					hideResultsUntilVotingComplete = Convert.ToBoolean(options["HideResultsUntilVotingComplete"]);
			}

			return PublicApi.Polls.Create(groupId, name, description, votingEndDate, (bool?) hideResultsUntilVotingComplete);
		}

		[Documentation("Update an existing poll.")]
		public PublicApi.Poll Update(
			[Documentation("The identifier of the poll to update.")]
			Guid id, 
			[
				Documentation(Name="Description", Type=typeof(string), Description="The description of the poll."),
				Documentation(Name = "Name", Type = typeof(string), Description = "The name of the poll."),
				Documentation(Name = "HideResultsUntilVotingComplete", Type = typeof(bool), Description = "True if results should not be shown until the voting end date (requires that voting end date is defined)."),
				Documentation(Name = "VotingEndDate", Type = typeof(DateTime), Description = "The date at which voting ends."),
				Documentation(Name = "ClearVotingEndDate", Type = typeof(bool), Description = "Set to true to remove the existing voting end date.")
			]
			IDictionary options
			)
		{
			string name = null;
			string description = null;
			DateTime? votingEndDate = null;
			bool hideResultsUntilVotingComplete = false;
			bool clearVotingEndDate = false;

			if (options != null)
			{
				if (options["Description"] != null)
					description = options["Description"].ToString();

				if (options["Name"] != null)
					name = options["Name"].ToString();

				if (options["VotingEndDate"] != null)
					votingEndDate = Convert.ToDateTime(options["VotingEndDate"]);
				else if (options["ClearVotingEndDate"] != null && Convert.ToBoolean(options["ClearVotingEndDate"]))
					clearVotingEndDate = true;

				if (options["HideResultsUntilVotingComplete"] != null)
					hideResultsUntilVotingComplete = Convert.ToBoolean(options["HideResultsUntilVotingComplete"]);
			}

			return PublicApi.Polls.Update(id, name, description, votingEndDate, (bool?) hideResultsUntilVotingComplete, (bool?) clearVotingEndDate);
		}

		[Documentation("Identifies if the accessing user can create a poll in the given group.")]
		public bool CanCreate(int groupId)
		{
			return PublicApi.Polls.CanCreate(groupId);
		}

		[Documentation("Identifies if the accessing user can vote on the given poll.")]
		public bool CanVote(Guid pollId)
		{
			return PublicApi.Polls.CanVote(pollId);
		}

		[Documentation("Identifies if the accessing user can edit the given poll.")]
		public bool CanEdit(Guid pollId)
		{
			return PublicApi.Polls.CanEdit(pollId);
		}

		[Documentation("Identifies if the accessing user can delete the given poll.")]
		public bool CanDelete(Guid pollId)
		{
			return PublicApi.Polls.CanDelete(pollId);
		}

		[Documentation("Renders the poll user interface.")]
		public string UI(
			Guid pollId
			)
		{
			return UI(pollId, null);
		}

		[Documentation("Renders the poll user interface.")]
		public string UI(
			Guid pollId, 
			[
				Documentation(Name="ReadOnly", Type=typeof(bool), Description="When true, the UI does not support interactions/voting.", Default=false),
				Documentation(Name="ShowNameAndDescription", Type=typeof(bool), Description="When true, the UI includes the name and description of the poll.", Default=true)
			]
			IDictionary options
			)
		{
			bool readOnly = false;
			bool showNameAndDescription = true;

			if (options != null)
			{
				if (options["ReadOnly"] != null)
					readOnly = Convert.ToBoolean(options["ReadOnly"]);

				if (options["ShowNameAndDescription"] != null)
					showNameAndDescription = Convert.ToBoolean(options["ShowNameAndDescription"]);
			}

			return PublicApi.Polls.UI(pollId, readOnly, showNameAndDescription);
		}
	}
}
