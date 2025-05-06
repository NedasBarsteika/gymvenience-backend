using gymvenience.Repositories.GymRepo;
using gymvenience_backend.DTOs;

namespace gymvenience.Services.GymService
{
    public class GymService : IGymService
    {
        private readonly IGymRepository _repository;

        public GymService(IGymRepository repository)
        {
            _repository = repository;
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
