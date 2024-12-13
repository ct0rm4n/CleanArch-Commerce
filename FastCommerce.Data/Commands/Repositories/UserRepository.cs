using Core.Entities.Domain.User;
using Core.ViewModel.Customer;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Repositories
{
    public class UserRepository : GenericRepository<User, UserVM>
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
