
namespace Service.Interfaces
{
    public partial interface IAuthService
    {

        (string Token, bool Logged) Login(string username, string password);
        (string Token, bool Logged) LoginValidate(string token);

    }
}
