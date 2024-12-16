using Core.Entities.Domain.User;
using Data.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Service.Interfaces;

namespace Service.Services
{
    public partial class AuthService : IAuthService
    {
        private readonly IMemoryCache cache;
        private readonly IUserService _userService;

        public AuthService(IUserService userService)
        {
            cache = new MemoryCache(new MemoryCacheOptions());
            _userService = userService;
        }

        public (string Token, bool Logged)  Login(string username, string password)
        {
            var auth_user = Authenticate(username, password);
            if (auth_user.valid)
            {
                if(_userService.ValidateAuth(auth_user.user, null))
                    return (null, true);

                string token = GenerateToken();
                DateTime expiration = DateTime.Now.AddMinutes(30);
                cache.Set(username, (token, expiration), expiration);
                if (IsTokenValid(username))
                {
                    _userService.Auth(auth_user.user, token, expiration);
                    return (token,true);
                }
                else
                {
                    return (null,false);
                }
            }
            else
            {
                return (null, false);
            }
        }
        public (string Token, bool Logged) LoginValidate(string token)
        {
            if (_userService.ValidateAuth(null,token))
                return (null, true);
            else
                return (null, false);
        }
        private (bool valid, User user) Authenticate(string username, string password)
        {
            var login  = _userService.Login(username, password);
            return (login is not null) ? (true, login) : (false, null);
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }

        private bool IsTokenValid(string username)
        {
            if (cache.TryGetValue(username, out (string Token, DateTime Expiration) session))
            {
                return DateTime.Now <= session.Expiration;
            }
            return false;
        }

    }
}
