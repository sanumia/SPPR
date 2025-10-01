namespace Lab1.Domain.Entities
{

        public class Sweet
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public string? Description { get; set; }
            public decimal Price { get; set; }
            public string? Image { get; set; }
            public string? ContentType { get; set; }

            // внешний ключ
            public int CategoryId { get; set; }

            // навигационное свойство
            public Category Category { get; set; } = null!;
        }

    
}