using ECommerceAfternoon.Server.Data;
using ECommerceAfternoon.Server.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceAfternoon.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIdClaim, out userId);
        }

        // GET: api/Wishlist
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var items = await _context.WishlistItems
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    id = x.Id,
                    productId = x.ProductId,
                    name = x.Product.Name,
                    price = x.Product.Price,
                    imageUrl = x.Product.ImageUrl,
                    stock = x.Product.Stock
                })
                .ToListAsync();

            return Ok(items);
        }

        // POST: api/Wishlist/{productId}
        [HttpPost("{productId:int}")]
        public async Task<IActionResult> Add(int productId)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (productId <= 0)
                return BadRequest("Invalid product ID.");

            var productExists = await _context.Products
                .AnyAsync(x => x.Id == productId);

            if (!productExists)
                return NotFound("Product not found.");

            var alreadyExists = await _context.WishlistItems
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.ProductId == productId);

            if (alreadyExists)
                return Conflict("Product is already in wishlist.");

            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                ProductId = productId
            };

            _context.WishlistItems.Add(wishlistItem);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product added to wishlist.",
                productId
            });
        }

        // DELETE: api/Wishlist/{productId}
        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> Remove(int productId)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (productId <= 0)
                return BadRequest("Invalid product ID.");

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.ProductId == productId);

            if (item == null)
                return NotFound("Product is not in wishlist.");

            _context.WishlistItems.Remove(item);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
