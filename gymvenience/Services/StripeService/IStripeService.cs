using System.Threading.Tasks;
using gymvenience_backend.DTOs; 

namespace gymvenience_backend.Services.StripeService
{
    public interface IStripeService
    {
        Task<string> CreateCheckoutSessionAsync(CheckoutSessionRequest request);
    }
}
