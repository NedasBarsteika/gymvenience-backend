using Microsoft.AspNetCore.Mvc;
using gymvenience_backend.DTOs;
using gymvenience_backend.Services.StripeService;
using System.Threading.Tasks;
using Stripe;

namespace gymvenience_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IStripeService _stripePaymentService;

        public PaymentController(IStripeService stripePaymentService)
        {
            _stripePaymentService = stripePaymentService;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutSessionRequest request)
        {
            try
            {
                var sessionId = await _stripePaymentService.CreateCheckoutSessionAsync(request);
                return Ok(new { sessionId });
            }
            catch (StripeException ex)
            {
                // Handle any errors returned from Stripe
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
