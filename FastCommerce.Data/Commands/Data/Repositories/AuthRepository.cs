using Core.Entities.Domain.User;
using Core.Entities.Enum;
using Core.ViewModel.Customer;
using Core.Wrappers;
using Dapper;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Data.Repositories
{
    public class AuthRepository : GenericRepository<AuthUser, UserVM>
    {
        private readonly string schema = "FastCommerce.dbo";
        public AuthRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public async Task InativeLogins(int userId)
        {
            try
            {
                string tableName = GetTableName();
                string query = $"UPDATE  {schema}.[{tableName}] SET IsActive = 0 WHERE [UserId] = '{userId}' AND Expiration < GETDATE() AND IsActive = 1 order by InsertedDate asc";
                var connection = this.GetConnection();
                connection.Open();
                var query_result = (connection.Query<dynamic>(query)).First();
                return;
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public async Task<(bool, string)> ValidateLogin(int userId)
        {
            try
            {
                string tableName = GetTableName();
                string query = $"select * from {schema}.[{tableName}] WHERE [UserId] = '{userId}' AND Expiration > GETDATE() AND IsActive = 1 order by InsertedDate desc";
                var connection = this.GetConnection();
                connection.Open();
                var result = connection.Query<AuthUser>(query);
                var query_result = (result).Count() > 0 ? true : false;
                return (query_result, result?.FirstOrDefault()?.Token ?? null);
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }
        public async Task<(bool Valid, string Token)> ValidateLoginToken(string token)
        {
            try
            {
                string tableName = GetTableName();
                string query = $"select * from {schema}.[{tableName}] WHERE [Token] = '{token}' AND Expiration >  GETDATE() AND IsActive = 1 order by InsertedDate desc";
                var connection = this.GetConnection();
                connection.Open();
                var result = connection.Query<AuthUser>(query);
                var query_result = (result).Count() > 0 ? true : false;
                return (query_result, result?.FirstOrDefault()?.Token ?? null);
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }
    }
}
