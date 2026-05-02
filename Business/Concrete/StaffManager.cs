using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using System.Collections.Generic;

namespace Business.Concrete
{
    public class StaffManager : IStaffService
    {
        private readonly IStaffDal _staffDal;

        public StaffManager(IStaffDal staffDal)
        {
            _staffDal = staffDal;
        }

        public void Add(Staff staff) { _staffDal.Add(staff); }
        public void Delete(Staff staff) { _staffDal.Delete(staff); }
        public List<Staff> GetAll() { return _staffDal.GetAll(); }
        public Staff GetById(int id) { return _staffDal.Get(s => s.Id == id); }
        public void Update(Staff staff) { _staffDal.Update(staff); }
        public Staff GetByEmail(string email)
        {
            return _staffDal.Get(s => s.Email == email);
        }
    }
}