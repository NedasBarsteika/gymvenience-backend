using gymvenience.Models;
using gymvenience.Repositories.GymRepo;
using gymvenience_backend.DTOs;
using gymvenience_backend.Models;

namespace gymvenience.Services.GymService
{
    public class GymService : IGymService
    {
        private readonly IGymRepository _repository;

        public GymService(IGymRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Gym>> GetAllGymsAsync()
        {
            return await _repository.GetAllAsync();
        }
        public Task<List<string>> GetAllCitiesAsync() => _repository.GetAllCitiesAsync();
        public Task<List<string>> GetAllAddressesAsync() => _repository.GetAllAddressesAsync();
        public GymDto? GetGymSummaryById(string id)
        {
            var gym = _repository.GetGymById(id);
            if (gym == null) return null;

            return new GymDto
            {
                Name = gym.Name,
                Address = gym.Address
            };
        }
    }
}
