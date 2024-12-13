using Core.Entities.Domain.Catalog;
using Core.ViewModel.Catalog;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Repositories
{
    public class CategoryRepository : GenericRepository<Category, CategoryVM>
    {
        public CategoryRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
