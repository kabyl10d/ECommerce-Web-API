using Microsoft.EntityFrameworkCore;
using SampleCrud.Data.Entities;

namespace SampleCrud.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CartItem> Cart { get; set; }
        public DbSet<Order> Order { get; set; } 
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Merchant> Merchants { get; set; }

        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()   //one to many
                .HasOne(e => e.Customer)
                .WithMany(e => e.Orders)
                .HasForeignKey(e => e.CustomerId)
                .IsRequired();

            //modelBuilder.Entity<Order>()   //one to many
            //    .HasOne(e => e.Product)
            //    .WithMany(e => e.Orders)
            //    .HasForeignKey(e => e.ProductId)
            //    .IsRequired();

            modelBuilder.Entity<Customer>()   //many to one
                .HasMany(e => e.Orders)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.CustomerId)
                .IsRequired();

            //modelBuilder.Entity<Product>()    //many to one
            //    .HasMany(e => e.Orders)
            //    .WithOne(e => e.Product)
            //    .HasForeignKey(e => e.ProductId)
            //    .IsRequired();

            modelBuilder.Entity<Merchant>()
                .HasMany(e => e.Products)
                .WithOne(e => e.Merchant)
                .HasForeignKey(e => e.MerchantId)
                .IsRequired();

            modelBuilder.Entity<Product>()
                .HasOne(e => e.Merchant)
                .WithMany(e => e.Products)
                .HasForeignKey(e => e.MerchantId)
                .IsRequired();

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Cart)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.CustomerId)
                .IsRequired();

            modelBuilder.Entity<CartItem>()
                .HasOne(e => e.Customer)
                .WithMany(e => e.Cart)
                .HasForeignKey(e => e.CustomerId)
                .IsRequired();

            //modelBuilder.Entity<Product>()
            //    .HasMany(e => e.CartItems)
            //    .WithOne(e => e.Product)
            //    .HasForeignKey(e => e.ProductId)
            //    .IsRequired();

            //modelBuilder.Entity<CartItem>()
            //    .HasOne(e => e.Product)
            //    .WithMany(e => e.CartItems)
            //    .HasForeignKey(e => e.ProductId)
            //    .IsRequired();
        }
    }
}
