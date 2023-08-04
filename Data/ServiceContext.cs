using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Entities.Entities;

namespace Data
{
    public class ServiceContext : DbContext
    {
        public ServiceContext(DbContextOptions<ServiceContext> options) : base(options) { }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserItem> UserItems { get; set; }
        public DbSet<OrderProduct> OrderProduct { get; set; }

    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderProduct>()
                .HasKey(op => op.Id);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.UserItem)
                .WithMany()
                .HasForeignKey(op => op.UsuarioId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.OrderItem)
                .WithMany()
                .HasForeignKey(op => op.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.ProductItem)
                .WithMany()
                .HasForeignKey(op => op.ProductId);

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