using gymvenience.Models;
using gymvenience_backend.DTOs;
using gymvenience_backend.Models;

namespace gymvenience.Services.GymService
{
    public interface IGymService
    {
        Task<IEnumerable<Gym>> GetAllGymsAsync();
        Task<List<string>> GetAllCitiesAsync();
        Task<List<string>> GetAllAddressesAsync();
        GymDto? GetGymSummaryById(string id);
    }
}
