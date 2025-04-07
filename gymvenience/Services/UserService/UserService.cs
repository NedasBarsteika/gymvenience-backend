using gymvenience_backend.Common;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories.OrderRepo;
using gymvenience_backend.Repositories.UserRepo;
using gymvenience_backend.Services.AuthService;
using gymvenience_backend.Services.ProductService;
using gymvenience_backend.Services.PasswordService;
using Microsoft.EntityFrameworkCore;

namespace gymvenience_backend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPasswordService _passwordService;
        private readonly IAuthService _authService;

        public UserService(IUserRepository userRepository, IPasswordService passwordService, IAuthService authService, IProductRepository productRepository, IOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _passwordService = passwordService;
            _authService = authService;
        }

        public async Task<(Result, User?)> CreateUserAsync(string name, string surname, string email, string password)
        {
            // Validate password strength
            if (!_passwordService.IsPasswordStrongEnough(password))
            {
                return (Result.Failure("Password is not strong enough. It should have upper, lower letters, numbers and special characters."), null);
            }

            // Check if email is free to use
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                return (Result.Failure("User with specified email already exists"), null);
            }

            // Hashing password
            var (hashedPassword, salt) = _passwordService.HashPassword(password);


            var newUser = new User(Guid.NewGuid().ToString(), name, surname, email, hashedPassword, salt);
            // Create a cart for the new user
            var newCart = new Cart
            {
                UserId = newUser.Id,
                CartItems = new List<CartItem>()
            };
            Console.WriteLine(newUser.Id);
            await _userRepository.AddAsync(newUser);
            return (Result.Success(), newUser);
        }

        public async Task<(Result, string)> GenerateJwtAsync(string email, string password)
        {
            // Getting user
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return (Result.Failure("User with specified email does not exist"), string.Empty);
            }

            // Checking if password is correct
            if (!IsPasswordCorrect(password, user.HashedPassword, user.Salt))
            {
                return (Result.Failure("Password is not correct"), string.Empty);
            }
            return (Result.Success(), _authService.GenerateJwtToken(user));
        }

        public async Task<(Result, List<Order>?)> GetAllOrdersAsync(string userId)
        {
            // Checking if user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (Result.Failure("User with given id does not exist"), null);
            }

            var userOrders = await _orderRepository.GetUserOrdersAsync(userId);
            return (Result.Success(), userOrders);
        }

        public bool IsPasswordCorrect(string password, string storedHashedPassword, string storedSalt)
        {
            var hashedInputPassword = _passwordService.HashPassword(password, storedSalt);
            return hashedInputPassword == storedHashedPassword;
        }
    }
}
