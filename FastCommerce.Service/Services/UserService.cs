using Core.Entities.Domain;
using Data.Commands.Repositories;
using Data.Interfaces;
namespace Service
{
    public class UserService : IUserService
    {
        public bool Add(User user)
        {
            bool isAdded = false;
            try
            {
                UserRepository userRepository = new UserRepository();
                isAdded = userRepository.Add(user);
            }
            catch (Exception ex)
            {
            }
            return isAdded;
        }

        public List<User> GetAll()
        {
            List<User> users = new List<User>();
            try
            {
                UserRepository userRepository = new UserRepository();
                users = userRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return users;
        }

        public User Get(int Id)
        {
            User user = new User();
            try
            {
                UserRepository userRepository = new UserRepository();
                user = userRepository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return user;
        }

        public bool Update(User user)
        {
            bool isUpdated = false;
            try
            {
                UserRepository userRepository = new UserRepository();
                isUpdated = userRepository.Update(user);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(User user)
        {
            bool isDeleted = false;
            try
            {
                UserRepository userRepository = new UserRepository();
                isDeleted = userRepository.Delete(user);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }

    }
}
