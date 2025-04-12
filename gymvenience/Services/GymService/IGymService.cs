namespace gymvenience.Services.GymService
{
    public interface IGymService
    {
        Task<List<string>> GetAllCitiesAsync();
        Task<List<string>> GetAllAddressesAsync();
    }
}
