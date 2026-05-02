using Business.Abstract;
using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll() { return Ok(_categoryService.GetAll()); }

        [HttpGet("getbyid")]
        public IActionResult GetById(int id) { return Ok(_categoryService.GetById(id)); }

        [HttpPost("add")]
        public IActionResult Add(Category category) { _categoryService.Add(category); return Ok("Kategori başarıyla eklendi."); }

        [HttpPost("update")]
        public IActionResult Update(Category category) { _categoryService.Update(category); return Ok("Kategori başarıyla güncellendi."); }

        [HttpPost("delete")]
        public IActionResult Delete(Category category) { _categoryService.Delete(category); return Ok("Kategori başarıyla silindi."); }
    }
}