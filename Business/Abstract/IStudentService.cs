using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;
using System.Collections.Generic;

namespace Business.Abstract
{
    public interface IStudentService
    {
        List<Student> GetAll();
        Student GetById(int id);
        void Add(Student student);
        void Update(Student student);
        void Delete(Student student);
    }
}