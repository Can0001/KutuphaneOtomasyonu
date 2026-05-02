using Business.Abstract;
using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookTransactionsController : ControllerBase
    {
        private readonly IBookTransactionService _bookTransactionService;

        public BookTransactionsController(IBookTransactionService bookTransactionService)
        {
            _bookTransactionService = bookTransactionService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll() { return Ok(_bookTransactionService.GetAll()); }

        [HttpGet("getbyid")]
        public IActionResult GetById(int id) { return Ok(_bookTransactionService.GetById(id)); }

        [HttpPost("add")]
        public IActionResult Add(BookTransaction bookTransaction) { _bookTransactionService.Add(bookTransaction); return Ok("Ödünç işlemi başarıyla kaydedildi."); }

        [HttpPost("update")]
        public IActionResult Update(BookTransaction bookTransaction) { _bookTransactionService.Update(bookTransaction); return Ok("İşlem detayı güncellendi."); }

        [HttpPost("delete")]
        public IActionResult Delete(BookTransaction bookTransaction) { _bookTransactionService.Delete(bookTransaction); return Ok("İşlem kaydı sistemden silindi."); }
    }
}