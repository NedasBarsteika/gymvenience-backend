using gymvenience_backend.Models;
using gymvenience_backend.Repositories.OrderRepo;
using gymvenience_backend.Services.OrderService;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gymvenience_test
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderService = new OrderService(_orderRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllOrdersAsync_Should_Return_All_Orders()
        {
            // Arrange
            var orders = new List<Order>
        {
            new Order { Id = 1, UserId = "user1", Items = new List<OrderItem>() },
            new Order { Id = 1, UserId = "user2", Items = new List<OrderItem>() }
        };

            _orderRepositoryMock.Setup(repo => repo.GetAllOrdersAsync()).ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetAllOrdersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllOrdersAsync_Should_Return_EmptyList_When_No_Orders_Exist()
        {
            _orderRepositoryMock.Setup(repo => repo.GetAllOrdersAsync()).ReturnsAsync(new List<Order>());

            var result = await _orderService.GetAllOrdersAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserOrdersAsync_Should_Return_Orders_For_User()
        {
            // Arrange
            var userId = "user1";
            var orders = new List<Order>
        {
            new Order { Id = 1, UserId = userId, Items = new List<OrderItem>() },
            new Order { Id = 3, UserId = userId, Items = new List<OrderItem>() }
        };

            _orderRepositoryMock.Setup(repo => repo.GetUserOrdersAsync(userId)).ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetUserOrdersAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, order => Assert.Equal(userId, order.UserId));
        }

        [Fact]
        public async Task GetUserOrdersAsync_Should_Return_EmptyList_When_User_Has_No_Orders()
        {
            _orderRepositoryMock.Setup(repo => repo.GetUserOrdersAsync("user2")).ReturnsAsync(new List<Order>());

            var result = await _orderService.GetUserOrdersAsync("user2");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserOrdersAsync_Should_Return_EmptyList_When_User_Does_Not_Exist()
        {
            _orderRepositoryMock.Setup(repo => repo.GetUserOrdersAsync("nonexistent_user")).ReturnsAsync(new List<Order>());

            var result = await _orderService.GetUserOrdersAsync("nonexistent_user");

            Assert.Empty(result);
        }
    }
}
