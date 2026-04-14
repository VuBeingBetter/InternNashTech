using System.Text.Json;

public class JsonAccountService : IAccountService
{
    private string _filePath;
    private List<BankAccount> _accounts;

    public JsonAccountService()
    {
        var folder = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
        // var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        
        _filePath = Path.Combine(folder, "accounts.json");
       
        if (File.Exists(_filePath) && new FileInfo(_filePath).Length > 0)
        {
            var json = File.ReadAllText(_filePath);
            _accounts = JsonSerializer.Deserialize<List<BankAccount>>(json) ?? new List<BankAccount>();
        }
        else
        {
            File.WriteAllText(_filePath, "[]");
            _accounts = new List<BankAccount>();
        }
    }

    public List<BankAccount> GetAllAccounts() => _accounts;

    public BankAccount GetAccountByNumber(string accountNumber)
    {
        return _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
    }

    public void Add(BankAccount account)
    {
        if (_accounts.Any(a => a.AccountNumber == account.AccountNumber))
        {
            throw new Exception("Account number already exists.");
        }
        _accounts.Add(account);
        SaveToFile();
    }

    public void Update(BankAccount account)
    {
        var index = _accounts.FindIndex(a => a.AccountNumber == account.AccountNumber);
        if (index != -1)
        {
            _accounts[index] = account;
            SaveToFile();
        }
    }

    public void SaveToFile()
    {
        var json = JsonSerializer.Serialize(_accounts, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}