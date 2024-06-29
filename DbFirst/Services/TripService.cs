using DbFirst.Context;
using DbFirst.DTO;
using DbFirst.Models;
using DbFirst.Services;
using Microsoft.EntityFrameworkCore;

namespace TripsApi.Services
{
    public class TripService : ITripService
    {
        private readonly APBD_8Context _context;

        public TripService(APBD_8Context context)
        {
            _context = context;
        }

        public IEnumerable<TripDTO> GetTrips(int page, int pageSize)
        {
            try
            {
                var query = _context.Trips
                    .Include(t => t.IdCountries)
                    .Include(t => t.ClientTrips)
                        .ThenInclude(ct => ct.IdClientNavigation)
                    .OrderByDescending(t => t.DateFrom)
                    .Select(t => new TripDTO
                    {
                        Name = t.Name,
                        Description = t.Description,
                        DateFrom = t.DateFrom,
                        DateTo = t.DateTo,
                        MaxPeople = t.MaxPeople,
                        Countries = t.IdCountries.Select(c => new CountryDTO { Name = c.Name }).ToList(),
                        Clients = t.ClientTrips.Select(ct => new ClientDTO
                        {
                            FirstName = ct.IdClientNavigation.FirstName,
                            LastName = ct.IdClientNavigation.LastName
                        }).ToList()
                    });

                var totalCount = query.Count();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var trips = query.Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

                return trips;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving trips", ex);
            }
        }

        public async Task<int> AssignClientToTrip(int idTrip, AssignClientToTrip request)
        {
            var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == request.Client.Pesel);
            if (existingClient != null)
            {
                throw new InvalidOperationException("Client with provided PESEL already exists.");
            }

            var isClientAlreadyRegistered = await _context.ClientTrips
                .AnyAsync(ct => ct.IdClientNavigation.Pesel == request.Client.Pesel && ct.IdTrip == idTrip);
            if (isClientAlreadyRegistered)
            {
                throw new InvalidOperationException("Client is already registered for the trip.");
            }

            var trip = await _context.Trips.FindAsync(idTrip);
            if (trip == null)
            {
                throw new InvalidOperationException("Trip does not exist.");
            }
            if (trip.DateFrom <= DateTime.Today)
            {
                throw new InvalidOperationException("Cannot register for a trip that has already occurred.");
            }

            var newClient = new Client
            {
                FirstName = request.Client.FirstName,
                LastName = request.Client.LastName,
                Email = request.Client.Email,
                Telephone = request.Client.Telephone,
                Pesel = request.Client.Pesel
            };

            var clientTrip = new ClientTrip
            {
                IdClientNavigation = newClient,
                IdTripNavigation = trip,
                RegisteredAt = DateTime.Now,
                PaymentDate = request.Client.PaymentDate
            };

            _context.Clients.Add(newClient);
            _context.ClientTrips.Add(clientTrip);
            await _context.SaveChangesAsync();

            return clientTrip.IdClient;
        }
    }
}
