using Business.Abstract;
using Core.Utilities.Security.JWT;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper; 

        public AuthManager(IUserService userService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }

        public User Register(UserForRegisterDto userForRegisterDto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userForRegisterDto.Password);

            var user = new User
            {
                Email = userForRegisterDto.Email,
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                PasswordHash = passwordHash,
                Role = "Ogrenci",
                Status = true
            };

            _userService.Add(user);
            return user;
        }

        public AccessToken Login(UserForLoginDto userForLoginDto)
        {
            var userToCheck = _userService.GetByEmail(userForLoginDto.Email);
            if (userToCheck == null)
            {
                throw new Exception("Kullanıcı bulunamadı!");
            }

            if (!BCrypt.Net.BCrypt.Verify(userForLoginDto.Password, userToCheck.PasswordHash))
            {
                throw new Exception("Şifre hatalı!");
            }

            
            return _tokenHelper.CreateToken(
                userToCheck.Id.ToString(),
                userToCheck.Email,
                userToCheck.Role,
                userToCheck.FirstName);
        }

        public bool UserExists(string email)
        {
            if (_userService.GetByEmail(email) != null)
            {
                return true;
            }
            return false;
        }
    }
}