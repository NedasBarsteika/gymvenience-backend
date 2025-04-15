using gymvenience_backend.Models;

namespace gymvenience_backend.Services.OrderService
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetUserOrdersAsync(string userId);
    }
}
