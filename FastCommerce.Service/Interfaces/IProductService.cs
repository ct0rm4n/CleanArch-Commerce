using Core.Entities.Domain;
using Core.ViewModel.Catalog;
using Core.Wrappers;

namespace Data.Interfaces
{
    public partial interface IProductService
    {
        Product? Add(Product blogPost);
        List<Product> GetAll();
        bool Update(Product user);
        bool Delete(Product user);
        Product Get(int Id);
        PagedResponse<List<Product>> Search(PaginationFilter filter);
        public IList<string> GetValidation(ProductVM viewModel);
        public IEnumerable<Product> BulkInsertReturn(IEnumerable<Product> entitys);
    }
}
