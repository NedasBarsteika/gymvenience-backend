using gymvenience.Models;
using gymvenience_backend.Models;

namespace gymvenience.Repositories.GymRepo
{
    public interface IGymRepository
    {
        void GenerateMockGyms();
        Task<IEnumerable<Gym>> GetAllAsync();
        Task<List<string>> GetAllCitiesAsync();
        Task<List<string>> GetAllAddressesAsync();
        Gym? GetGymById(string id);
    }
}
