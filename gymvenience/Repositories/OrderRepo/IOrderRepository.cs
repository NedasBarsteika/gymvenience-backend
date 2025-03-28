using gymvenience_backend.Models;

namespace gymvenience_backend.Repositories.OrderRepo
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetUserOrdersAsync(string userId);
    }
}
