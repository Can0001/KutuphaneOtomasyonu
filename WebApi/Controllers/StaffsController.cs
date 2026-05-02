using Business.Abstract;
using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffsController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll() { return Ok(_staffService.GetAll()); }

        [HttpGet("getbyid")]
        public IActionResult GetById(int id) { return Ok(_staffService.GetById(id)); }

        [HttpPost("add")]
        public IActionResult Add(Staff staff) { _staffService.Add(staff); return Ok("Personel başarıyla eklendi."); }

        [HttpPost("update")]
        public IActionResult Update(Staff staff) { _staffService.Update(staff); return Ok("Personel güncellendi."); }

        [HttpPost("delete")]
        public IActionResult Delete(Staff staff) { _staffService.Delete(staff); return Ok("Personel silindi."); }
    }
}