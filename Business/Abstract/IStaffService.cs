using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface IStaffService
    {
        List<Staff> GetAll();
        Staff GetById(int id);

        Staff GetByEmail(string email);
        void Add(Staff staff);
        void Update(Staff staff);
        void Delete(Staff staff);
    }
}