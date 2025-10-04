using System.ComponentModel;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;

namespace MCPServer1.Tools
{
    [McpServerToolType]
    public class SessionTool
    {

        private readonly string _connectionString;
        public SessionTool(IOptions<AppConfig> appConfig)
        {
            _connectionString = appConfig?.Value?.ConnectionString ?? "";
        }

        public async Task<int> GetUserId (int userId = 0, string userName = "")
        {
            using var connection = new SqlConnection(_connectionString);
            //await connection.OpenAsync();
            if (userId != 0)
            {
                var userNo = await connection.QueryFirstOrDefaultAsync<int>("SELECT COUNT(1) FROM Users WHERE UserId = @UserId", new { UserId = userId });
                if (userNo > 0)
                {
                    return userId;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    var user = await connection.QueryFirstOrDefaultAsync<int>("SELECT UserId FROM Users WHERE FullName like @FullName", new { FullName = $"%{userName}%" });
                    if (user != 0)
                    {
                        return user;
                    }
                }
            }
            return 0;
        }

        [McpServerTool, Description("Starts a chat session. It can accept both username or userID")]
        public async Task<Guid> StartSession ([Description("The userId")] int? Id, [Description("The username")]  string userName = "")
        {
            var userId = await GetUserId(Id ?? 0, userName);

            using var connection = new SqlConnection(_connectionString);
            var sessionId = Guid.Empty;
            try
            {
                var sql = @"
                        INSERT INTO UserSession (UserId, SessionStartTime)
                        OUTPUT INSERTED.UserSessionId
                        VALUES (@UserId, SYSDATETIMEOFFSET());
                    ";
                sessionId = await connection.ExecuteScalarAsync<Guid>(sql, new { UserId = userId });
            }
            catch (Exception)
            {
                
            }

            return sessionId;
        }

    }
}
