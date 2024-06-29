using DbFirst.Context;
using DbFirst.DTO;
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
    }
}
