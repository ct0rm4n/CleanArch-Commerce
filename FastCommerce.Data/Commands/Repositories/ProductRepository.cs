using Core.Entities.Domain;
using Core.ViewModel.Catalog;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Repositories
{
    public class ProductRepository : GenericRepository<Product, ProductVM>
    {
        public ProductRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
