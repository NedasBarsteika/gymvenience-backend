using gymvenience_backend.Repositories.OrderRepo;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories;
using gymvenience_backend.Models;
using Microsoft.EntityFrameworkCore;
using gymvenience_backend.DTOs;

namespace gymvenience_backend.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;

        public OrderService(IOrderRepository orderRepository, ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }
        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
        // Get all orders
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        // Get orders by user ID
        public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
        {
            return await _orderRepository.GetUserOrdersAsync(userId);
        }
        public async Task<bool> DeliverOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;
            if (order.IsDelivered) return false;

            await _orderRepository.MarkDeliveredAsync(order);
            return true;
        }
        public async Task<bool> ExistsForSessionAsync(string stripeSessionId)
        {
            if (string.IsNullOrWhiteSpace(stripeSessionId))
                return false;

            return await _context.Orders
                                 .AsNoTracking()
                                 .AnyAsync(o => o.StripeSessionId == stripeSessionId);
        }
        public async Task<Order?> GetBySessionAsync(string stripeSessionId)
        {
            return await _context.Orders
                         .AsNoTracking()
                         .FirstOrDefaultAsync(o => o.StripeSessionId == stripeSessionId);
        }
    }
}
