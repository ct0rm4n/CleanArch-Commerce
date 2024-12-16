using Core.Entities.Domain.User;

namespace Data.Interfaces
{
    public interface IUserService 
    {
        bool Add(User user);
        List<User> GetAll();
        User? Login(string email, string password);
        bool ValidateAuth(User user_auth, string token);
        User Get(int Id);
        bool Update(User user);
        bool Delete(User user);
        User Auth(User user, string token, DateTime expiration);
    }
}
