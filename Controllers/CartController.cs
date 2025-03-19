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
        public async Task<IActionResult> GetCart(string userId)
        {
            var cart = await _cartService.GetOrCreateCartForUserAsync(userId);
            return Ok(cart);
        }

        [HttpPost("{userId}/add")]
        public async Task<IActionResult> AddProductToCart(string userId, [FromBody] AddProductDto request)
        {
            var cart = await _cartService.AddProductToCartAsync(userId, request.ProductId, request.Quantity);
            return Ok(cart);
        }

        [HttpPost("{userId}/remove")]
        public async Task<IActionResult> RemoveProductFromCart(string userId, [FromBody] RemoveProductDto request)
        {
            var cart = await _cartService.RemoveProductFromCartAsync(userId, request.ProductId);
            return Ok(cart);
        }

        [HttpDelete("{userId}/delete")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            var success = await _cartService.ClearCartByUserIdAsync(userId);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}

