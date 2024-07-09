using Core.Entities.Abstract;
using Core.ViewModel.Generic.Abstracts;
using Core.Wrappers;

namespace Data.Interfaces
{
    public partial interface ISettingsService
    {
        Settings? Add(Settings blogPost);
        List<Settings> GetAll();
        bool Update(Settings user);
        bool Delete(Settings user);
        Settings Get(int Id);
        PagedResponse<List<Settings>> Search(PaginationFilter filter);
        public IList<string> GetValidation(SettingsVM viewModel);
        public IEnumerable<Settings> BulkInsertReturn(IEnumerable<Settings> entitys);
    }
}
