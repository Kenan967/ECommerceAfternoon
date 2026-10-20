using ECommerceAfternoon.Server.DTOs.Cart;
using ECommerceAfternoon.Server.Entities;
using ECommerceAfternoon.Server.Repositories;
using ECommerceAfternoon.Server.Services;
using Moq;
using Xunit;

namespace ECommerceAfternoon.Server.Tests
{
    public class CartServiceTests
    {
        private readonly Mock<ICartRepository> _repository = new();
        private CartService CreateService() => new(_repository.Object);

        [Fact]
        public async Task AddToCart_QuantityZero_ThrowsBadRequest()
        {
            var service = CreateService();

            var exception = await Assert.ThrowsAsync<CartServiceException>(
                () => service.AddToCartAsync(1, new AddToCartDto
                {
                    ProductId = 10,
                    Quantity = 0
                }));

            Assert.Equal(400, exception.StatusCode);
            _repository.Verify(x => x.GetProductByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task AddToCart_ProductDoesNotExist_ThrowsNotFound()
        {
            _repository.Setup(x => x.GetProductByIdAsync(10))
                .ReturnsAsync((Product?)null);

            var service = CreateService();

            var exception = await Assert.ThrowsAsync<CartServiceException>(
                () => service.AddToCartAsync(1, new AddToCartDto
                {
                    ProductId = 10,
                    Quantity = 1
                }));

            Assert.Equal(404, exception.StatusCode);
        }

        [Fact]
        public async Task AddToCart_QuantityExceedsStock_ThrowsBadRequest()
        {
            _repository.Setup(x => x.GetProductByIdAsync(10))
                .ReturnsAsync(new Product { Id = 10, Name = "T-Shirt", Stock = 2 });
            _repository.Setup(x => x.GetCartByUserIdAsync(1))
                .ReturnsAsync((Cart?)null);

            var service = CreateService();

            var exception = await Assert.ThrowsAsync<CartServiceException>(
                () => service.AddToCartAsync(1, new AddToCartDto
                {
                    ProductId = 10,
                    Quantity = 3
                }));

            Assert.Equal(400, exception.StatusCode);
            _repository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddToCart_ValidProduct_CreatesCartAndSaves()
        {
            _repository.Setup(x => x.GetProductByIdAsync(10))
                .ReturnsAsync(new Product { Id = 10, Name = "T-Shirt", Stock = 5 });
            _repository.Setup(x => x.GetCartByUserIdAsync(1))
                .ReturnsAsync((Cart?)null);

            Cart? createdCart = null;
            _repository.Setup(x => x.AddCartAsync(It.IsAny<Cart>()))
                .Callback<Cart>(cart => createdCart = cart)
                .Returns(Task.CompletedTask);

            var service = CreateService();

            await service.AddToCartAsync(1, new AddToCartDto
            {
                ProductId = 10,
                Quantity = 2
            });

            Assert.NotNull(createdCart);
            Assert.Equal(1, createdCart!.UserId);
            Assert.Single(createdCart.Items);
            Assert.Equal(10, createdCart.Items.First().ProductId);
            Assert.Equal(2, createdCart.Items.First().Quantity);
            _repository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddToCart_ExistingItem_IncreasesQuantity()
        {
            var product = new Product { Id = 10, Name = "T-Shirt", Stock = 5 };
            var cart = new Cart
            {
                Id = 1,
                UserId = 1,
                Items = new List<CartItem>
                {
                    new CartItem { Id = 7, ProductId = 10, Quantity = 1, Product = product }
                }
            };

            _repository.Setup(x => x.GetProductByIdAsync(10)).ReturnsAsync(product);
            _repository.Setup(x => x.GetCartByUserIdAsync(1)).ReturnsAsync(cart);

            var service = CreateService();

            await service.AddToCartAsync(1, new AddToCartDto
            {
                ProductId = 10,
                Quantity = 2
            });

            Assert.Equal(3, cart.Items.First().Quantity);
            _repository.Verify(x => x.SaveChangesAsync(), Times.Once);
            _repository.Verify(x => x.AddCartAsync(It.IsAny<Cart>()), Times.Never);
        }
    }
}
