using Microsoft.EntityFrameworkCore;
using OrderService.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Api.Data
{
    public class OrderContext : DbContext, IOrderContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .OwnsOne(o => o.Address)
                .WithOwner();

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Product)
                .WithOne()
                .HasForeignKey(typeof(Order), "ProductId");
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

    }
}
