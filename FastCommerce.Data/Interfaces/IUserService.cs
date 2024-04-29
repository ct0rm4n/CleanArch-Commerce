using FastCommerce.Core.Entities.Domain;

namespace FastCommerce.Data.Interfaces
{
    public interface IUserService 
    {
        bool Add(User user);
        List<User> GetAll();
        User Get(int Id);
        bool Update(User user);
        bool Delete(User user);        
    }
}
