using Microsoft.EntityFrameworkCore;

namespace NaturalLanguageSQLAPI.Models
{
    public partial class ChatbotContext : DbContext
    {
        public ChatbotContext()
        {
        }

        public ChatbotContext(DbContextOptions<ChatbotContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.Property(e => e.City).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Country).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Email).HasMaxLength(150).IsUnicode(false);
                entity.Property(e => e.FirstName).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.LastName).HasMaxLength(100).IsUnicode(false);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.Category).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.ProductName).HasMaxLength(150).IsUnicode(false);
            });
        }
    }
}
