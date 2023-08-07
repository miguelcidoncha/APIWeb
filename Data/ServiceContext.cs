using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection.Emit;
using Entities.Entities;

namespace Data
{
    public class ServiceContext : DbContext
    {
        public ServiceContext(DbContextOptions<ServiceContext> options) : base(options) { }
        public DbSet<ProductItem> Products { get; set; }
        public DbSet<UserItem> Users { get; set; }
        public DbSet<RollItem> RollUser { get; set; }
        public DbSet<OrderItem> Orders { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<ImageItem> Images { get; set; }
        public DbSet<OrderProduct> OrderProduct { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProductItem>(entity =>         // Конфигурация модели ProductItem
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.ProductId);

                entity.HasMany(p => p.OrderProduct)       // Навигационное свойство для связи с промежуточной моделью OrderProduct
                .WithOne(op => op.Product)
                .HasForeignKey(op => op.ProductId);

                entity.HasMany(p => p.ImageItem)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);
            });

            builder.Entity<ImageItem>(entity =>    // Конфигурация таблицы "Images"
            {
                entity.ToTable("Images");
                entity.HasKey(i => i.IdImage);
            });

            builder.Entity<UserItem>(entity =>             // Задаем связь с таблицей "Users" по идентификатору IdUsuario    
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.IdUsuario);
                entity.HasOne<RollItem>().WithMany().HasForeignKey(u => u.IdRol);
            });

            builder.Entity<RollItem>(entity =>              // Конфигурация модели RollItem
            {
                entity.ToTable("RollUser");
                entity.HasKey(u => u.IdRoll);
            });
            
            builder.Entity<OrderItem>(entity =>             // Конфигурация модели OrderItem
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.OrderId);

                entity.HasMany(o => o.OrderProduct)
                      .WithOne(op => op.Order)
                      .HasForeignKey(op => op.OrderId);

                entity.HasOne(o => o.OrderStatus)           // Задаем связь с таблицей OrderStatus по идентификатору состояния заказа
                    .WithMany()
                    .HasForeignKey(o => o.OrderStatusId)
                    .OnDelete(DeleteBehavior.NoAction); // Задаем правило удаления NO ACTION
            });

            // Конфигурация модели OrderProduct (промежуточной модели)
            builder.Entity<OrderProduct>(entity =>
            {
                entity.ToTable("OrderProduct");
                entity.HasKey(op => op.Id);

                // Определение составного ключа
                entity.HasIndex(op => new { op.OrderId, op.ProductId });

                // Навигационные свойства для связи с моделями OrderItem и ProductItem
                entity.HasOne(op => op.Order)
                      .WithMany(o => o.OrderProduct)
                      .HasForeignKey(op => op.OrderId);

                entity.HasOne(op => op.Product)
                      .WithMany(p => p.OrderProduct)
                      .HasForeignKey(op => op.ProductId);

                entity.HasOne(op => op.UserItem) // Настройка связи для IdUsuario
                      .WithMany()
                      .HasForeignKey(op => op.IdUsuario)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<AuditLog>(entity =>              // Конфигурация модели AuditLog
            {
                entity.ToTable("AuditLog");
                entity.HasKey(a => a.IdLog);
                // Устанавливаем внешний ключ для связи с таблицей Users
                entity.HasOne(a => a.User)
                      .WithMany(u => u.AuditLogs)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<OrderStatus>(entity =>           // Конфигурация модели OrderStatus
            {
                entity.ToTable("OrderStatuses");
                entity.HasKey(os => os.OrderStatusId);
                base.OnModelCreating(builder);
            });
        }


        // Метод для удаления записи из таблицы "Products" по productId
        public bool RemoveProductById(int productId)
        {
            var productToRemove = Products.FirstOrDefault(p => p.ProductId == productId);

            if (productToRemove != null)
            {
                Products.Remove(productToRemove);
                SaveChanges();
                return true; // Успешное удаление
            }

            return false; // Запись с указанным идентификатором не найдена
        }

        // Метод для удаления записи из таблицы "Orders" по идентификатору
        public bool RemoveOrderById(int orderId)
        {
            var orderToRemove = Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (orderToRemove != null)
            {
                Orders.Remove(orderToRemove);
                SaveChanges();
                return true;// Успешное удаление
            }
            return false;// Запись с указанным идентификатором не найдена
        }

        // В классе ServiceContext или другом классе, отвечающем за доступ к базе данных

        public bool RemoveUserById(int userId)
        {
            // Находим запись пользователя, которую хотим удалить, по ее идентификатору
            var userToRemove = Users.FirstOrDefault(u => u.IdUsuario == userId);

            // Проверяем, была ли найдена запись
            if (userToRemove != null)
            {
                // Удаляем запись из контекста
                Users.Remove(userToRemove);

                // Сохраняем изменения в базе данных
                SaveChanges();
                return true; // Успешное удаление
            }

            return false; // Запись с указанным идентификатором не найдена
        }

        // Метод для сохранения заказа
        public void SaveOrder(OrderItem orderItem)
        {
            // Проверяем, существует ли указанный OrderStatusId в таблице OrderStatuses
            bool orderStatusExists = OrderStatuses.Any(os => os.OrderStatusId == orderItem.OrderStatusId);

            if (orderStatusExists)
            {
                // Получаем объект OrderStatus по его OrderStatusId
                var orderStatus = OrderStatuses.Find(orderItem.OrderStatusId);

                // Присваиваем объект OrderStatus объекту OrderItem
                orderItem.OrderStatus = orderStatus;

                // Сохраняем объект OrderItem
                Orders.Add(orderItem);
                SaveChanges();
            }
            else
            {
                throw new Exception("Invalid OrderStatusId. The specified OrderStatusId does not exist.");
            }
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