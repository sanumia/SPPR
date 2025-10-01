using Lab1.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1.UI.Data
{
    internal class TempDbContext:DbContext
    {
        public TempDbContext(DbContextOptions<TempDbContext> options) : base(options)
        {
        }

        // DbSet'ы для ваших сущностей
        public DbSet<Sweet> Sweets { get; set; }
        public DbSet<Category> Categories { get; set; }

        // Если у вас есть другие сущности, добавьте их здесь
        // public DbSet<Dish> Dishes { get; set; }
        // public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация для сущности Sweet
            modelBuilder.Entity<Sweet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Description)
                      .HasMaxLength(500);
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                // Связь с Category
                entity.HasOne(e => e.Category)
                      .WithMany() // или .WithMany(c => c.Sweets) если есть навигационное свойство
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Конфигурация для сущности Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(e => e.NormalizedName)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(e => e.Description)
                      .HasMaxLength(300);

                // Индекс для быстрого поиска по NormalizedName
                entity.HasIndex(e => e.NormalizedName)
                      .IsUnique();
            });

            // При необходимости добавьте начальные данные
            // modelBuilder.Entity<Category>().HasData(...);
        }
    }
}
