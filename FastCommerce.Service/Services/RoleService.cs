using Core.Entities.Domain.User;
using Core.ViewModel.User;
using Core.Wrappers;
using Data.Commands.Repositories;
using Data.Interfaces;
using Service.Helpers;

namespace Service.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleRepository _repository;

        public RoleService(RoleRepository repository) {
            _repository = repository;
        }


        public Role Add(Role banner)
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

        public List<Role> GetAll()
        {
            List<Role> banners = new List<Role>();
            try
            {
                banners = _repository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return banners;
        }

        public Role Get(int Id)
        {
            Role banner = new Role();
            try
            {
                banner = _repository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return banner;
        }

        public bool Update(Role banner)
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

        public bool Delete(Role banner)
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
        public PagedResponse<List<Role>> Search(PaginationFilter filter)
        {

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = new List<Role>();
            if (!string.IsNullOrEmpty(filter.SerachText))
            {
                data = (GetAll()).ToList().Where(f =>
                    f.Name.Contains(filter.SerachText, StringComparison.CurrentCultureIgnoreCase)
                    || f.Observation.Contains(filter.SerachText, StringComparison.CurrentCultureIgnoreCase)).ToList();
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
            var pagedReponse = PaginationHelper.CreatePagedReponse<Role>(pagedData, validFilter, totalRecords, "", "https://localhost:7152/Role/getall");
            return (pagedReponse);
        }
        public IList<string> GetValidation(RoleVM viewModel)
        {
            return _repository.GetValidationMessage(viewModel);
        }

        public IEnumerable<Role> BulkInsertReturn(IEnumerable<Role> entitys)
        {
            return _repository.BulkInsertReturn(entitys);
        }
    }
}
