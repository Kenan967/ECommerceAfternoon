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

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = GetUserId();

            var items = await _context.WishlistItems
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.Product)
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

        [HttpPost("{productId:int}")]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = GetUserId();

            var productExists = await _context.Products
                .AnyAsync(x => x.Id == productId);

            if (!productExists)
                return NotFound("Product not found.");

            var exists = await _context.WishlistItems
                .AnyAsync(x => x.UserId == userId && x.ProductId == productId);

            if (exists)
                return Conflict("Product is already in wishlist.");

            _context.WishlistItems.Add(new WishlistItem
            {
                UserId = userId,
                ProductId = productId
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product added to wishlist." });
        }

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = GetUserId();

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId && x.ProductId == productId);

            if (item is null)
                return NotFound("Product is not in wishlist.");

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
