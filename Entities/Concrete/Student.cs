using Core.Entities;
using System;

namespace KutüphaneOtomasyonu.Entities.Concrete
{
    public class Student : IEntity
    {
        public int Id { get; set; }
        public string StudentNumber { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; } 
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime RegisteredDate { get; set; } = DateTime.Now;
    }
}