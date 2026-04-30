using Business.Abstract;
using Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            // Önce bu email daha önce alınmış mı kontrol et
            if (_authService.UserExists(userForRegisterDto.Email))
            {
                return BadRequest("Bbu email zaten sistemde var!");
            }

            var registerResult = _authService.Register(userForRegisterDto);
            return Ok("Kayıt başarılı! Hoş geldiniz.");
        }

        [HttpPost("login")]
        public IActionResult Login(UserForLoginDto userForLoginDto)
        {
            try
            {
                // Artık Login metodu bize bir AccessToken (Sanal Kart) veriyor
                var token = _authService.Login(userForLoginDto);
                return Ok(token); // Bu kartı doğrudan ekrana basıyoruz
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}