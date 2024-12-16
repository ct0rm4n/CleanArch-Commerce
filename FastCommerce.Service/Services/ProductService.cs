using Core.Entities.Domain;
using Core.ViewModel.Catalog;
using Core.Wrappers;
using Data.Commands.Data.Repositories;
using Data.Interfaces;
using Service.Helpers;

namespace Service
{
    public partial class ProductService : IProductService
    {
        private readonly ProductRepository _repository;

        public ProductService(ProductRepository repository)
        {
            _repository = repository;
        }
                
        public Product? Add(Product banner)
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

        public List<Product> GetAll()
        {
            List<Product> banners = new List<Product>();
            try
            {
                banners = _repository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return banners;
        }

        public Product Get(int Id)
        {
            Product banner = new Product();
            try
            {
                banner = _repository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return banner;
        }

        public bool Update(Product banner)
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

        public bool Delete(Product banner)
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
        public PagedResponse<List<Product>> Search(PaginationFilter filter)
        {

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = new List<Product>();
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
            var pagedReponse = PaginationHelper.CreatePagedReponse<Product>(pagedData, validFilter, totalRecords, "", "https://localhost:7152/Produto/getall");
            return (pagedReponse);
        }
        public IList<string> GetValidation(ProductVM viewModel)
        {
            return _repository.GetValidationMessage(viewModel);
        }

        public IEnumerable<Product> BulkInsertReturn(IEnumerable<Product> entitys)
        {
            return _repository.BulkInsertReturn(entitys);
        }
    }
}