using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq; 

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
            _userService.Add(user);
            return Ok("Kullanıcı başarıyla sisteme eklendi!");
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
    }
}