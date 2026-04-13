public interface IAccountService
{
    List<BankAccount> GetAllAccounts();
    BankAccount GetAccountByNumber(string accountNumber);
    void Add(BankAccount account);
    void Update(BankAccount account);
    void SaveToFile();
}