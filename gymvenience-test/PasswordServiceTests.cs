using Xunit;
using FluentAssertions;
using gymvenience_backend.Services.PasswordService;

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

            hash.Should().NotBeNullOrWhiteSpace();
            salt.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void HashPassword_With_Salt_Should_Produce_Consistent_Hash()
        {
            string password = "StrongPassword@123";
            var (_, salt) = _passwordService.HashPassword(password);

            var hash1 = _passwordService.HashPassword(password, salt);
            var hash2 = _passwordService.HashPassword(password, salt);

            hash1.Should().Be(hash2);
        }

        [Fact]
        public void VerifyPassword_Should_Return_True_For_Valid_Credentials()
        {
            string password = "StrongPassword@123";
            var (hash, salt) = _passwordService.HashPassword(password);

            var result = _passwordService.VerifyPassword(password, hash, salt);

            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_Should_Return_False_For_Invalid_Password()
        {
            string password = "StrongPassword@123";
            var (hash, salt) = _passwordService.HashPassword(password);

            var result = _passwordService.VerifyPassword("WrongPassword", hash, salt);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsPasswordStrongEnough_Should_Return_True_For_Strong_Password()
        {
            var result = _passwordService.IsPasswordStrongEnough("StrongPassword@123");

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("weak")] // Too short
        [InlineData("onlylowercaseletters")]
        [InlineData("ONLYUPPERCASELETTERS")]
        [InlineData("1234567890")]
        [InlineData("NoSymbolsOrNumbers")]
        [InlineData("noUppercase123")]
        public void IsPasswordStrongEnough_Should_Return_False_For_Weak_Passwords(string password)
        {
            var result = _passwordService.IsPasswordStrongEnough(password);

            result.Should().BeFalse();
        }
    }
}
