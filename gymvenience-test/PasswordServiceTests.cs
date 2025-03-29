using gymvenience_backend.Services.PasswordService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gymvenience_test
{
    public class PasswordServiceTests
    {
        private readonly PasswordService _passwordService;

        public PasswordServiceTests()
        {
            _passwordService = new PasswordService();
        }

        [Fact]
        public void HashPassword_Should_Return_Hash_And_Salt()
        {
            var (hash, salt) = _passwordService.HashPassword("StrongPassword@123");
            Assert.False(string.IsNullOrEmpty(hash));
            Assert.False(string.IsNullOrEmpty(salt));
        }

        [Fact]
        public void HashPassword_With_Salt_Should_Produce_Consistent_Hash()
        {
            string password = "StrongPassword@123";
            var (_, salt) = _passwordService.HashPassword(password);

            string hash1 = _passwordService.HashPassword(password, salt);
            string hash2 = _passwordService.HashPassword(password, salt);

            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void VerifyPassword_Should_Return_True_For_Correct_Password()
        {
            string password = "StrongPassword@123";
            var (hash, salt) = _passwordService.HashPassword(password);

            bool isValid = _passwordService.VerifyPassword(password, hash, salt);

            Assert.True(isValid);
        }

        [Fact]
        public void VerifyPassword_Should_Return_False_For_Wrong_Password()
        {
            string password = "StrongPassword@123";
            var (hash, salt) = _passwordService.HashPassword(password);

            bool isValid = _passwordService.VerifyPassword("WrongPassword@123", hash, salt);

            Assert.False(isValid);
        }

        [Fact]
        public void IsPasswordStrongEnough_Should_Return_True_For_Strong_Password()
        {
            bool result = _passwordService.IsPasswordStrongEnough("StrongPassword@123");
            Assert.True(result);
        }

        [Theory]
        [InlineData("weak")] // Too short
        [InlineData("onlylowercaseletters")]
        [InlineData("ONLYUPPERCASELETTERS")]
        [InlineData("1234567890")]
        [InlineData("NoSymbolsOrNumbers")] // Lacks number and symbol
        [InlineData("noUppercase123")] // Lacks uppercase
        public void IsPasswordStrongEnough_Should_Return_False_For_Weak_Passwords(string weakPassword)
        {
            bool result = _passwordService.IsPasswordStrongEnough(weakPassword);
            Assert.False(result);
        }
    }
}
