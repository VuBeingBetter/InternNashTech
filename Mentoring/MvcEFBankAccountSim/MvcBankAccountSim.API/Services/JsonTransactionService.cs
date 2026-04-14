using System.Text.Json;

public class JsonTransactionService : ITransactionService
{
    private string _filePath;
    private List<Transaction> _transactions;

    public JsonTransactionService()
    {
        var folder = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
        // var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        
        _filePath = Path.Combine(folder, "transactions.json");
       
        if (File.Exists(_filePath) && new FileInfo(_filePath).Length > 0)
        {
            var json = File.ReadAllText(_filePath);
            _transactions = JsonSerializer.Deserialize<List<Transaction>>(json) ?? new List<Transaction>();
        }
        else
        {
            File.WriteAllText(_filePath, "[]");
            _transactions = new List<Transaction>();
        }
    }

    public List<Transaction> GetAllTransactions()
    {
        return _transactions;
    }

    public List<Transaction> GetTransactionsByAccountNumber(string accountNumber)
    {
        return _transactions.Where(t => t.AccountNumber == accountNumber).ToList();
    }

    public void Add(Transaction transaction)
    {
        transaction.Id = _transactions.Count > 0 ? _transactions.Max(t => t.Id) + 1 : 1;
        _transactions.Add(transaction);
        SaveToFile();
    }

    public void SaveToFile()
    {
        var json = JsonSerializer.Serialize(_transactions, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}