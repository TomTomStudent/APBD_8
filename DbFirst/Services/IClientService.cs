namespace DbFirst.Services
{
    public interface IClientService
    {
        Task<bool> HasAssignedTrips(int clientId);
        Task<bool> DeleteClient(int clientId);
    }
}
