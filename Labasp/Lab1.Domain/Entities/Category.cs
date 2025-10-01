namespace Lab1.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public string NormalizedName { get; set; } = null!;

        // Коллекция конфет в этой категории
        public virtual ICollection<Sweet> Sweets { get; set; } = new List<Sweet>();
    }
}