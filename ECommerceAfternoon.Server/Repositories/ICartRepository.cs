using ECommerceAfternoon.Server.Entities;

namespace ECommerceAfternoon.Server.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<CartItem?> GetCartItemAsync(int userId, int productId);
        Task AddCartAsync(Cart cart);
        Task SaveChangesAsync();
        Task RemoveCartItemAsync(CartItem item);
        Task ClearCartItemsAsync(Cart cart);
    }
}
