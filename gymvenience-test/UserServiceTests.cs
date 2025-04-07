using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using gymvenience_backend;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.OrderRepo;
using gymvenience_backend.Repositories.UserRepo;
using gymvenience_backend.Services.AuthService;
using gymvenience_backend.Services.PasswordService;
using gymvenience_backend.Services.UserService;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace gymvenience_test;

public class FakeAuthService : IAuthService
{
    public string GenerateJwtToken(User user) => "dummy_token";
}

public class UserServiceTests : IAsyncLifetime
{
    private ApplicationDbContext _context = null!;
    private UserRepository _userRepository = null!;
    private OrderRepository _orderRepository = null!;
    private PasswordService _passwordService = null!;
    private FakeAuthService _authService = null!;
    private UserService _userService = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _userRepository = new UserRepository(_context);
        _orderRepository = new OrderRepository(_context);
        _passwordService = new PasswordService();

        _userService = new UserService(
            _userRepository,
            _passwordService,
            _authService = new FakeAuthService(),
            null, // product repo unused
            _orderRepository);

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
        var (hashed, salt) = _passwordService.HashPassword("Password@123");

        var user = new User
        {
            Id = "1",
            Name = "John",
            Surname = "Doe",
            Email = "john@example.com",
            HashedPassword = hashed,
            Salt = salt
        };

        _context.Users.Add(user);
        _context.Orders.Add(new Order { Id = 1, UserId = "1" });
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateUserAsync_Should_Create_User_When_Valid()
    {
        var (result, user) = await _userService.CreateUserAsync("Jane", "Smith", "jane@example.com", "Password@123");

        result.IsSuccess.Should().BeTrue();
        user.Should().NotBeNull();
        user!.Email.Should().Be("jane@example.com");
    }

    [Fact]
    public async Task CreateUserAsync_Should_Fail_When_Password_Weak()
    {
        var (result, user) = await _userService.CreateUserAsync("Weak", "Pass", "weak@example.com", "weak");

        result.IsSuccess.Should().BeFalse();
        user.Should().BeNull();
    }

    [Fact]
    public async Task CreateUserAsync_Should_Fail_When_Email_Exists()
    {
        var (result, user) = await _userService.CreateUserAsync("John", "Doe", "john@example.com", "Password@123");

        result.IsSuccess.Should().BeFalse();
        user.Should().BeNull();
    }

    [Fact]
    public async Task GenerateJwtAsync_Should_Return_Token_When_Credentials_Valid()
    {
        var (result, token) = await _userService.GenerateJwtAsync("john@example.com", "Password@123");

        result.IsSuccess.Should().BeTrue();
        token.Should().Be("dummy_token");
    }

    [Fact]
    public async Task GenerateJwtAsync_Should_Fail_When_User_Does_Not_Exist()
    {
        var (result, token) = await _userService.GenerateJwtAsync("nouser@example.com", "Password@123");

        result.IsSuccess.Should().BeFalse();
        token.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllOrdersAsync_Should_Return_Orders_When_User_Exists()
    {
        var (result, orders) = await _userService.GetAllOrdersAsync("1");

        result.IsSuccess.Should().BeTrue();
        orders.Should().NotBeNull();
        orders.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAllOrdersAsync_Should_Fail_When_User_Does_Not_Exist()
    {
        var (result, orders) = await _userService.GetAllOrdersAsync("99");

        result.IsSuccess.Should().BeFalse();
        orders.Should().BeNull();
    }

    [Fact]
    public void IsPasswordCorrect_Should_Return_True_When_Correct()
    {
        var (hashed, salt) = _passwordService.HashPassword("Password@123");

        var result = _userService.IsPasswordCorrect("Password@123", hashed, salt);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsPasswordCorrect_Should_Return_False_When_Incorrect()
    {
        var result = _userService.IsPasswordCorrect("wrong", "hashed", "salt");
        result.Should().BeFalse();
    }
}
