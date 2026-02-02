namespace Lab1.Domain.Models
{
    public class CartItem
    {
        public int SweetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public int Count { get; set; } = 1;

        public decimal TotalPrice => Price * Count;
    }
}

