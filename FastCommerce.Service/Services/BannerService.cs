using Core.Entities.Domain.Banner;
using Core.ViewModel.Banner;
using Core.Wrappers;
using Data.Commands.Repositories;
using Data.Interfaces;
using Service.Helpers;

namespace Service
{
    public partial class BannerService : IBannerService
    {
        private readonly BannerRepository _repository = new BannerRepository();
                
        public Banner? Add(Banner banner)
        {
            var isAdded = false;
            try
            {
                var inserted = _repository.AddReturn(banner);
                isAdded = inserted.Item2;
                return inserted.Item1;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        public List<Banner> GetAll()
        {
            List<Banner> banners = new List<Banner>();
            try
            {
                banners = _repository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return banners;
        }

        public Banner Get(int Id)
        {
            Banner banner = new Banner();
            try
            {
                banner = _repository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return banner;
        }

        public bool Update(Banner banner)
        {
            bool isUpdated = false;
            try
            {
                isUpdated = _repository.Update(banner);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(Banner banner)
        {
            bool isDeleted = false;
            try
            {                
                isDeleted = _repository.Delete(banner);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
        public PagedResponse<List<Banner>> Search(PaginationFilter filter)
        {

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = new List<Banner>();
            if (!string.IsNullOrEmpty(filter.SerachText))
            {
                data = (GetAll()).ToList().Where(f =>
                    f.Title.Contains(filter.SerachText, StringComparison.CurrentCultureIgnoreCase)
                    || f.Title.Contains(filter.SerachText, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            else
            {
                data = (GetAll()).ToList();
            }
            var pagedData = data
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToList();

            var totalRecords = data.Count();
            var pagedReponse = PaginationHelper.CreatePagedReponse<Banner>(pagedData, validFilter, totalRecords, "", "https://localhost:7152/Banner/getall");
            return (pagedReponse);
        }
        public IList<string> GetValidation(BannerVM viewModel)
        {
            return _repository.GetValidationMessage(viewModel);
        }

        public IEnumerable<Banner> BulkInsertReturn(IEnumerable<Banner> entitys)
        {
            return _repository.BulkInsertReturn(entitys);
        }
    }
}