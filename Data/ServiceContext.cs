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
        public DbSet<OrderItem> Orders { get; set; } //Añadir DbSet para pedidos
        public DbSet<AuditLog> AuditLogs { get; set; } //Añadir DbSet para el registro
        public DbSet<OrderStatus> OrderStatuses { get; set; } //Añadir estado del pedido DbSet


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProductItem>(entity =>         // Configuración del modelo ProductItem
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.ProductId);
            });

            builder.Entity<UserItem>(entity =>             // Establecer conexión con la tabla "Usuarios" por identificador IdUsuario   
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.IdUsuario);
                entity.HasOne<RollItem>().WithMany().HasForeignKey(u => u.IdRol);
            });

            builder.Entity<RollItem>(entity =>              // Configuración del modelo RollItem
            {
                entity.ToTable("RollUser");
                entity.HasKey(u => u.IdRoll);
            });
            
            builder.Entity<OrderItem>(entity =>             // Configuración del modelo OrderItem
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.IdOrder);
                entity.HasOne(o => o.Product)               // Establecer la conexión con la tabla "Productos" por ID de producto
                      .WithMany(p => p.Orders)
                      .HasForeignKey(o => o.ProductId);
                entity.HasOne(o => o.OrderStatus)           // Definir conexión con la tabla OrderStatus por identificador de estado del pedido
                    .WithMany()
                    .HasForeignKey(o => o.OrderStatusId)
                    .OnDelete(DeleteBehavior.NoAction); // Establecimiento de la regla de supresión NO ACTION
            });

            builder.Entity<AuditLog>(entity =>              // Configuración del modelo AuditLog
            {
                entity.ToTable("AuditLog");
                entity.HasKey(a => a.IdLog);
                // Establecer la clave externa para la comunicación con la tabla Usuarios
                entity.HasOne(a => a.User)
                      .WithMany(u => u.AuditLogs)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            builder.Entity<OrderStatus>(entity =>           // Configuración del modelo OrderStatus
            {
                entity.ToTable("OrderStatuses");
                entity.HasKey(os => os.OrderStatusId);
                base.OnModelCreating(builder);
            });
        }


        // Método para eliminar un registro de la tabla "Productos" por productId
        public bool RemoveProductById(int productId)
        {
            var productToRemove = Products.FirstOrDefault(p => p.ProductId == productId);

            if (productToRemove != null)
            {
                Products.Remove(productToRemove);
                SaveChanges();
                return true; // Eliminación con éxito
            }

            return false; // No se ha encontrado ningún registro con el identificador especificado
        }

        // Método para eliminar un registro de la tabla "Pedidos" por identificador
        public bool RemoveOrderById(int orderId)
        {
            var orderToRemove = Orders.FirstOrDefault(o => o.IdOrder == orderId);

            if (orderToRemove != null)
            {
                Orders.Remove(orderToRemove);
                SaveChanges();
                return true;// Eliminación con éxito
            }
            return false;//No se ha encontrado ningún registro con el identificador especificado
        }

        // En la clase ServiceContext u otra clase responsable de acceder a la base de datos

        public bool RemoveUserById(int userId)
        {
            // Busque el registro de usuario que desea eliminar por su identificador
            var userToRemove = Users.FirstOrDefault(u => u.IdUsuario == userId);

            // Comprobar si se ha encontrado el registro
            if (userToRemove != null)
            {
                // Eliminar una entrada del contexto
                Users.Remove(userToRemove);

                // Guardar los cambios en la base de datos
                SaveChanges();
                return true; // Eliminación con éxito
            }

            return false; // No se ha encontrado ningún registro con el identificador especificado
        }

        // Método para guardar el pedido
        public void SaveOrder(OrderItem orderItem)
        {
            // Comprueba si el OrderStatusId especificado existe en la tabla OrderStatuses
            bool orderStatusExists = OrderStatuses.Any(os => os.OrderStatusId == orderItem.OrderStatusId);

            if (orderStatusExists)
            {
                // Obtener el objeto OrderStatus por su OrderStatusId
                var orderStatus = OrderStatuses.Find(orderItem.OrderStatusId);

                // Asignar el objeto OrderStatus al objeto OrderItem
                orderItem.OrderStatus = orderStatus;

                // Guardar el objeto OrderItem
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