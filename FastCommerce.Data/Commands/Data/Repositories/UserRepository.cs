using Core.Entities.Domain.User;
using Core.Entities.Enum;
using Core.ViewModel.Customer;
using Dapper;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Data.Repositories
{
    public class UserRepository : GenericRepository<User, UserVM>
    {
        private readonly string schema = "FastCommerce.dbo";
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }


        public User? Login(string username, string password)
        {
            try
            {
                string tableName = GetTableName();
                string query = $"SELECT * FROM  {schema}.[{tableName}] WHERE [Email] = '{username}' AND [Password] = '{password}'";
                var connection = this.GetConnection();
                connection.Open();
                var query_result = (connection.Query<dynamic>(query)).First();
                if (query_result == null || query_result.Count() == 0) return null;
                var user = new User()
                {
                    FirstName = query_result.FirstName,
                    LastName = query_result.LastName,
                    Status = (StatusEntity)query_result.Status,
                    Email = query_result.Email,
                    Password = query_result.Password,
                    Id = query_result.Id
                };
                return user;
            }
            catch (Exception ex) {
                return null;
            }
        }
        public async Task<User> GetCurrentUserByToken(string Token)
        {
            try
            {
                string tableName = GetTableName();
                string query = $"select u.* from {schema}.[AuthUser] as auth_table " +
                    $"left join [User] u on auth_table.UserId = u.Id " +
                    $"WHERE auth_table.Token = '{Token}' AND " +
                    $"auth_table.Expiration > GETDATE() AND auth_table.IsActive = 1";

                var connection = this.GetConnection();
                connection.Open();
                var result = connection.Query<User>(query);
                var query_result = (result).Count() > 0 ? true : false;
                return result?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
