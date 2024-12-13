using Core.Entities.Domain.User;
using Core.ViewModel.User;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Repositories
{
    public class RoleRepository : GenericRepository<Role, RoleVM>
    {
        public RoleRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
