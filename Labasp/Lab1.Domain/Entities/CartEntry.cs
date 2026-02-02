namespace Lab1.Domain.Entities
{
    public class CartEntry
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int SweetId { get; set; }
        public int Quantity { get; set; }

        public Sweet Sweet { get; set; } = null!;
    }
}

