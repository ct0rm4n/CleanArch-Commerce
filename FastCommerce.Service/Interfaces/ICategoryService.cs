using Core.Entities.Domain.Catalog;
using Core.ViewModel.Catalog;
using Core.Wrappers;

namespace Data.Interfaces
{
    public partial interface ICategoryService
    {
        Category? Add(Category blogPost);
        List<Category> GetAll();
        bool Update(Category user);
        bool Delete(Category user);
        Category Get(int Id);
        PagedResponse<List<Category>> Search(PaginationFilter filter);
        public IList<string> GetValidation(CategoryVM viewModel);
    }
}
