namespace Infrastructure.Data;

using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<BankAccount> Accounts => Set<BankAccount>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Fluent API Configuration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Seeding initial data
        modelBuilder.Entity<BankAccount>().HasData(
            new { 
                AccountNumber = "1000000000", 
                OwnerName = "Initial Admin", 
                Balance = 500.00m, 
                Status = AccountStatus.ACTIVE, 
                CreatedAt = new DateTime(2026, 4, 16) 
            }
        );
    }
}
