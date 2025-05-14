using gymvenience_backend.Common;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories.OrderRepo;
using gymvenience_backend.Repositories.UserRepo;
using gymvenience_backend.Services.AuthService;
using gymvenience_backend.Services.ProductService;
using gymvenience_backend.Services.PasswordService;
using Microsoft.EntityFrameworkCore;
using gymvenience.Repositories.TrainerAvailabilityRepo;
using gymvenience_backend.Repositories.ReservationRepo;

namespace gymvenience_backend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPasswordService _passwordService;
        private readonly IAuthService _authService;
        private readonly ITrainerAvailabilityRepository _trainerAvailabilityRepository;
        private readonly IReservationRepository _reservationRepository;

        public UserService(IUserRepository userRepository, IPasswordService passwordService, IAuthService authService, IProductRepository productRepository, IOrderRepository orderRepository, ITrainerAvailabilityRepository trainerRepository, IReservationRepository reservationRepository)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _passwordService = passwordService;
            _authService = authService;
            _trainerAvailabilityRepository = trainerRepository;
            _reservationRepository = reservationRepository;
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
        // Remove Trainer (demote into user)
        public async Task<bool> DemoteTrainerAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsTrainer)
                return false;

            // remove trainer availability slots
            _trainerAvailabilityRepository.RemoveAllForTrainer(userId);

            // demote
            user.DemoteFromTrainer();
            await _userRepository.UpdateAsync(user);

            return true;
        }
        public async Task<bool> DeleteUserAsync(string userId)
        {
            // You could add business rules here (e.g. prevent deleting last admin)
            return await _userRepository.DeleteAsync(userId);
        }
        // Getting all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }
        // Promoting user to trainer
        public async Task<bool> PromoteToTrainerAsync(string userId)
        {
            return await _userRepository.PromoteToTrainerAsync(userId);
        }
        // Searchas
        public async Task<IEnumerable<User>> SearchTrainersByNameAsync(string searchText)
        {
            return await _userRepository.SearchTrainersByNameAsync(searchText);
        }
        public async Task<decimal> CalculateTrainerEarningsAsync(string trainerId)
        {
            var trainer = await _userRepository.GetByIdAsync(trainerId);
            if (trainer == null || !trainer.IsTrainer)
                throw new KeyNotFoundException();

            var completed = _reservationRepository.GetCompletedReservationsByTrainer(trainerId);

            decimal total = completed
                .Sum(r => (decimal)r.Duration.TotalHours * r.RateAtBooking);

            return total;
        }


        public async Task<bool> SetHourlyRateAsync(string trainerId, decimal newRate)
        {
            // Prevent if any pending bookings exist
            //var pending = _reservationRepository.GetPendingReservationsByTrainer(trainerId);
            //if (pending.Any()) return false;

            return await _userRepository.UpdateHourlyRateAsync(trainerId, newRate);
        }

    }
}
