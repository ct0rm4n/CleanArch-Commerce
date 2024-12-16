using Core.Entities.Domain.Catalog;
using Core.ViewModel.Catalog;
using Data.Commands.Data;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category, CategoryVM>
    {
        public CategoryRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
