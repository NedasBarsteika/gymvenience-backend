using gymvenience_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace gymvenience_backend.Repositories.OrderRepo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Items) // Include OrderItems
                .ToListAsync();
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items) // Include OrderItems
                .ToListAsync();
        }
        // Finds the order by its id
        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                                 .Include(o => o.Items)
                                 .FirstOrDefaultAsync(o => o.Id == orderId);
        }
        // Marks order as delivered
        public async Task MarkDeliveredAsync(Order order)
        {
            order.IsDelivered = true;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
