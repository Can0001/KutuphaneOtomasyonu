using Core.Entities;
using System;

namespace Entities.Concrete
{
    // Kendi sistemindeki interface adı neyse onu tut (IEntity vb.)
    public class BookTransaction : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public DateTime? ReturnDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}