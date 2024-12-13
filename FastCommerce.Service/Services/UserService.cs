using Core.Entities.Domain.User;
using Data.Commands.Repositories;
using Data.Interfaces;
namespace Service
{
    public class UserService : IUserService
    {
        private readonly UserRepository userRepository;


        public UserService(UserRepository userRepository) {
            this.userRepository = userRepository;
        }

        public bool Add(User user)
        {
            bool isAdded = false;
            try
            {
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
                isDeleted = userRepository.Delete(user);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }

    }
}
