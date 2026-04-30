using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAuthService
    {
        User Register(UserForRegisterDto userForRegisterDto);
        AccessToken Login(UserForLoginDto userForLoginDto); // Geriye Token döneceğini belirttik
        bool UserExists(string email);
    }
}