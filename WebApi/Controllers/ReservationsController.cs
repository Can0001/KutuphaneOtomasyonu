using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _reservationService.GetAll();
            return Ok(result);
        }

        [HttpPost("add")]
        public IActionResult Add(Reservation reservation)
        {
            _reservationService.Add(reservation);
            return Ok("Rezervasyon (ödünç alma) işlemi başarıyla kaydedildi!");
        }
    }
}