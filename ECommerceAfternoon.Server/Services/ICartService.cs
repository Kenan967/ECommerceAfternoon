using ECommerceAfternoon.Server.DTOs.Cart;

namespace ECommerceAfternoon.Server.Services
{
    public interface ICartService
    {
        Task<CartResponseDto> GetCartAsync(int userId);
        Task AddToCartAsync(int userId, AddToCartDto dto);
        Task UpdateQuantityAsync(int userId, int productId, int quantity);
        Task RemoveFromCartAsync(int userId, int productId);
        Task ClearCartAsync(int userId);
    }
}
