using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using DataAccess.Contexts;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfReservationDal : EfEntityRepositoryBase<Reservation, LibraryDbContext>, IReservationDal
    {
    }
}