using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using PollsApi = Telligent.BigSocial.Polling.PublicApi;
using Telligent.Evolution.Extensibility.Api.Entities.Version1;

namespace Telligent.BigSocial.Polling.InternalApi
{
    internal class PollsQueryBuilder
    {
        protected string databaseOwner = "dbo";
        private PollsApi.PollsListOptions query;
        private readonly List<SqlParameter> _parameters = null;
        private Group _group;
        private User _accessingUser;
        private Guid[] _groupsToQuery;


        internal SqlParameter[] SqlParameters
        {
            get { return _parameters.ToArray(); }
        }

        internal PollsQueryBuilder(Group group, User accessingUser, Guid[] groupsToQuery, PollsApi.PollsListOptions options)
        {
            query = options;
            _group = group;
            _accessingUser = accessingUser;
            _groupsToQuery = groupsToQuery;
            _parameters = new List<SqlParameter>();
        }

        internal string BuildQuery()
        {
            _parameters.Clear();

            var queryBuilder = new StringBuilder();

            BuildStartQuery(queryBuilder);
            BuildWhereClause(queryBuilder);
            BuildPaging(queryBuilder);

            return queryBuilder.ToString();
        }

        private void BuildStartQuery(StringBuilder queryBuilder)
        {
            queryBuilder.AppendLine(
                string.Format(
                    @"WITH Polls AS (SELECT ROW_NUMBER() OVER({0}) AS RowId
                        ,polls.Id
                        ,COUNT(*) OVER () AS TotalRecords
                         FROM           dbo.polling_Polls polls"
                    , GetSortBy()
                )
            );
        }

        private string GetSortBy()
        {
            var sb = new StringBuilder();

            sb.Append("ORDER BY ");

            switch (query.SortBy.ToLowerInvariant())
            {
                case "name":
                    sb.Append(" polls.Name ");
                    break;
                case "lastupdateddate":
                    sb.Append(" polls.LastUpdatedDateUtc ");
                    break;
                default:
                    sb.Append(" polls.CreatedDateUtc ");
                    break;
            }

            if (query.SortOrder.ToLowerInvariant() == "ascending")
                sb.Append(" ASC ");
            else
                sb.Append(" DESC ");

            return sb.ToString();
        }

        private void BuildWhereClause(StringBuilder queryBuilder)
        {
            queryBuilder.AppendLine(" WHERE 1 = 1 ");

            if (!query.IgnorePermissions)
            {
                queryBuilder.AppendLine(@" AND EXISTS ( SELECT 1 FROM dbo.[vw_v1_permissions_EffectivePermissions] ep WHERE ep.UserId = @QueryUserId AND polls.ApplicationId = ep.NodeId AND ep.IsAllowed = 1 AND ep.PermissionId = @QueryPermissionId)");

                AddSqlParameter("@QueryUserId", _accessingUser.Id.Value, SqlDbType.Int, 4);
                AddSqlParameter("@QueryPermissionId", query.Permission, SqlDbType.UniqueIdentifier);
            }

            if (_groupsToQuery != null && _groupsToQuery.Length > 0)
                queryBuilder.AppendFormat(" AND polls.ApplicationId IN ('{0}') ", string.Join("','", _groupsToQuery));
            else
            {
                queryBuilder.AppendLine(" AND polls.ApplicationId = @QueryApplicationId ");
                AddSqlParameter("@QueryApplicationId", _group.ApplicationId, SqlDbType.UniqueIdentifier);
            }

            if (query.AuthorUserId.HasValue)
            {
                queryBuilder.AppendLine(" AND polls.AuthorUserId=@QueryAuthorUserId ");
                AddSqlParameter("@QueryAuthorUserId", query.AuthorUserId.Value, SqlDbType.Int);
            }

            if (query.IsEnabled.HasValue)
            {
                queryBuilder.AppendLine(" AND polls.IsEnabled=@QueryIsEnabled ");
                AddSqlParameter("@QueryIsEnabled", query.IsEnabled.Value, SqlDbType.Bit);
            }

            if (query.IsSuspectedAbusive.HasValue)
            {
                queryBuilder.AppendLine(" AND polls.IsSuspectedAbusive=@QueryIsSuspectedAbusive ");
                AddSqlParameter("@QueryIsSuspectedAbusive", query.IsSuspectedAbusive.Value, SqlDbType.Bit);
            }
        }

        private void BuildPaging(StringBuilder queryBuilder)
        {
            queryBuilder.AppendLine(" ) ");

            queryBuilder.AppendLine("SELECT RowId, Id, TotalRecords from Polls");
            queryBuilder.AppendLine("WHERE RowId > @QueryLowerBound AND RowId <= @QueryUpperBound ");
            queryBuilder.AppendLine("ORDER BY RowId ASC");

            this.AddSqlParameter("@QueryLowerBound", (query.PageSize * query.PageIndex), SqlDbType.Int, 4);
            this.AddSqlParameter("@QueryUpperBound", (query.PageSize * (query.PageIndex + 1)), SqlDbType.Int, 4);
        }

        private void AddSqlParameter(string parameterName, object value, SqlDbType? dbType = null, int? size = null)
        {
            if (!_parameters.Exists(x => { return x.ParameterName == parameterName; }))
            {
                var parameter = new SqlParameter(parameterName, value);

                if (dbType != null)
                    parameter.SqlDbType = dbType.Value;

                if (size != null)
                    parameter.Size = size.Value;

                _parameters.Add(parameter);
            }
        }
    }
}
