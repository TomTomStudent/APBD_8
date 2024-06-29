using DbFirst.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TripsApi.Services;

namespace TripsApi.Controllers
{
    [Route("api/clients")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpDelete("{idClient}")]
        public async Task<IActionResult> DeleteClient(int idClient)
        {
            var hasAssignedTrips = await _clientService.HasAssignedTrips(idClient);
            if (hasAssignedTrips)
            {
                return BadRequest("Cannot delete client because they have assigned trips.");
            }

            var isDeleted = await _clientService.DeleteClient(idClient);
            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
