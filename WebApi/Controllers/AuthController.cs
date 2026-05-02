using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService; // Sadece UserService var

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
            // 1. Kullanıcıyı bul (Öğrenci veya Admin fark etmez, hepsi User tablosunda)
            var user = _userService.GetByMail(userForLoginDto.Email);
            if (user == null)
            {
                return BadRequest("Kullanıcı bulunamadı!");
            }

            // 2. Şifre kontrolü
            if (user.PasswordHash != userForLoginDto.Password)
            {
                return BadRequest("Parola hatası!");
            }

            // 3. Herkes İçin Ortak Token Üretimi
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("KutuphaneOtomasyonu_Icin_Kirilamaz_Gizli_Anahtar_2026!?*");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.Role) // Next.js'in menü gizlemek için kullanacağı sütun!
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "library.com",
                Audience = "library.com"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Başarılı girişte Token ve Rol bilgisi dönüyor
            return Ok(new
            {
                Token = tokenHandler.WriteToken(token),
                User = user
            });
        }
    }
}