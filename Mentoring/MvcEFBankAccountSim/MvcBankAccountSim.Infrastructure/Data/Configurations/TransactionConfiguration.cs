namespace MvcBankAccountSim.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
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
        builder.HasOne<BankAccount>()
               .WithMany()
               .HasForeignKey(t => t.AccountNumber);
    }
}