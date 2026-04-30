using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class ReservationManager : IReservationService
    {
        private readonly IReservationDal _reservationDal;

        public ReservationManager(IReservationDal reservationDal)
        {
            _reservationDal = reservationDal;
        }

        public void Add(Reservation reservation)
        {
            // İleride buraya "Kitap başkasındaysa ödünç verilemez" gibi kurallar yazacağız.
            _reservationDal.Add(reservation);
        }

        public void Delete(Reservation reservation)
        {
            _reservationDal.Delete(reservation);
        }

        public List<Reservation> GetAll()
        {
            return _reservationDal.GetAll();
        }

        public Reservation GetById(int id)
        {
            return _reservationDal.Get(r => r.Id == id);
        }

        public void Update(Reservation reservation)
        {
            _reservationDal.Update(reservation);
        }
    }
}