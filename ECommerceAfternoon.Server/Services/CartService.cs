using ECommerceAfternoon.Server.DTOs.Cart;
using ECommerceAfternoon.Server.Entities;
using ECommerceAfternoon.Server.Repositories;

namespace ECommerceAfternoon.Server.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;

        public CartService(ICartRepository repository)
        {
            _repository = repository;
        }

        public async Task<CartResponseDto> GetCartAsync(int userId)
        {
            var cart = await _repository.GetCartByUserIdAsync(userId);

            if (cart is null)
                return new CartResponseDto { Id = 0, UserId = userId, Total = 0 };

            var items = cart.Items.Select(item => new CartItemResponseDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Name = item.Product.Name,
                Price = item.Product.Price,
                ImageUrl = item.Product.ImageUrl,
                Quantity = item.Quantity,
                Subtotal = item.Product.Price * item.Quantity
            }).ToList();

            return new CartResponseDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = items,
                Total = items.Sum(item => item.Subtotal)
            };
        }

        public async Task AddToCartAsync(int userId, AddToCartDto dto)
        {
            if (dto.Quantity <= 0)
                throw new CartServiceException(400, "Quantity must be greater than zero.");

            var product = await _repository.GetProductByIdAsync(dto.ProductId);
            if (product is null)
                throw new CartServiceException(404, "Product not found.");

            var cart = await _repository.GetCartByUserIdAsync(userId);
            var existingItem = cart?.Items.FirstOrDefault(item => item.ProductId == dto.ProductId);
            var requestedQuantity = dto.Quantity + (existingItem?.Quantity ?? 0);

            if (requestedQuantity > product.Stock)
                throw new CartServiceException(400, "Not enough stock.");

            if (cart is null)
            {
                cart = new Cart { UserId = userId };
                await _repository.AddCartAsync(cart);
            }

            if (existingItem is not null)
            {
                existingItem.Quantity = requestedQuantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                });
            }

            await _repository.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int userId, int productId, int quantity)
        {
            if (quantity <= 0)
                throw new CartServiceException(400, "Quantity must be greater than zero.");

            var item = await _repository.GetCartItemAsync(userId, productId);
            if (item is null)
                throw new CartServiceException(404, "Cart item not found.");

            if (quantity > item.Product.Stock)
                throw new CartServiceException(400, "Not enough stock.");

            item.Quantity = quantity;
            await _repository.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int userId, int productId)
        {
            var item = await _repository.GetCartItemAsync(userId, productId);
            if (item is null)
                throw new CartServiceException(404, "Cart item not found.");

            await _repository.RemoveCartItemAsync(item);
            await _repository.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _repository.GetCartByUserIdAsync(userId);
            if (cart is null)
                return;

            await _repository.ClearCartItemsAsync(cart);
            await _repository.SaveChangesAsync();
        }
    }
}
