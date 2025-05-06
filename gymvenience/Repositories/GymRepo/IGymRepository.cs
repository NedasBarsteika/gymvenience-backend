using gymvenience.Models;


namespace gymvenience.Repositories.GymRepo
{
    public interface IGymRepository
    {
        void GenerateMockGyms();
        Task<List<string>> GetAllCitiesAsync();
        Task<List<string>> GetAllAddressesAsync();
        public Gym? GetGymById(string id);
    }
}
