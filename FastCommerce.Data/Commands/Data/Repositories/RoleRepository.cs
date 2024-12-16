using Core.Entities.Domain.User;
using Core.ViewModel.User;
using Data.Commands.Data;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Data.Repositories
{
    public class RoleRepository : GenericRepository<Role, RoleVM>
    {
        public RoleRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
