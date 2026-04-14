public interface ITransactionService
{
    List<Transaction> GetAllTransactions();
    List<Transaction> GetTransactionsByAccountNumber(string accountNumber);
    void Add(Transaction transaction);
    void SaveToFile();
}