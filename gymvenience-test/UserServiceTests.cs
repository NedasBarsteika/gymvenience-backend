using gymvenience_backend.Models;
using gymvenience_backend.Repositories.OrderRepo;
using gymvenience_backend.Repositories.UserRepo;
using gymvenience_backend.Services.AuthService;
using gymvenience_backend.Services.PasswordService;
using gymvenience_backend.Services.UserService;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gymvenience_test
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<IOrderRepository> _orderRepoMock = new();
        private readonly Mock<IPasswordService> _passwordServiceMock = new();
        private readonly Mock<IAuthService> _authServiceMock = new();
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userService = new UserService(
                _userRepoMock.Object,
                _passwordServiceMock.Object,
                _authServiceMock.Object,
                null, // ProductRepo not used
                _orderRepoMock.Object
            );
        }

        [Fact]
        public async Task CreateUserAsync_Should_Create_User_When_Valid()
        {
            _passwordServiceMock.Setup(p => p.IsPasswordStrongEnough(It.IsAny<string>())).Returns(true);
            _userRepoMock.Setup(u => u.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _passwordServiceMock.Setup(p => p.HashPassword(It.IsAny<string>())).Returns(("hashed", "salt"));

            var (result, user) = await _userService.CreateUserAsync("John", "Doe", "john@example.com", "Password@123");

            Assert.True(result.IsSuccess);
            Assert.NotNull(user);
        }

        [Fact]
        public async Task CreateUserAsync_Should_Fail_When_Password_Weak()
        {
            _passwordServiceMock.Setup(p => p.IsPasswordStrongEnough(It.IsAny<string>())).Returns(false);

            var (result, user) = await _userService.CreateUserAsync("John", "Doe", "john@example.com", "weak");

            Assert.False(result.IsSuccess);
            Assert.Null(user);
        }

        [Fact]
        public async Task CreateUserAsync_Should_Fail_When_Email_Exists()
        {
            _passwordServiceMock.Setup(p => p.IsPasswordStrongEnough(It.IsAny<string>())).Returns(true);
            _userRepoMock.Setup(u => u.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User("1", "John", "Doe", "john@example.com", "hashed", "salt"));

            var (result, user) = await _userService.CreateUserAsync("John", "Doe", "john@example.com", "Password@123");

            Assert.False(result.IsSuccess);
            Assert.Null(user);
        }

        [Fact]
        public async Task GenerateJwtAsync_Should_Return_Token_When_Credentials_Valid()
        {
            _userRepoMock.Setup(u => u.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User("1", "John", "Doe", "john@example.com", "hashed", "salt"));
            _passwordServiceMock.Setup(p => p.HashPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("hashed");
            _authServiceMock.Setup(a => a.GenerateJwtToken(It.IsAny<User>())).Returns("valid_jwt");

            var (result, token) = await _userService.GenerateJwtAsync("john@example.com", "Password@123");

            Assert.True(result.IsSuccess);
            Assert.Equal("valid_jwt", token);
        }

        [Fact]
        public async Task GenerateJwtAsync_Should_Fail_When_User_Does_Not_Exist()
        {
            _userRepoMock.Setup(u => u.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var (result, token) = await _userService.GenerateJwtAsync("john@example.com", "Password@123");

            Assert.False(result.IsSuccess);
            Assert.Equal(string.Empty, token);
        }

        [Fact]
        public async Task GetAllOrdersAsync_Should_Return_Orders_When_User_Exists()
        {
            _userRepoMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(new User("1", "John", "Doe", "john@example.com", "hashed", "salt"));
            _orderRepoMock.Setup(o => o.GetUserOrdersAsync(It.IsAny<string>())).ReturnsAsync(new List<Order> { new Order { UserId = "1" } });

            var (result, orders) = await _userService.GetAllOrdersAsync("1");

            Assert.True(result.IsSuccess);
            Assert.NotNull(orders);
            Assert.Single(orders);
        }

        [Fact]
        public async Task GetAllOrdersAsync_Should_Fail_When_User_Does_Not_Exist()
        {
            _userRepoMock.Setup(u => u.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var (result, orders) = await _userService.GetAllOrdersAsync("1");

            Assert.False(result.IsSuccess);
            Assert.Null(orders);
        }

        [Fact]
        public void IsPasswordCorrect_Should_Return_True_When_Correct()
        {
            _passwordServiceMock.Setup(p => p.HashPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("hashed");

            var result = _userService.IsPasswordCorrect("Password@123", "hashed", "salt");

            Assert.True(result);
        }

        [Fact]
        public void IsPasswordCorrect_Should_Return_False_When_Incorrect()
        {
            _passwordServiceMock.Setup(p => p.HashPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("wrong_hash");

            var result = _userService.IsPasswordCorrect("Password@123", "hashed", "salt");

            Assert.False(result);
        }
    }
}
