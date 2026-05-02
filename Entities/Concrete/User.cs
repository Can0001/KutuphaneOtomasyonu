using Core.Entities;
using System;

namespace Entities.Concrete
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Admin veya Personel kaydedilirken buralar boş (null) kalacak
        public string StudentNumber { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
    }
}