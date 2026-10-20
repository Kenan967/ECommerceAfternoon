using ECommerceAfternoon.Server.Data;
using ECommerceAfternoon.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAfternoon.Server.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Cart?> GetCartByUserIdAsync(int userId) =>
            _context.Carts
                .Include(cart => cart.Items)
                .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == userId);

        public Task<Product?> GetProductByIdAsync(int productId) =>
            _context.Products.FirstOrDefaultAsync(product => product.Id == productId);

        public Task<CartItem?> GetCartItemAsync(int userId, int productId) =>
            _context.CartItems
                .Include(item => item.Cart)
                .Include(item => item.Product)
                .FirstOrDefaultAsync(item =>
                    item.Cart.UserId == userId &&
                    item.ProductId == productId);

        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task RemoveCartItemAsync(CartItem item)
        {
            _context.CartItems.Remove(item);
            return Task.CompletedTask;
        }

        public Task ClearCartItemsAsync(Cart cart)
        {
            _context.CartItems.RemoveRange(cart.Items);
            return Task.CompletedTask;
        }
    }
}
