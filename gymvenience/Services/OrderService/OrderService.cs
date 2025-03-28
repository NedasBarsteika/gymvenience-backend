using gymvenience_backend.Repositories.OrderRepo;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories;
using gymvenience_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace gymvenience_backend.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // Get all orders
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        // Get orders by user ID
        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            return await _orderRepository.GetUserOrdersAsync(userId);
        }
    }
}
