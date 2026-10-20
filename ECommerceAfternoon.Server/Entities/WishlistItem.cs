using System.ComponentModel.DataAnnotations;

namespace ECommerceAfternoon.Server.Entities
{
    public class WishlistItem
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

