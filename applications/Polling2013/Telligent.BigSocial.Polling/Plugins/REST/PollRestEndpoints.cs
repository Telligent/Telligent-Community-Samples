using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telligent.Evolution.Extensibility.Version1;
using Telligent.Evolution.Extensibility.Rest.Version2;
using Telligent.Evolution.Extensibility.Rest.Entities.Version2;
using Telligent.Evolution.Extensibility.Api.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.Plugins
{
	public class PollRestEndpoints: IPlugin, IRestEndpoints
	{
		#region IPlugin Members

		public string Name
		{
			get { return "Poll REST API Endpoints";  }
		}

		public string Description
		{
			get { return "Adds support for Poll REST endpoints."; }
		}

		public void Initialize()
		{
		}

		#endregion

		#region IRestEndpoints Members

		public void Register(IRestEndpointController controller)
		{
			#region Poll Endpoints

			controller.Add(2, "groups/{groupid}/polls", null, new { groupid = @"\d+" }, HttpMethod.Get, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();
				response.Name = "Polls";

				try
				{
					int pageSize;
					int pageIndex;
					string sortBy = "Date";

					if (!int.TryParse(request.Request.QueryString["PageSize"], out pageSize))
						pageSize = 20;

					if (!int.TryParse(request.Request.QueryString["PageIndex"], out pageIndex))
						pageIndex = 0;

					if (request.Request.QueryString["SortBy"] != null)
						sortBy = request.Request.QueryString["SortBy"];

					if (sortBy == "TopPollsScore")
					{
						var group = TEApi.Groups.Get(new GroupsGetOptions { Id = Convert.ToInt32(request.PathParameters["groupid"]) });
						if (group == null || group.HasErrors())
							response.Data = new Telligent.Evolution.Extensibility.Rest.Entities.Version1.PagedList<RestApi.Poll>();
						else
						{
							var scores = TEApi.CalculatedScores.List(Plugins.TopPollsScore.ScoreId, new CalculatedScoreListOptions { ApplicationId = group.ApplicationId, ContentTypeId = PublicApi.Polls.ContentTypeId, PageIndex = pageIndex, PageSize = pageSize, SortOrder = "Descending" });

							var polls = new List<RestApi.Poll>();
							foreach (var score in scores)
							{
								if (score.Content != null)
								{
									var poll = InternalApi.PollingService.GetPoll(score.Content.ContentId);
									if (poll != null)
										polls.Add(new RestApi.Poll(poll));
								}
							}

							response.Data = new Telligent.Evolution.Extensibility.Rest.Entities.Version1.PagedList<RestApi.Poll>(polls, scores.PageSize, scores.PageIndex, scores.TotalCount);
						}
					}
					else
					{
                        var polls = InternalApi.PollingService.ListPolls(Convert.ToInt32(request.PathParameters["groupid"]), new PublicApi.PollsListOptions() { PageIndex = pageIndex, PageSize = pageSize, SortBy = sortBy });
						response.Data = new Telligent.Evolution.Extensibility.Rest.Entities.Version1.PagedList<RestApi.Poll>(polls.Select(x => new RestApi.Poll(x)), polls.PageSize, polls.PageIndex, polls.TotalCount);
					}
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			controller.Add(2, "polls/poll", HttpMethod.Get, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();
				response.Name = "Poll";

				try
				{
					Guid pollId;
					if (!Guid.TryParse(request.Request.QueryString["Id"], out pollId))
						throw new ArgumentException("Id is required.");

					var poll = InternalApi.PollingService.GetPoll(pollId);
					if (poll == null)
						throw new Exception("The poll does not exist.");
					
					response.Data = new RestApi.Poll(poll);
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			controller.Add(2, "polls/poll", HttpMethod.Delete, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();

				try
				{
					Guid pollId;
					if (!Guid.TryParse(request.Request.QueryString["Id"], out pollId))
						throw new ArgumentException("Id is required.");

					var poll = InternalApi.PollingService.GetPoll(pollId);
					if (poll != null)
						InternalApi.PollingService.DeletePoll(poll);
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			controller.Add(2, "polls/poll", HttpMethod.Post, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();
				response.Name = "Poll";

				try
				{
					// Create
					int groupId;
					if (!int.TryParse(request.Form["GroupId"], out groupId))
						throw new ArgumentException("GroupId is required.");

                    var group = TEApi.Groups.Get(new GroupsGetOptions() { Id = groupId });
                    if (group == null)
                        throw new ArgumentException("Group not found");

					string name = request.Form["Name"] ?? string.Empty;
					string description = request.Form["Description"];
					bool hideResultsUntilVotingComplete = request.Form["HideResultsUntilVotingComplete"] == null ? false : Convert.ToBoolean(request.Form["HideResultsUntilVotingComplete"]);
					DateTime? votingEndDate = request.Form["VotingEndDate"] == null ? null : (DateTime?)InternalApi.Formatting.FromUserTimeToUtc(DateTime.Parse(request.Form["VotingEndDate"]));

					var poll = new InternalApi.Poll();
					poll.ApplicationId = group.ApplicationId;
					poll.Name = name;
					poll.Description = description;
					poll.IsEnabled = true;
					poll.AuthorUserId = request.UserId;
					poll.HideResultsUntilVotingComplete = hideResultsUntilVotingComplete;
					poll.VotingEndDateUtc = votingEndDate;

					InternalApi.PollingService.AddUpdatePoll(poll);
					poll = InternalApi.PollingService.GetPoll(poll.Id);
					
					response.Data = new RestApi.Poll(poll);
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			controller.Add(2, "polls/poll", HttpMethod.Put, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();
				response.Name = "Poll";

				try
				{
					// Update
					Guid pollId;
					if (!Guid.TryParse(request.Request.QueryString["Id"], out pollId))
						throw new ArgumentException("Id is required.");

					string name = request.Form["Name"];
					string description = request.Form["Description"];

					var poll = InternalApi.PollingService.GetPoll(pollId);
					if (poll == null)
						throw new Exception("The poll does not exist.");

					if (request.Form["HideResultsUntilVotingComplete"] != null)
						poll.HideResultsUntilVotingComplete = Convert.ToBoolean(request.Form["HideResultsUntilVotingComplete"]);

					if (request.Form["VotingEndDate"] != null)
						poll.VotingEndDateUtc = (DateTime?) InternalApi.Formatting.FromUserTimeToUtc(Convert.ToDateTime(request.Form["VotingEndDate"]));
					else if (request.Form["ClearVotingEndDate"] != null && Convert.ToBoolean(request.Form["ClearVotingEndDate"]))
						poll.VotingEndDateUtc = null;
					
					if (name != null)
						poll.Name = name;

					if (description != null)
						poll.Description = description;

					InternalApi.PollingService.AddUpdatePoll(poll);
					poll = InternalApi.PollingService.GetPoll(poll.Id);

					response.Data = new RestApi.Poll(poll);
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			#endregion

			#region Poll Answer Endpoints

			controller.Add(2, "polls/answer", HttpMethod.Get, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();
				response.Name = "PollAnswer";

				try
				{
					Guid pollAnswerId;
					if (!Guid.TryParse(request.Request.QueryString["Id"], out pollAnswerId))
						throw new ArgumentException("Id is required.");

					var pollAnswer = InternalApi.PollingService.GetPollAnswer(pollAnswerId);
					if (pollAnswer == null)
						throw new Exception("The poll answer does not exist.");

					var poll = InternalApi.PollingService.GetPoll(pollAnswer.PollId);
					if (poll == null)
						throw new Exception("The poll does not exist.");

					response.Data = new RestApi.PollAnswer(pollAnswer, poll);
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			controller.Add(2, "polls/answer", HttpMethod.Delete, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();

				try
				{
					Guid pollAnswerId;
					if (!Guid.TryParse(request.Request.QueryString["Id"], out pollAnswerId))
						throw new ArgumentException("Id is required.");

					var pollAnswer = InternalApi.PollingService.GetPollAnswer(pollAnswerId);
					if (pollAnswer != null)
						InternalApi.PollingService.DeletePollAnswer(pollAnswer);
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			controller.Add(2, "polls/answer", HttpMethod.Post, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();
				response.Name = "PollAnswer";

				try
				{
					// Create
					Guid pollId;
					if (!Guid.TryParse(request.Form["PollId"], out pollId))
						throw new ArgumentException("PollId is required.");

					string name = request.Form["Name"] ?? string.Empty;

					var pollAnswer = new InternalApi.PollAnswer();
					pollAnswer.PollId = pollId;
					pollAnswer.Name = name;

					InternalApi.PollingService.AddUpdatePollAnswer(pollAnswer);
					pollAnswer = InternalApi.PollingService.GetPollAnswer(pollAnswer.Id);

					var poll = InternalApi.PollingService.GetPoll(pollAnswer.PollId);

					response.Data = new RestApi.PollAnswer(pollAnswer, poll);
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			controller.Add(2, "polls/answer", HttpMethod.Put, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();
				response.Name = "PollAnswer";

				try
				{
					// Update
					Guid pollAnswerId;
					if (!Guid.TryParse(request.Request.QueryString["Id"], out pollAnswerId))
						throw new ArgumentException("Id is required.");

					string name = request.Form["Name"];

					var pollAnswer = InternalApi.PollingService.GetPollAnswer(pollAnswerId);
					if (pollAnswer == null)
						throw new Exception("The poll answer does not exist.");

					if (name != null)
						pollAnswer.Name = name;

					InternalApi.PollingService.AddUpdatePollAnswer(pollAnswer);
					pollAnswer = InternalApi.PollingService.GetPollAnswer(pollAnswer.Id);

					var poll = InternalApi.PollingService.GetPoll(pollAnswer.PollId);

					response.Data = new RestApi.PollAnswer(pollAnswer, poll);
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			#endregion

			#region Poll Voting Endpoints

			controller.Add(2, "polls/vote", HttpMethod.Get, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();
				response.Name = "PollVote";

				try
				{
					Guid pollId;
					if (!Guid.TryParse(request.Request.QueryString["PollId"], out pollId))
						throw new ArgumentException("PollId is required.");

					var poll = InternalApi.PollingService.GetPoll(pollId);
					if (poll == null)
						throw new Exception("The poll does not exist.");

					var vote = InternalApi.PollingService.GetPollVote(pollId, request.UserId);
					if (vote != null)
						response.Data = new RestApi.PollVote(vote);
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			controller.Add(2, "polls/vote", HttpMethod.Delete, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();

				try
				{
					Guid pollId;
					if (!Guid.TryParse(request.Form["PollId"], out pollId))
						throw new ArgumentException("PollId is required.");

					var vote = InternalApi.PollingService.GetPollVote(pollId, request.UserId);
					if (vote != null)
						InternalApi.PollingService.DeletePollVote(vote);
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			controller.Add(2, "polls/vote", HttpMethod.Post, (IRestRequest request) =>
			{
				var response = new RestApi.RestResponse();
				response.Name = "PollVote";

				try
				{
					// Create
					Guid pollId;
					if (!Guid.TryParse(request.Form["PollId"], out pollId))
						throw new ArgumentException("PollId is required.");

					Guid pollAnswerId;
					if (!Guid.TryParse(request.Form["PollAnswerId"], out pollAnswerId))
						throw new ArgumentException("PollAnswerId is required.");
					
					var pollVote = new InternalApi.PollVote();
					pollVote.PollId = pollId;
					pollVote.PollAnswerId = pollAnswerId;
					pollVote.UserId = request.UserId;

					InternalApi.PollingService.AddUpdatePollVote(pollVote);
					pollVote = InternalApi.PollingService.GetPollVote(pollId, request.UserId);

					response.Data = new RestApi.PollVote(pollVote);
				}
				catch (Exception ex)
				{
					response.Errors = new string[] { ex.Message };
				}

				return response;
			});

			#endregion
		}

		#endregion
	}
}
