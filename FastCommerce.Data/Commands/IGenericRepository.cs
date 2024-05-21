
using Core.ViewModel.Generic.Abstracts;

namespace Data.Commands
{
    public interface IGenericRepository<T, IBaseVM>
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        bool Add(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        IList<string> GetValidationMessage(IBaseVM viewModel);
    }
}
