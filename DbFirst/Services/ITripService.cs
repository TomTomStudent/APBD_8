using DbFirst.DTO;
namespace DbFirst.Services

{
    public interface ITripService
    {
        IEnumerable<TripDTO> GetTrips(int page, int pageSize);
    }
}
