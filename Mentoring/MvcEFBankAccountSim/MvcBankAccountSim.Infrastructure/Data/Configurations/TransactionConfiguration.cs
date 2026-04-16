using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using MvcBankAccountSim.Domain.Entities;
using MvcBankAccountSim.Domain.Enums;
using MvcBankAccountSim.Application.Interfaces;
using MvcBankAccountSim.Infrastructure.Data;



namespace MvcBankAccountSim.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
               .ValueGeneratedOnAdd();

        builder.Property(t => t.AccountNumber)
               .IsRequired()
               .HasMaxLength(20);
        builder.Property(t => t.Amount)
               .IsRequired()
               .HasColumnType("decimal(18,2)");
        builder.Property(t => t.Type)
               .IsRequired()
               .HasConversion<string>();
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.Description)
               .HasMaxLength(200);
               
        builder.HasOne<BankAccount>()
               .WithMany()
               .HasForeignKey(t => t.AccountNumber);
    }
}