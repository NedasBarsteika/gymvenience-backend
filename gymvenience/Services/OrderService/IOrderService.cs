using gymvenience_backend.Models;
using gymvenience_backend.DTOs;

namespace gymvenience_backend.Services.OrderService
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<OrderDto>>  GetUserOrdersAsync(string userId);
        Task<bool> DeliverOrderAsync(int orderId);
        Task<bool> ExistsForSessionAsync(string stripeSessionId);
        Task<Order?> GetBySessionAsync(string stripeSessionId);
    }
}
