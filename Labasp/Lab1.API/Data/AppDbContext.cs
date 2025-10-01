using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab1.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab1.API.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Sweet> Sweets { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфигурация связи Sweet -> Category
            modelBuilder.Entity<Sweet>(entity =>
            {
                entity.HasKey(s => s.Id);

                // Связь: одна категория -> много конфет
                entity.HasOne(s => s.Category)
                      .WithMany(c => c.Sweets)
                      .HasForeignKey(s => s.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict); // или Cascade

                // Дополнительные настройки если нужно
                entity.Property(s => s.Name).HasMaxLength(100);
                entity.Property(s => s.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).HasMaxLength(50);
            });
        }
    }
}