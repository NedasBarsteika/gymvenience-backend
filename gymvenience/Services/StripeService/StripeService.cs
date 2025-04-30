using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;
using gymvenience_backend.DTOs; 

namespace gymvenience_backend.Services.StripeService
{
    public class StripeService : IStripeService
    {
        public async Task<string> CreateCheckoutSessionAsync(CheckoutSessionRequest request)
        {
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
                            UnitAmount = (long)Math.Round(request.TotalPrice * 100), 
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

            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            return session.Id;
        }
    }
}
