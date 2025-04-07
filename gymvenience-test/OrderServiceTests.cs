using Xunit;
using FluentAssertions;
using gymvenience_backend.Services.OrderService;
using gymvenience_backend.Repositories.OrderRepo;
using gymvenience_backend.Models;
using gymvenience_backend;
using Microsoft.EntityFrameworkCore;

namespace gymvenience_test
{
    public class OrderServiceTests : IAsyncLifetime
    {
        private ApplicationDbContext _context;
        private OrderService _orderService;
        private IOrderRepository _orderRepository;

        private string userId = "test-user";

        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _orderRepository = new OrderRepository(_context);
            _orderService = new OrderService(_orderRepository);

            await SeedDataAsync();
        }

        public Task DisposeAsync()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            return Task.CompletedTask;
        }

        private async Task SeedDataAsync()
        {
            var orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    UserId = userId,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = "prod-1", Quantity = 2, Price = 10.0 }
                    }
                },
                new Order
                {
                    Id = 2,
                    UserId = "another-user",
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductId = "prod-2", Quantity = 1, Price = 5.0 }
                    }
                }
            };

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllOrdersAsync_Should_Return_All_Orders()
        {
            var result = await _orderService.GetAllOrdersAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllOrdersAsync_Should_Return_EmptyList_When_No_Orders_Exist()
        {
            await DisposeAsync();
            await InitializeAsync();

            _context.Orders.RemoveRange(_context.Orders);
            await _context.SaveChangesAsync();

            var result = await _orderService.GetAllOrdersAsync();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUserOrdersAsync_Should_Return_Orders_For_User()
        {
            var result = await _orderService.GetUserOrdersAsync(userId);

            result.Should().NotBeNull();
            result.Should().OnlyContain(order => order.UserId == userId);
        }

        [Fact]
        public async Task GetUserOrdersAsync_Should_Return_EmptyList_When_User_Has_No_Orders()
        {
            var result = await _orderService.GetUserOrdersAsync("user-without-orders");

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUserOrdersAsync_Should_Return_EmptyList_When_User_Does_Not_Exist()
        {
            var result = await _orderService.GetUserOrdersAsync("nonexistent_user");

            result.Should().BeEmpty();
        }
    }
}
