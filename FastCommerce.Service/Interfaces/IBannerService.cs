using Core.Entities.Domain.Banner;
using Core.ViewModel.Banner;
using Core.Wrappers;

namespace Data.Interfaces
{
    public partial interface IBannerService
    {
        Banner? Add(Banner blogPost);
        List<Banner> GetAll();
        bool Update(Banner user);
        bool Delete(Banner user);
        Banner Get(int Id);
        PagedResponse<List<Banner>> Search(PaginationFilter filter);
        public IList<string> GetValidation(BannerVM viewModel);
        public IEnumerable<Banner> BulkInsertReturn(IEnumerable<Banner> entitys);
    }
}
