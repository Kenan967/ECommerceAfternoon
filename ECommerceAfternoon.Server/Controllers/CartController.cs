using ECommerceAfternoon.Server.DTOs.Cart;
using ECommerceAfternoon.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAfternoon.Server.Controllers
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

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            try
            {
                return Ok(await _cartService.GetCartAsync(userId));
            }
            catch (CartServiceException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [HttpPost("{userId:int}/items")]
        public async Task<IActionResult> AddToCart(int userId, [FromBody] AddToCartDto dto)
        {
            try
            {
                await _cartService.AddToCartAsync(userId, dto);
                return Ok();
            }
            catch (CartServiceException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [HttpPut("{userId:int}/items/{productId:int}")]
        public async Task<IActionResult> UpdateQuantity(
            int userId, int productId, [FromQuery] int quantity)
        {
            try
            {
                await _cartService.UpdateQuantityAsync(userId, productId, quantity);
                return Ok();
            }
            catch (CartServiceException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [HttpDelete("{userId:int}/items/{productId:int}")]
        public async Task<IActionResult> RemoveFromCart(int userId, int productId)
        {
            try
            {
                await _cartService.RemoveFromCartAsync(userId, productId);
                return NoContent();
            }
            catch (CartServiceException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [HttpDelete("{userId:int}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            try
            {
                await _cartService.ClearCartAsync(userId);
                return NoContent();
            }
            catch (CartServiceException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }
    }
}
