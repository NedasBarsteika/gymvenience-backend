using Microsoft.AspNetCore.Mvc;
using gymvenience_backend.Services;
using gymvenience_backend.DTOs;

namespace gymvenience_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _cartService.GetOrCreateCartForUserAsync(userId);
            return Ok(cart);
        }

        [HttpPost("{userId}/add")]
        public async Task<IActionResult> AddProductToCart(int userId, [FromBody] AddProductDto request)
        {
            var cart = await _cartService.AddProductToCartAsync(userId, request.ProductId, request.Quantity);
            return Ok(cart);
        }

        [HttpPost("{userId}/remove")]
        public async Task<IActionResult> RemoveProductFromCart(int userId, [FromBody] RemoveProductDto request)
        {
            var cart = await _cartService.RemoveProductFromCartAsync(userId, request.ProductId, request.Quantity);
            return Ok(cart);
        }

        [HttpDelete("{cartId}")]
        public async Task<IActionResult> ClearCart(int cartId)
        {
            var success = await _cartService.ClearCartAsync(cartId);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}

