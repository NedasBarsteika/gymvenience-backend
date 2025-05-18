using gymvenience_backend.Models;
using gymvenience_backend.DTOs;
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

public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
{
    return await _context.Orders
        .Where(o => o.UserId == userId)
        .Select(o => new OrderDto {
            Id          = o.Id,
            OrderDate   = o.OrderDate,
            IsDelivered = o.IsDelivered,
            Items       = o.Items.Select(i => new OrderItemDto {
                ProductId   = i.ProductId,
                ProductName = _context.Products
                                       .Where(p => p.Id == i.ProductId)
                                       .Select(p => p.Name)
                                       .FirstOrDefault() ?? "Unknown",
                Quantity    = i.Quantity,
                Price   = i.Price
            }).ToList()
        })
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
