using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;
using PollsApi = Telligent.BigSocial.Polling.PublicApi;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.InternalApi
{
	internal static class PollingDataService
	{
		internal static string ConnectionString { get; set; }

		internal static void AddUpdatePoll(Poll poll)
		{
			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_Poll_AddUpdate]", connection))
				{
					command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = poll.Id;
					command.Parameters.Add("@Name", SqlDbType.NVarChar, 255).Value = poll.Name;
					command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = poll.Description;
					command.Parameters.Add("@AuthorUserId", SqlDbType.Int).Value = poll.AuthorUserId;
                    command.Parameters.Add("@ApplicationId", SqlDbType.UniqueIdentifier).Value = poll.ApplicationId;
					command.Parameters.Add("@IsEnabled", SqlDbType.Bit).Value = poll.IsEnabled;
					command.Parameters.Add("@DateUtc", SqlDbType.DateTime).Value = DateTime.UtcNow;
					command.Parameters.Add("@HideResultsUntilVotingComplete", SqlDbType.Bit).Value = poll.HideResultsUntilVotingComplete;
					command.Parameters.Add("@VotingEndDateUtc", SqlDbType.DateTime).Value = poll.VotingEndDateUtc;
                    command.Parameters.Add("@NewPollEmailSent", SqlDbType.Bit).Value = poll.NewPollEmailSent;
                    command.Parameters.Add("@IsSuspectedAbusive", SqlDbType.Bit).Value = poll.IsSuspectedAbusive;

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
			}
		}

		internal static void DeletePoll(Guid pollId)
		{
			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_Poll_Delete]", connection))
				{
					command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = pollId;

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
			}
		}

		internal static Poll GetPoll(Guid pollId)
		{
			Poll poll = null;

			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_Poll_Get]", connection))
				{
					command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = pollId;

					connection.Open();

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							poll = PopulatePoll(reader);

							reader.NextResult();
							while (reader.Read())
							{
								poll.Answers.Add(PopulatePollAnswer(reader));
							}
						}
					}

					connection.Close();
				}
			}

			return poll;
		}

        internal static PagedList<Poll> ListPolls(Telligent.Evolution.Extensibility.Api.Entities.Version1.Group group, User user, Guid[] filterGroupIds, PollsApi.PollsListOptions options)
		{
			List<Poll> polls = new List<Poll>();

			using (var connection = GetSqlConnection())
			{
                using (var command = CreateSprocCommand("[polling_Polls_List]", connection))
				{
                    PollsQueryBuilder queryBuilder = new PollsQueryBuilder(group, user, filterGroupIds, options);

                    command.Parameters.Add("@sql", SqlDbType.NVarChar, -1).Value = queryBuilder.BuildQuery();
                    command.Parameters.Add("@TotalRecords", SqlDbType.Int).Direction = ParameterDirection.Output;

                    for (int i = 0; i < queryBuilder.SqlParameters.Length; i++)
                        command.Parameters.Add(queryBuilder.SqlParameters[i]);

					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						Dictionary<Guid, Poll> pollsById = new Dictionary<Guid, Poll>();

						while (reader.Read())
						{
							var poll = PopulatePoll(reader);
							pollsById[poll.Id] = poll;
							polls.Add(poll);
						}

						reader.NextResult();

						while (reader.Read())
						{
							var pollAnswer = PopulatePollAnswer(reader);
							Poll poll;
							if (pollsById.TryGetValue(pollAnswer.PollId, out poll))
								poll.Answers.Add(pollAnswer);
						}

                        reader.Close();
					}
					connection.Close();

					return new PagedList<Poll>(polls, options.PageSize, options.PageIndex, (int)command.Parameters["@TotalRecords"].Value);
				}
			}
		}

        internal static List<Poll> ListPollsToSendNewPollEmail()
        {
            List<Poll> polls = new List<Poll>();
            using (var connection = GetSqlConnection())
            {
                using (var command = CreateSprocCommand("[polling_Poll_ListPollsToSendNewPollEmail]", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        Dictionary<Guid, Poll> pollsById = new Dictionary<Guid, Poll>();

                        while (reader.Read())
                        {
                            var poll = PopulatePoll(reader);
                            pollsById[poll.Id] = poll;
                            polls.Add(poll);
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            var pollAnswer = PopulatePollAnswer(reader);
                            Poll poll;
                            if (pollsById.TryGetValue(pollAnswer.PollId, out poll))
                                poll.Answers.Add(pollAnswer);
                        }
                    }
                    connection.Close();

                    return polls;
                }
            }
        }

        internal static void ReIndexAllPolls()
        {
            using (var connection = GetSqlConnection())
            {
                using (var command = CreateSprocCommand("[polling_Poll_ReindexAll]", connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        
        internal static PagedList<Poll> ListPollsToReindex(int pageSize, int pageIndex)
		{
			List<Poll> polls = new List<Poll>();
			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_Poll_GetToReindex]", connection))
				{
					command.Parameters.Add("@PagingBegin", SqlDbType.Int).Value = pageSize * pageIndex;
					command.Parameters.Add("@PagingEnd", SqlDbType.Int).Value = pageSize * (pageIndex + 1);
					command.Parameters.Add("@TotalRecords", SqlDbType.Int).Direction = ParameterDirection.Output;

					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						Dictionary<Guid, Poll> pollsById = new Dictionary<Guid, Poll>();

						while (reader.Read())
						{
							var poll = PopulatePoll(reader);
							pollsById[poll.Id] = poll;
							polls.Add(poll);
						}

						reader.NextResult();

						while (reader.Read())
						{
							var pollAnswer = PopulatePollAnswer(reader);
							Poll poll;
							if (pollsById.TryGetValue(pollAnswer.PollId, out poll))
								poll.Answers.Add(pollAnswer);
						}
					}
					connection.Close();

					return new PagedList<Poll>(polls, pageSize, pageIndex, (int)command.Parameters["@TotalRecords"].Value);
				}
			}
		}

		internal static void SetPollsAsIndexed(IEnumerable<Guid> pollIds)
		{
			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_Poll_SetIndexed]", connection))
				{
					command.Parameters.Add("@PollIds", SqlDbType.Xml).Value = "<Polls>" + string.Join("", pollIds.Select(x => "<Poll Id=\"" + x.ToString() + "\" />")) + "</Polls>";

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
			}
		}

		internal static void AddUpdatePollAnswer(PollAnswer answer)
		{
			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_PollAnswer_AddUpdate]", connection))
				{
					command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = answer.Id;
					command.Parameters.Add("@PollId", SqlDbType.UniqueIdentifier).Value = answer.PollId;
					command.Parameters.Add("@Name", SqlDbType.NVarChar, 255).Value = answer.Name;

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
			}
		}

		internal static void DeletePollAnswer(Guid pollAnswerId)
		{
			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_PollAnswer_Delete]", connection))
				{
					command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = pollAnswerId;

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
			}
		}

		internal static PollAnswer GetPollAnswer(Guid pollAnswerId)
		{
			PollAnswer answer = null;

			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_PollAnswer_Get]", connection))
				{
					command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = pollAnswerId;

					connection.Open();

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							answer = PopulatePollAnswer(reader);
						}
					}

					connection.Close();
				}
			}

			return answer;
		}

		internal static void AddUpdatePollVote(PollVote vote)
		{
			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_PollVote_AddUpdate]", connection))
				{
					command.Parameters.Add("@PollId", SqlDbType.UniqueIdentifier).Value = vote.PollId;
					command.Parameters.Add("@UserId", SqlDbType.Int).Value = vote.UserId;
					command.Parameters.Add("@PollAnswerId", SqlDbType.UniqueIdentifier).Value = vote.PollAnswerId;
					command.Parameters.Add("@DateUtc", SqlDbType.DateTime).Value = DateTime.UtcNow;

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
			}
		}

		internal static void DeletePollVote(Guid pollId, int userId)
		{
			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_PollVote_Delete]", connection))
				{
					command.Parameters.Add("@PollId", SqlDbType.UniqueIdentifier).Value = pollId;
					command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
			}
		}

		internal static PollVote GetPollVote(Guid pollId, int userId)
		{
			PollVote vote = null;

			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_PollVote_Get]", connection))
				{
					command.Parameters.Add("@PollId", SqlDbType.UniqueIdentifier).Value = pollId;
					command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

					connection.Open();

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							vote = PopulatePollVote(reader);
						}
					}

					connection.Close();
				}
			}

			return vote;
		}	

		internal static bool IsConnectionStringValid()
		{
			bool isValid = false;

			if (!string.IsNullOrEmpty(ConnectionString))
			{
				try
				{
					using (var connection = GetSqlConnection())
					{
						using (var command = new SqlCommand("SELECT IS_MEMBER('db_owner') As IsOwner", connection))
						{
							connection.Open();
							using (var reader = command.ExecuteReader())
							{
								isValid = reader.Read() && Convert.ToBoolean(reader["IsOwner"]);
							}
							connection.Close();
						}
					}
				}
				catch 
				{
					isValid = false;
				}
			}

			return isValid;
		}

		internal static void DeletePollsByGroup(Guid applicationId)
		{
			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_Poll_DeleteByGroup]", connection))
				{
                    command.Parameters.Add("@ApplicationId", SqlDbType.Int).Value = applicationId;

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
			}
		}

		internal static void ReassigneUser(int oldUserId, int newUserId)
		{
			using (var connection = GetSqlConnection())
			{
				using (var command = CreateSprocCommand("[polling_ReassignUser]", connection))
				{
					command.Parameters.Add("@OldUserId", SqlDbType.Int).Value = oldUserId;
					command.Parameters.Add("@NewUserId", SqlDbType.Int).Value = newUserId;

					connection.Open();
					command.ExecuteNonQuery();
					connection.Close();
				}
			}
		}

		internal static void Install()
		{
			Install("install.sql");
		}

		internal static void Install(string fileName)
		{
			using (var connection = GetSqlConnection())
			{
				connection.Open();
				foreach (string statement in GetStatementsFromSqlBatch(EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources.Sql." + fileName)))
				{
					using (var command = new SqlCommand(statement, connection))
					{
						command.ExecuteNonQuery();
					}
				}
				connection.Close();
			}
		}

		internal static void UnInstall()
		{
			using (var connection = GetSqlConnection())
			{
				connection.Open();
				foreach (string statement in GetStatementsFromSqlBatch(EmbeddedResources.GetString("Telligent.BigSocial.Polling.Resources.Sql.uninstall.sql")))
				{
					using (var command = new SqlCommand(statement, connection))
					{
						command.ExecuteNonQuery();
					}
				}
				connection.Close();
			}
		}

		#region Population

		private static Poll PopulatePoll(IDataReader reader)
		{
			Poll poll = new Poll();

			poll.AuthorUserId = Convert.ToInt32(reader["AuthorUserId"]);
			poll.CreatedDateUtc = Convert.ToDateTime(reader["CreatedDateUtc"]);
			poll.Description = reader["Description"] == DBNull.Value ? string.Empty : reader["Description"].ToString();
            poll.ApplicationId = (Guid)reader["ApplicationId"];
			poll.Id = (Guid)reader["Id"];
			poll.IsEnabled = Convert.ToBoolean(reader["IsEnabled"]);
			poll.LastUpdatedDateUtc = Convert.ToDateTime(reader["LastUpdatedDateUtc"]);
			poll.Name = reader["Name"].ToString();
			poll.HideResultsUntilVotingComplete = Convert.ToBoolean(reader["HideResultsUntilVotingComplete"]);
			poll.VotingEndDateUtc = reader["VotingEndDateUtc"] == DBNull.Value ? null : (DateTime?) Convert.ToDateTime(reader["VotingEndDateUtc"]);
            poll.NewPollEmailSent = Convert.ToBoolean(reader["NewPollEmailSent"]);
            poll.IsSuspectedAbusive = Convert.ToBoolean(reader["IsSuspectedAbusive"]);
			
			poll.Answers = new List<PollAnswer>();

			return poll;
		}

		private static PollAnswer PopulatePollAnswer(IDataReader reader)
		{
			PollAnswer answer = new PollAnswer();

			answer.Id = (Guid)reader["Id"];
			answer.Name = reader["Name"].ToString();
			answer.PollId = (Guid)reader["PollId"];
			answer.VoteCount = Convert.ToInt32(reader["VoteCount"]);

			return answer;
		}

		private static PollVote PopulatePollVote(IDataReader reader)
		{
			PollVote vote = new PollVote();

			vote.CreatedDateUtc = Convert.ToDateTime(reader["CreatedDateUtc"]);
			vote.LastUpdatedDateUtc = Convert.ToDateTime(reader["LastUpdatedDateUtc"]);
			vote.PollAnswerId = (Guid)reader["PollAnswerId"];
			vote.PollId = (Guid)reader["PollId"];
			vote.UserId = Convert.ToInt32(reader["UserId"]);

			return vote;
		}

		#endregion

		#region Helpers

		private static IEnumerable<string> GetStatementsFromSqlBatch(string sqlBatch)
		{
			// This isn't as reliable as the SQL Server SDK, but works for most SQL batches and prevents another assembly reference
			foreach (string statement in Regex.Split(sqlBatch, @"^\s*GO\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline))
			{
				string sanitizedStatement = Regex.Replace(statement, @"(?:^SET\s+.*?$|\/\*.*?\*\/|--.*?$)", "\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline).Trim();
				if (sanitizedStatement.Length > 0)
					yield return sanitizedStatement;
			}				
		}

		private static SqlConnection GetSqlConnection()
		{
			return new SqlConnection(ConnectionString);
		}

		private static SqlCommand CreateSprocCommand(string sprocName, SqlConnection connection)
		{
			return new SqlCommand("dbo." + sprocName, connection) { CommandType = CommandType.StoredProcedure };
		}

		#endregion
	}
}
