using Microsoft.EntityFrameworkCore;

namespace Development_Demand_Forecasting.Model
{
    internal class AppContext : DbContext
    {
        public DbSet<Products> Products { get; set; }
        public DbSet<Warehouses> Warehouses { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<SalesHistory> SalesHistory { get; set; }

        public string path = System.IO.Path.Combine(System.AppContext.BaseDirectory, "Development_Demand_Forecasting.db");

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={path}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Products>()
                .HasIndex(p => p.SupplierId);

            modelBuilder.Entity<Inventory>()
                .HasIndex(i => i.ProductId);

            modelBuilder.Entity<Inventory>()
                .HasIndex(i => i.WarehouseId);

            modelBuilder.Entity<SalesHistory>()
                .HasIndex(s => new { s.ProductId, s.Date });

            modelBuilder.Entity<Inventory>()
                .HasKey(i => new { i.ProductId, i.WarehouseId });

            modelBuilder.Entity<Products>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SalesHistory>()
                .HasOne(s => s.Product)
                .WithMany(p => p.SalesHistory)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Inventory)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Warehouse)
                .WithMany(w => w.Inventory)
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}