using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Security.Cryptography; 
namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            if (_authService.UserExists(userForRegisterDto.Email))
            {
                return BadRequest("Bu email zaten sistemde var!");
            }

            var registerResult = _authService.Register(userForRegisterDto);
            return Ok("Kayıt başarılı! Hoş geldiniz.");
        }

        [HttpPost("login")]
        public IActionResult Login(UserForLoginDto userForLoginDto)
        {
            var user = _userService.GetByMail(userForLoginDto.Email);
            if (user == null)
            {
                return BadRequest("Kullanıcı bulunamadı!");
            }

            string hashedInputPassword = CreatePasswordHash(userForLoginDto.Password);

            if (user.PasswordHash != hashedInputPassword)
            {
                return BadRequest("Erişim Reddedildi: Parola hatası!");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("KutuphaneOtomasyonu_Icin_Kirilamaz_Gizli_Anahtar_2026!?*");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.Role) 
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "library.com",
                Audience = "library.com"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                Token = tokenHandler.WriteToken(token),
                User = user
            });
        }

        private string CreatePasswordHash(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}