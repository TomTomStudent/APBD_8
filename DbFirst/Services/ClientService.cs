using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DbFirst.Context;
using DbFirst.Models;

namespace DbFirst.Services
{
    public class ClientService : IClientService
    {
        private readonly APBD_8Context _context;

        public ClientService(APBD_8Context context)
        {
            _context = context;
        }

        public async Task<bool> HasAssignedTrips(int clientId)
        {
            return await _context.Clients
                .Where(c => c.IdClient == clientId)
                .SelectMany(c => c.ClientTrips)
                .AnyAsync();
        }

        public async Task<bool> HasAssignedTripsAlt(int clientId)
        {
            return await _context.Set<ClientTrip>()
                .AnyAsync(ct => ct.IdClient == clientId);
        }

        public async Task<bool> DeleteClient(int clientId)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client == null)
                return false;

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
