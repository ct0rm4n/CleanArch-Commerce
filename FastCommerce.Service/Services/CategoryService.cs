using Core.Entities.Domain.Catalog;
using Core.ViewModel.Catalog;
using Core.Wrappers;
using Data.Commands.Repositories;
using Data.Interfaces;
using Service.Helpers;

namespace Service.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly CategoryRepository _repository = new CategoryRepository();
        public Category? Add(Category model)
        {
            var isAdded = false;
            try
            {
                var inserted = _repository.AddReturn(model);
                isAdded = inserted.Item2;
                return inserted.Item1;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool Update(Category model)
        {
            bool isUpdated = false;
            try
            {
                isUpdated = _repository.Update(model);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }
        public bool Delete(Category category)
        {
            bool isDeleted = false;
            try
            {
                isDeleted = _repository.Delete(category);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }

        public Category Get(int Id)
        {
            Category model = new Category();
            try
            {
                model = _repository.GetById(Id);
            }
            catch (Exception ex)
            {
            }
            return model;
        }

        public List<Category> GetAll()
        {
            List<Category> list = new List<Category>();
            try
            {
                list = _repository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return list;
        }


        public PagedResponse<List<Category>> Search(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = new List<Category>();
            if (!string.IsNullOrEmpty(filter.SerachText))
            {
                data = (GetAll()).ToList().Where(f =>
                    f.Name.Contains(filter.SerachText, StringComparison.CurrentCultureIgnoreCase)
                    || f.Html.Contains(filter.SerachText, StringComparison.CurrentCultureIgnoreCase)).ToList();
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
            var pagedReponse = PaginationHelper.CreatePagedReponse<Category>(pagedData, validFilter, totalRecords, "", "https://localhost:7152/BlogPost/getall");
            return (pagedReponse);
        }

        public IList<string> GetValidation(CategoryVM viewModel)
        {
            return _repository.GetValidationMessage(viewModel);
        }
    }
}
