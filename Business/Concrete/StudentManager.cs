using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using System.Collections.Generic;

namespace Business.Concrete
{
    public class StudentManager : IStudentService
    {
        private readonly IStudentDal _studentDal;

        public StudentManager(IStudentDal studentDal)
        {
            _studentDal = studentDal;
        }

        public void Add(Student student) { _studentDal.Add(student); }
        public void Delete(Student student) { _studentDal.Delete(student); }
        public List<Student> GetAll() { return _studentDal.GetAll(); }
        public Student GetById(int id) { return _studentDal.Get(s => s.Id == id); }
        public void Update(Student student) { _studentDal.Update(student); }
    }
}