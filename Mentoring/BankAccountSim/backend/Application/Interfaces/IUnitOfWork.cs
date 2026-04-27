namespace Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IAccountRepository Accounts { get; }
    ITransactionRepository Transactions { get; }
    Task<int> SaveChangesAsync(); // Change SaveToFile into SQL Save
}