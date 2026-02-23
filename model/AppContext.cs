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
            // Configure relationships
            modelBuilder.Entity<Products>()
                .HasOne(p => p.Supplier)
                .WithMany()
                .HasForeignKey(p => p.SupplierId);

            modelBuilder.Entity<SalesHistory>()
                .HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<Inventory>()
                .HasKey(i => new { i.ProductId, i.WarehouseId });

        }
    }
}