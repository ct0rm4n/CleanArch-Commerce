using Core.Entities.Domain.User;
using Core.Entities.Enum;
using Data.Commands.Data.Repositories;
using Data.Interfaces;
namespace Service
{
    public class UserService : IUserService
    {
        private readonly UserRepository userRepository;
        private readonly AuthRepository authRepository;


        public UserService(UserRepository userRepository, AuthRepository authRepository) {
            this.userRepository = userRepository;
            this.authRepository = authRepository;
        }
        public User Login(string username, string password)
        {
            User user = new User();
            try
            {
                
                user = userRepository.Login(username, password);
            }
            catch (Exception ex)
            {
            }
            return user;
        }
        public bool ValidateAuth(User? user_auth, string token)
        {
            if (token is not null && !authRepository.ValidateLoginToken(token).Result)
            {
                return false;
            }
            else if (user_auth is not null && !authRepository.ValidateLogin(user_auth.Id).Result)
            {
                return false;
            }
            return true;
        }

        public User Auth(User user, string token, DateTime expiration)
        {
            
            try
            {
                AuthUser user_auth = new AuthUser()
                {
                    UserId = user.Id,
                    Token = token,
                    InsertedDate = DateTime.Now,
                    Expiration = expiration,
                    isActive = true,
                    Status = StatusEntity.Inserted,
                    LastModifiedDate = DateTime.Now
                };
                authRepository.InativeLogins(user_auth.UserId);                
                var logged = authRepository.AddReturn(user_auth);               
                
            }
            catch (Exception ex)
            {
            }
            return user;
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
