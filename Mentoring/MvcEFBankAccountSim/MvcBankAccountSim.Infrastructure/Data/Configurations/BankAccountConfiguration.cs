using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MvcBankAccountSim.Domain.Entities;
using MvcBankAccountSim.Domain.Enums;
using MvcBankAccountSim.Application.Interfaces;
using MvcBankAccountSim.Infrastructure.Data;


namespace MvcBankAccountSim.Infrastructure.Data.Configurations;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.HasKey(a => a.AccountNumber);
        
        builder.Property(a => a.AccountNumber)
               .IsRequired()
               .HasMaxLength(20);
        builder.Property(a => a.OwnerName)
               .IsRequired()
               .HasMaxLength(100);
        builder.Property(a => a.Balance)
               .IsRequired()
               .HasColumnType("decimal(18,2)");
        builder.Property(a => a.Status)
               .IsRequired()
               .HasConversion<string>();
        builder.Property(a => a.CreatedAt).IsRequired();
    }
}