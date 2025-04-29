using gymvenience_backend.DTOs;
using gymvenience_backend.Models;
using gymvenience_backend.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Stripe;
using Stripe.Checkout;

namespace gymvenience_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IOrderService _orderService;
        
        public PaymentController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        
        [HttpPost("create-checkout-session")]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutSessionRequest request)
        {

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }
            

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "eur",
                            UnitAmount = (long)(request.TotalPrice * 100), // converting to cents
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Order Payment",
                            },
                        },
                        Quantity = 1,
                    }
                },
                Mode = "payment",
                SuccessUrl = "http://localhost:5173/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "http://localhost:5173/cancel",
            };

            var stripeService = new SessionService();
            Session session;
            try
            {
                session = await stripeService.CreateAsync(options);
            }
            catch (StripeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Items = new List<OrderItem>()
            };

            foreach (var itemDto in request.Items)
            {
                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    Price = itemDto.Price
                };
                order.Items.Add(orderItem);
            }

            try
            {
                await _orderService.CreateOrderAsync(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to create order", details = ex.Message });
            }

            return Ok(new { sessionId = session.Id });
        }
    }
}
