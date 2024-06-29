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

        [HttpPost("{idTrip}/clients")]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, AssignClientToTrip request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _tripService.AssignClientToTrip(idTrip, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
