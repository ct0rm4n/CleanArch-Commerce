using Core.Entities.Domain.User;
using Core.ViewModel.User;
using Core.Wrappers;

namespace Data.Interfaces
{
    public interface IRoleService
    {
        Role? Add(Role user);
        List<Role> GetAll();
        Role Get(int Id);
        bool Update(Role user);
        bool Delete(Role user);
        PagedResponse<List<Role>> Search(PaginationFilter filter);
        public IList<string> GetValidation(RoleVM viewModel);
        public IEnumerable<Role> BulkInsertReturn(IEnumerable<Role> entitys);
    }
}
