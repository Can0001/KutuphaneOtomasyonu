using Core.Entities;

namespace Entities.Concrete
{
    public class Reservation : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; } 
        public DateTime ReturnDate { get; set; }
        public bool IsReturned { get; set; }
    }
}