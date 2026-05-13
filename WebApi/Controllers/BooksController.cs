using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization; 
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IBookTransactionService _transactionService;

        public BooksController(IBookService bookService, IWebHostEnvironment webHostEnvironment, IBookTransactionService transactionService)
        {
            _bookService = bookService;
            _webHostEnvironment = webHostEnvironment;
            _transactionService = transactionService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll() => Ok(_bookService.GetAll());

        [HttpGet("getbyid/{id}")]
        public IActionResult GetById(int id)
        {
            var result = _bookService.GetById(id);
            if (result == null) return NotFound("Eser bulunamadı.");
            return Ok(result);
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin,Personel")] 
        public async Task<IActionResult> Add([FromForm] BookAddDto bookDto)
        {
            string? imagePath = null;
            if (bookDto.Image != null && bookDto.Image.Length > 0)
            {
                imagePath = await SaveImage(bookDto.Image);
            }

            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                ISBN = bookDto.ISBN,
                PageCount = bookDto.PageCount,
                ImagePath = imagePath,
                IsAvailable = true,
                Status = true
            };

            _bookService.Add(book);
            return Ok(new { message = "Eser başarıyla eklendi." });
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin,Personel")] 
        public async Task<IActionResult> Update([FromForm] BookUpdateDto bookDto)
        {
            var existingBook = _bookService.GetById(bookDto.Id);
            if (existingBook == null) return NotFound("Güncellenecek eser bulunamadı.");

            if (bookDto.Image != null && bookDto.Image.Length > 0)
            {
                existingBook.ImagePath = await SaveImage(bookDto.Image);
            }

            existingBook.Title = bookDto.Title;
            existingBook.Author = bookDto.Author;
            existingBook.ISBN = bookDto.ISBN;
            existingBook.PageCount = bookDto.PageCount;

            _bookService.Update(existingBook);
            return Ok(new { message = "Eser başarıyla güncellendi." });
        }

        [HttpPost("changestatus")]
        [Authorize(Roles = "Admin,Personel")] 
        public IActionResult ChangeStatus(int id)
        {
            try
            {
                var book = _bookService.GetById(id);
                if (book == null) return NotFound("Kitap bulunamadı!");

                var isBorrowed = _transactionService.GetAll()
                    .Any(t => t.BookId == id && (t.Status == "Approved" || t.Status == "Overdue"));

                if (isBorrowed)
                {
                    return BadRequest("Hata: Bu kitap şu an bir öğrencide ödünçte veya gecikmiş durumda olduğu için pasife alınamaz!");
                }

                book.Status = !book.Status;
                _bookService.Update(book);

                return Ok(new { message = "Kitap durumu güncellendi!", newStatus = book.Status });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("borrow/{id}")]
        [Authorize(Roles = "Admin,Personel")]
        public IActionResult Borrow(int id, [FromBody] BorrowRequest request)
        {
            try
            {
                _bookService.BorrowBook(id, request.UserId, request.UserName);
                return Ok(new { message = "Kitap başarıyla teslim edildi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("return/{id}")]
        [Authorize(Roles = "Admin,Personel")]
        public IActionResult Return(int id)
        {
            _bookService.ReturnBook(id);
            return Ok(new { message = "Kitap iade alındı." });
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")] 
        public IActionResult Delete(int id)
        {
            var book = _bookService.GetById(id);
            if (book == null) return NotFound("Silinecek eser bulunamadı.");

            var isBorrowed = _transactionService.GetAll()
                .Any(t => t.BookId == id && (t.Status == "Approved" || t.Status == "Overdue"));

            if (isBorrowed)
            {
                return BadRequest("Hata: Bu kitap şu an bir öğrencide olduğu için sistemden silinemez! Önce iade alınmalıdır.");
            }

            try
            {
                _bookService.Delete(book);
                return Ok(new { message = "Eser başarıyla kaldırıldı." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            string wwwrootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadDir = Path.Combine(wwwrootPath, "images");

            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return "/images/" + fileName;
        }
    }

    public class BorrowRequest
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

    public class BookAddDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class BookUpdateDto : BookAddDto
    {
        public int Id { get; set; }
    }
}