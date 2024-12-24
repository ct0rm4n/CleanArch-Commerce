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
        public async Task<User> GetCurrentUserByToken(string Token)
        {
            User user = new User();
            try
            {
                user = await userRepository.GetCurrentUserByToken(Token);
            }
            catch (Exception ex)
            {
            }
            return user;
        }
        public (bool,string) ValidateAuth(User? user_auth, string token)
        {
            if (user_auth is not null)
            {
                var validate_auth = authRepository.ValidateLogin(user_auth.Id).Result;
                if (token is not null && !validate_auth.Item1)
                {
                    return (false, null);
                }
                else if (user_auth is not null && validate_auth.Item1)
                {
                    return (true, validate_auth.Item2);
                }
                else
                {
                    return (false, null);
                }
            }
            else
            {
                var validate_auth = authRepository.ValidateLoginToken(token).Result;
                if (token is not null && validate_auth.Valid)
                {
                    return (true, token);
                }                
                else
                {
                    return (false, null);
                }
            }
            
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
