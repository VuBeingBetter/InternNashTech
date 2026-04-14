namespace MvcBankAccountSim.Infrastructure.Data;

using MvcBankAccountSim.Domain.Entities;
using MvcBankAccountSim.Domain.Enums;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<BankAccount> Accounts => Set<BankAccount>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API Configuration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Seeding initial data
        modelBuilder.Entity<BankAccount>().HasData(
            new { 
                AccountNumber = "ACC-1000", 
                OwnerName = "Initial Admin", 
                Balance = 500.00m, 
                Status = AccountStatus.ACTIVE, 
                CreatedAt = DateTime.Now 
            }
        );
    }
}
