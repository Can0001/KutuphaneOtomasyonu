using Business.Abstract;
using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using System; 

namespace WebApi.Controllers
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
        public IActionResult GetAll()
        {
            var result = _bookTransactionService.GetAll();
            return Ok(result);
        }

        [HttpGet("getbyid")]
        public IActionResult GetById(int id)
        {
            var result = _bookTransactionService.GetById(id);
            return Ok(result);
        }

        [HttpPost("add")]
        public IActionResult Add(BookTransaction bookTransaction)
        {
            _bookTransactionService.Add(bookTransaction);
            return Ok("İşlem eklendi.");
        }

        [HttpPost("update")]
        public IActionResult Update(BookTransaction bookTransaction)
        {
            _bookTransactionService.Update(bookTransaction);
            return Ok("İşlem güncellendi.");
        }

        [HttpPost("delete")]
        public IActionResult Delete(BookTransaction bookTransaction)
        {
            _bookTransactionService.Delete(bookTransaction);
            return Ok("İşlem silindi.");
        }

        [HttpGet("getpending")]
        public IActionResult GetPendingRequests()
        {
            var result = _bookTransactionService.GetPendingRequests();
            return Ok(result);
        }


        [HttpPost("requestbook")]
        public IActionResult RequestBook(BookTransaction bookTransaction)
        {
            try
            {
                _bookTransactionService.RequestBook(bookTransaction);
                return Ok(new { Message = "Kitap talebiniz başarıyla alındı, kütüphaneci onayı bekleniyor!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("approve")]
        public IActionResult Approve(int id)
        {
            try
            {
                _bookTransactionService.ApproveRequest(id);
                return Ok(new { Message = "Öğrencinin kitap talebi onaylandı!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reject")]
        public IActionResult Reject(int id)
        {
            _bookTransactionService.RejectRequest(id);
            return Ok(new { Message = "Öğrencinin kitap talebi reddedildi." });
        }

        [HttpPost("return")]
        public IActionResult ReturnBook(int id)
        {
            try
            {
                _bookTransactionService.ReturnBook(id);
                return Ok(new { Message = "Kitap başarıyla iade alındı!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyuser")]
        public IActionResult GetByUser(int userId)
        {
            var result = _bookTransactionService.GetByUserId(userId);
            return Ok(result);
        }

        [HttpPost("returnbybook")]
        public IActionResult ReturnByBook(int bookId)
        {
            try
            {
                _bookTransactionService.ReturnByBookId(bookId);
                return Ok(new { Message = "Kitap başarıyla iade alındı ve rafa eklendi!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("assignbook")]
        public IActionResult AssignBook(BookTransaction transaction)
        {
            try
            {
                transaction.Status = "Approved"; 
                transaction.TransactionDate = DateTime.Now; 
                _bookTransactionService.Add(transaction);
                return Ok(new { Message = "Kitap başarıyla öğrenciye ödünç verildi!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}