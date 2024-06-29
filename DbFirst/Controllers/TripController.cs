using Microsoft.AspNetCore.Mvc;
using DbFirst.DTO;
using DbFirst.Services;

namespace TripsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TripDTO>> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var trips = _tripService.GetTrips(page, pageSize);
            return Ok(trips);
        }
    }
}
