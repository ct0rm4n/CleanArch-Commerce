using Core.Entities.Abstract;
using Core.ViewModel.Generic.Abstracts;
using Core.Wrappers;
using Data.Commands.Repositories;
using Data.Interfaces;
using Service.Helpers;

namespace Service
{
    public partial class SettingsService : ISettingsService
    {
        private readonly SettingsRepository _repository = new SettingsRepository();
                
        public Settings? Add(Settings Settings)
        {
            var isAdded = false;
            try
            {
                var inserted = _repository.AddReturn(Settings);
                isAdded = inserted.Item2;
                return inserted.Item1;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        public List<Settings> GetAll()
        {
            List<Settings> Settingss = new List<Settings>();
            try
            {
                Settingss = _repository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return Settingss;
        }

        public Settings Get(int Id)
        {
            Settings Settings = new Settings();
            try
            {
                Settings = _repository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return Settings;
        }

        public bool Update(Settings Settings)
        {
            bool isUpdated = false;
            try
            {
                isUpdated = _repository.Update(Settings);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(Settings Settings)
        {
            bool isDeleted = false;
            try
            {                
                isDeleted = _repository.Delete(Settings);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
        public PagedResponse<List<Settings>> Search(PaginationFilter filter)
        {

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = new List<Settings>();
            if (!string.IsNullOrEmpty(filter.SerachText))
            {
                data = (GetAll()).ToList().Where(f =>
                    f.Body.Contains(filter.SerachText, StringComparison.CurrentCultureIgnoreCase)
                    || f.Body.Contains(filter.SerachText, StringComparison.CurrentCultureIgnoreCase)).ToList();
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
            var pagedReponse = PaginationHelper.CreatePagedReponse<Settings>(pagedData, validFilter, totalRecords, "", "https://localhost:7152/Settings/getall");
            return (pagedReponse);
        }
        public IList<string> GetValidation(SettingsVM viewModel)
        {
            return _repository.GetValidationMessage(viewModel);
        }

        public IEnumerable<Settings> BulkInsertReturn(IEnumerable<Settings> entitys)
        {
            return _repository.BulkInsertReturn(entitys);
        }
    }
}