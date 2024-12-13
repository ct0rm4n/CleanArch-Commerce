using Core.Entities.Domain.Banner;
using Core.ViewModel.Banner;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Repositories
{
    public class BannerRepository : GenericRepository<Banner, BannerVM>
    {
        public BannerRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
