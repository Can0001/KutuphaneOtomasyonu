using System;
using Core.Entities;

namespace Entities.Concrete
{
    public class Book : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public string? ImagePath { get; set; }

        public int? BorrowerId { get; set; }
        public string? BorrowedBy { get; set; }
        public DateTime? BorrowedDate { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool Status { get; set; } = true;
    }
}