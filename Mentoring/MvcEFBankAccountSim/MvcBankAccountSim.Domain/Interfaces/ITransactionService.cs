namespace MvcBankAccountSim.Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetHistoryAsync(string accountNumber, string filter);
}