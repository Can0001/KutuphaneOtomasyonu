using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Contexts;
using Entities.Concrete;
using KutüphaneOtomasyonu.Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfStaffDal : EfEntityRepositoryBase<Staff, LibraryDbContext>, IStaffDal
    {
    }
}