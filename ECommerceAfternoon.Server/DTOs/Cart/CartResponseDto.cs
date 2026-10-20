namespace ECommerceAfternoon.Server.DTOs.Cart
{
    public class CartResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemResponseDto> Items { get; set; } = new();
        public decimal Total { get; set; }
    }

    public class CartItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
