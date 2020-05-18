using Microsoft.EntityFrameworkCore;
using Supermarket.API.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Persistence.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
             
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Category>().HasKey(p => p.Id);
            modelBuilder.Entity<Category>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            modelBuilder.Entity<Category>().Property(p => p.Name).IsRequired().HasMaxLength(30);
            modelBuilder.Entity<Category>().HasMany(p => p.Products).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Category>().HasData
         (
             new Category { Id = 100, Name = "Fruits and Vegetables" }, // Id set manually due to in-memory provider
             new Category { Id = 101, Name = "Dairy" }
         );

            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Product>().Property(p => p.QuantityInPackage).IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.UnitOfMeasurement).IsRequired();
        }
    }
}
