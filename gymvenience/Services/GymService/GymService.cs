using gymvenience.Repositories.GymRepo;

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
    }
}
