using Core.Entities;
using System;

namespace KutüphaneOtomasyonu.Entities.Concrete
{
    public class BookTransaction : IEntity
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int StudentId { get; set; } 
        public int StaffId { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.Now; 
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; } 

        public string Status { get; set; }
    }
}