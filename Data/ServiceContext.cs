using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Entities.Entities;

namespace Data
{
    public class ServiceContext : DbContext
    {
        public ServiceContext(DbContextOptions<ServiceContext> options) : base(options) { }
        public DbSet<ProductItem> Products { get; set; }          //Products это имя используется в Services, также это имя БД     
        public DbSet<OrderItem> Orders { get; set; }
        public DbSet<UserItem> Users { get; set; }
        public DbSet<RolItem> UserRol { get; set; }
        public DbSet<OrderDetal> OrderDetal { get; set; }

    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasKey(o => o.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Users)
                .WithMany(u => u.Order)
                .HasForeignKey(o => o.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<OrderDetal>()
                .HasKey(op => op.Id);

            modelBuilder.Entity<OrderDetal>()
                .HasOne(op => op.OrderItem)
                .WithMany(o => o.OrderDetal)
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetal>()
                .HasOne(op => op.ProductItem)
                .WithMany(p => p.OrderDetal)
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetal>()
                .HasOne(op => op.UserItem)
                .WithMany(p => p.OrderDetal)
                .HasForeignKey(op => op.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ProductItem>()
                .HasKey(op => op.ProductId);


            modelBuilder.Entity<RolItem>()
                .HasKey(op => op.RolId);


            modelBuilder.Entity<UserItem>()
                .HasKey(op => op.UsuarioId);

            modelBuilder.Entity<UserItem>()
                 .HasOne(u => u.UserRol) // UserItem и UserRol, используется навигационное свойство
                 .WithMany(ur => ur.User)
                 .HasForeignKey(u => u.RolId)
                 .OnDelete(DeleteBehavior.Restrict);

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