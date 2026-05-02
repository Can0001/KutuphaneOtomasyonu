using Business.Abstract;
using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll() { return Ok(_studentService.GetAll()); }

        [HttpGet("getbyid")]
        public IActionResult GetById(int id) { return Ok(_studentService.GetById(id)); }

        [HttpPost("add")]
        public IActionResult Add(Student student) { _studentService.Add(student); return Ok("Öğrenci eklendi."); }

        [HttpPost("update")]
        public IActionResult Update(Student student) { _studentService.Update(student); return Ok("Öğrenci güncellendi."); }

        [HttpPost("delete")]
        public IActionResult Delete(Student student) { _studentService.Delete(student); return Ok("Öğrenci silindi."); }
    }
}