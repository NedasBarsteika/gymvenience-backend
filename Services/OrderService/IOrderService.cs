using gymvenience_backend.Models;

namespace gymvenience_backend.Services.OrderService
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetUserOrdersAsync(string userId);
    }
}
