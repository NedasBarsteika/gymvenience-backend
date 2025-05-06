using gymvenience_backend.DTOs;

namespace gymvenience.Services.GymService
{
    public interface IGymService
    {
        Task<List<string>> GetAllCitiesAsync();
        Task<List<string>> GetAllAddressesAsync();
        GymDto? GetGymSummaryById(string id);
    }
}
