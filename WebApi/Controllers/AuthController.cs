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
        private readonly IStaffService _staffService;

        // İki servisi de Constructor'da içeri aldık
        public AuthController(IAuthService authService, IStaffService staffService)
        {
            _authService = authService;
            _staffService = staffService;
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
            // 1. SENARYO: Acaba giriş yapmaya çalışan kişi bir PERSONEL mi?
            var staff = _staffService.GetByEmail(userForLoginDto.Email);
            if (staff != null)
            {
                if (staff.PasswordHash != userForLoginDto.Password)
                {
                    return BadRequest("Parola hatası!");
                }

                // Personel için "Rol" barındıran özel token üretimi
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes("KutuphaneOtomasyonu_Icin_Kirilamaz_Gizli_Anahtar_2026!?*");

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, staff.Id.ToString()),
                        new Claim(ClaimTypes.Email, staff.Email),
                        new Claim(ClaimTypes.Name, $"{staff.FirstName} {staff.LastName}"),
                        new Claim(ClaimTypes.Role, staff.Role) // Next.js'in aradığı kritik rol verisi
                    }),
                    Expires = DateTime.UtcNow.AddHours(4),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = "library.com",
                    Audience = "library.com"
                };

                var staffToken = tokenHandler.CreateToken(tokenDescriptor);

                // Personel girişi başarılı, Next.js'e IsStaff=true ile birlikte dönüyoruz
                return Ok(new
                {
                    Token = tokenHandler.WriteToken(staffToken),
                    User = staff,
                    IsStaff = true
                });
            }

            // 2. SENARYO: Personel değilse, normal ÖĞRENCİ/ÜYE girişini dene
            try
            {
                var userToken = _authService.Login(userForLoginDto);
                return Ok(new
                {
                    Token = userToken.Token, // Sende token objesi dönüyorsa property'sine göre ayarla (örn: userToken.Token)
                    IsStaff = false
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}