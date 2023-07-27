using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.IO;
using System.Reflection.Emit;

namespace Data
{
    public class ServiceContext : DbContext
    {
        public ServiceContext(DbContextOptions<ServiceContext> options) : base(options) { }
        public DbSet<ProductItem> Products { get; set; }
        public DbSet<UserItem> Users { get; set; }
        public DbSet<RollItem> RollUser { get; set; }
        public DbSet<OrderItem> Orders { get; set; } //Добавляем DbSet для заказов
        public DbSet<AuditLog> AuditLogs { get; set; } //Добавляем DbSet для логирования


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProductItem>(entity =>         // Конфигурация модели ProductItem
            {
                entity.ToTable("Products");
            });

            builder.Entity<UserItem>(entity =>             // Задаем связь с таблицей "Users" по идентификатору IdUsuario    
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.IdUsuario);
                entity.HasOne<RollItem>().WithMany().HasForeignKey(u => u.Rol);
            });

            builder.Entity<RollItem>(entity =>              // Конфигурация модели RollItem
            {
                entity.ToTable("RollUser");
                entity.HasKey(u => u.IdRoll);
            });
            
            builder.Entity<OrderItem>(entity =>             // Конфигурация модели OrderItem
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.Id);                   // Задаем связь с таблицей "Products" по идентификатору продукта
                entity.HasOne(o => o.Product)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(o => o.ProductId)
                      .OnDelete(DeleteBehavior.NoAction); // Задаем правило удаления NO ACTION
            });

            builder.Entity<AuditLog>(entity =>              // Конфигурация модели AuditLog
            {
                entity.ToTable("AuditLog");
                entity.HasKey(a => a.Id);
                // Устанавливаем внешний ключ для связи с таблицей Users
                entity.HasOne(a => a.User)
                      .WithMany(u => u.AuditLogs)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(builder);
        }
        

        // Метод для удаления записи из таблицы "Products" по идентификатору
        public bool RemoveUserById(int productId)
        {
            // Находим запись, которую хотим удалить, по ее идентификатору
            var productToRemove = Products.FirstOrDefault(p => p.Id == productId);

            // Проверяем, была ли найдена запись
            if (productToRemove != null)
            {
                // Удаляем запись из контекста
                Products.Remove(productToRemove);

                // Сохраняем изменения в базе данных
                SaveChanges();
                return true; // Успешное удаление
            }

            return false; // Запись с указанным идентификатором не найдена
        }

        // Метод для удаления записи из таблицы "Orders" по идентификатору
        public bool RemoveOrderById(int orderId)
        {
            // Находим запись, которую хотим удалить, по ее идентификатору
            var orderToRemove = Orders.FirstOrDefault(p => p.Id == orderId);

            // Проверяем, была ли найдена запись
            if (orderToRemove != null)
            {
                // Удаляем запись из контекста
                Orders.Remove(orderToRemove);

                // Сохраняем изменения в базе данных
                SaveChanges();
                return true; // Успешное удаление
            }

            return false; // Запись с указанным идентификатором не найдена

        }


    }
    public class ServiceContextFactory : IDesignTimeDbContextFactory<ServiceContext>
    {
        public ServiceContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", false, true);
            var config = builder.Build();
            var connectionString = config.GetConnectionString("ServiceContext");
            var optionsBuilder = new DbContextOptionsBuilder<ServiceContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("ServiceContext"));
            return new ServiceContext(optionsBuilder.Options);
        }
    }
}