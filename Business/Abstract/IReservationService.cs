using Entities.Concrete;

namespace Business.Abstract
{
    public interface IReservationService
    {
        List<Reservation> GetAll();
        Reservation GetById(int id);
        void Add(Reservation reservation);
        void Update(Reservation reservation);
        void Delete(Reservation reservation);
    }
}