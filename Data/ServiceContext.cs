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
        public DbSet<OrderItem> Orders { get; set; } 


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProductItem>(entity =>
            {
                entity.ToTable("Products");
            });


            builder.Entity<UserItem>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.IdUsuario);
                entity.HasOne<RollItem>().WithMany().HasForeignKey(u => u.Rol);
            });

            builder.Entity<RollItem>(entity =>
            {
                entity.ToTable("RollUser");
                entity.HasKey(u => u.IdRoll);
            });
            
            builder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.Id);
                
                entity.HasOne(o => o.Product)
                      .WithMany(p => p.Orders)
                      .HasForeignKey(o => o.ProductId)
                      .OnDelete(DeleteBehavior.NoAction); 
            });
        }

        public bool RemoveUserById(int IdUsuario)
        {

            var userToRemove = Users.FirstOrDefault(p => p.IdUsuario == IdUsuario);


            if (userToRemove != null)
            {

                Users.Remove(userToRemove);


                SaveChanges();
                return true;
            }

            return false;
        }


        public bool RemoveProductById(int productId)
        {
           
            var productToRemove = Products.FirstOrDefault(p => p.Id == productId);

            
            if (productToRemove != null)
            {

                Products.Remove(productToRemove);

             
                SaveChanges();
                return true;
            }

            return false; 
        }

       
        public bool RemoveOrderById(int orderId)
        {
         
            var orderToRemove = Orders.FirstOrDefault(p => p.Id == orderId);

            if (orderToRemove != null)
            {
               
                Orders.Remove(orderToRemove);

                SaveChanges();
                return true; 
            }

            return false; 

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