using AppNLayer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppNLayer.DAL;
public class AppNLayerDbContext(DbContextOptions<AppNLayerDbContext> options) 
    : IdentityDbContext<IdentityUser>(options)
{
    //public AppNLayerDbContext(DbContextOptions<AppNLayerDbContext> options) : base(options)
    //{        
    //}

    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(d => d.Category)
                  .WithMany(p => p.Books)
                  .HasForeignKey(d => d.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Seed initial data (optional)
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Fiction" },
            new Category { Id = 2, Name = "Science" },
            new Category { Id = 3, Name = "History" }
        );
    }

}
