using gymvenience_backend.Common;
using gymvenience_backend.Models;
using gymvenience_backend.Repositories.ProductRepo;
using gymvenience_backend.Repositories.PurchaseRepo;
using gymvenience_backend.Repositories.UserRepo;
using gymvenience_backend.Services.AuthService;
using gymvenience_backend.Services.ProductService;
using gymvenience_backend.Services.PasswordService;

namespace gymvenience_backend.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IPasswordService _passwordService;
        private readonly IAuthService _authService;

        public UserService(IUserRepository userRepository, IPasswordService passwordService, IAuthService authService, IProductRepository productRepository, IPurchaseRepository purchaseRepository)
        {
            _userRepository = userRepository;
            _productRepository = productRepository;
            _purchaseRepository = purchaseRepository;
            _passwordService = passwordService;
            _authService = authService;
        }

        public async Task<Result> AddNewPurchaseAsync(string userId, string productId, ProductType category)
        {
            // Check if product is not already purchased
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return Result.Failure("Product with specified id does not exist");
            }

            // Check if user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User with specified id does not exist");
            }


            var newPurchase = new Purchase(Guid.NewGuid().ToString(), userId, product, category);
            await _purchaseRepository.AddNewPurchaseAsync(newPurchase);
            return Result.Success();
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

        public async Task<(Result, List<Purchase>?)> GetAllPurchasesAsync(string userId)
        {
            // Checking if user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (Result.Failure("User with given id does not exist"), null);
            }

            var userPurchases = await _purchaseRepository.GetUserPurchasesAsync(userId);
            return (Result.Success(), userPurchases);
        }

        public bool IsPasswordCorrect(string password, string storedHashedPassword, string storedSalt)
        {
            var hashedInputPassword = _passwordService.HashPassword(password, storedSalt);
            return hashedInputPassword == storedHashedPassword;
        }
    }
}
