using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _userService.GetAll();
            return Ok(result);
        }

        [HttpPost("add")]
        public IActionResult Add(User user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    user.PasswordHash = CreatePasswordHash(user.PasswordHash);
                }

                _userService.Add(user);
                return Ok(new { Message = "Kullanıcı başarıyla sisteme eklendi!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getstudents")]
        public IActionResult GetStudents()
        {
            var students = _userService.GetAll().Where(u => u.Role == "Ogrenci").ToList();
            return Ok(students);
        }

        [HttpGet("getbyrole")]
        public IActionResult GetByRole(string role)
        {
            var result = _userService.GetAllByRole(role);
            return Ok(result);
        }

        [HttpPost("changestatus")]
        public IActionResult ChangeStatus(int id)
        {
            try
            {
                var user = _userService.GetAll().FirstOrDefault(u => u.Id == id);

                if (user == null)
                {
                    return BadRequest("Kullanıcı bulunamadı!");
                }

                user.Status = !user.Status;

                _userService.Update(user);

                return Ok(new { Message = "Kullanıcı durumu başarıyla güncellendi!", NewStatus = user.Status });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update")]
        public IActionResult Update([FromBody] UserUpdateDto userDto)
        {
            try
            {
                var existingUser = _userService.GetAll().FirstOrDefault(u => u.Id == userDto.Id);

                if (existingUser == null)
                {
                    return BadRequest("Güncellenecek kullanıcı bulunamadı!");
                }

                existingUser.FirstName = userDto.FirstName;
                existingUser.LastName = userDto.LastName;
                existingUser.Email = userDto.Email;
                existingUser.Role = userDto.Role;

                _userService.Update(existingUser);

                return Ok(new { Message = "Kullanıcı rolü ve bilgileri başarıyla güncellendi!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}