using Business.Abstract;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IBookTransactionService _transactionService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;

        public ReportsController(IBookTransactionService transactionService, IBookService bookService, IUserService userService)
        {
            _transactionService = transactionService;
            _bookService = bookService;
            _userService = userService;
        }

        [HttpGet("getstats")]
        public IActionResult GetStats()
        {
            var transactions = _transactionService.GetAll();

            var topBookGroup = transactions.GroupBy(t => t.BookId)
                                           .OrderByDescending(g => g.Count())
                                           .FirstOrDefault();

            string topBookName = "-"; 
            if (topBookGroup != null)
            {
                var book = _bookService.GetById(topBookGroup.Key);
                topBookName = book != null ? book.Title : "Bilinmeyen Kitap";
            }

            var stats = new
            {
                TotalBooks = _bookService.GetAll().Count,
                ActiveBorrows = transactions.Count(t => t.Status == "Approved" || t.Status == "Overdue"),
                OverdueBooks = transactions.Count(t => t.Status == "Overdue"),
                TotalStudents = _userService.GetAll().Count(u => u.Role == "Ogrenci"),
                TopBook = topBookName 
            };
            return Ok(stats);
        }

        [HttpGet("getdetails")]
        public IActionResult GetDetails()
        {
            var transactions = _transactionService.GetAll();
            var result = transactions.Select(t => {
                var user = _userService.GetById(t.UserId);
                var book = _bookService.GetById(t.BookId);
                return new
                {
                    Id = t.Id,
                    BookName = book != null ? book.Title : "Silinmiş Kitap",
                    UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Silinmiş Kullanıcı", 
                    TransactionDate = t.TransactionDate,
                    ReturnDate = t.ReturnDate,
                    DueDate = t.DueDate,
                    Status = t.Status
                };
            }).OrderByDescending(t => t.TransactionDate).ToList();

            return Ok(result);
        }

        [HttpGet("exportexcel")]
        public IActionResult ExportExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Kitap Raporu");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "İşlem ID";
                worksheet.Cell(currentRow, 2).Value = "Kitap Adı";
                worksheet.Cell(currentRow, 3).Value = "Öğrenci Adı";
                worksheet.Cell(currentRow, 4).Value = "Durum";
                worksheet.Cell(currentRow, 5).Value = "Teslim Tarihi";

                var data = _transactionService.GetAll();
                foreach (var item in data)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Id;
                    worksheet.Cell(currentRow, 2).Value = _bookService.GetById(item.BookId)?.Title;
                    var user = _userService.GetById(item.UserId);
                    worksheet.Cell(currentRow, 3).Value = $"{user?.FirstName} {user?.LastName}";
                    worksheet.Cell(currentRow, 4).Value = item.Status;
                    worksheet.Cell(currentRow, 5).Value = item.DueDate.ToShortDateString();
                }

                worksheet.Range("A1:E1").Style.Font.Bold = true;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Kutuphane_Rapor.xlsx");
                }
            }
        }
    }
}