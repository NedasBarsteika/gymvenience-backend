using gymvenience_backend.Models;

namespace gymvenience_backend.Services.AuthService
{
    public interface IAuthService
    {
        public string GenerateJwtToken(User user);
    }
}
