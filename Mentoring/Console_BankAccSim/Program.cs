using System.Text.Json;
using Console_BankAccSim;

const string pathToFile = "data";
string accountsFileName = Path.Combine(pathToFile, "accounts.json");
string transactionsFileName = Path.Combine(pathToFile, "transactions.json");


List<BankAccount> accounts = [];
List<Transaction> transactions = [];

LoadAccountsFromFile();
//LoadTransactionsFromFile();

while (true)
{
    Console.Clear();
    ShowMenu();
    Console.Write("Enter your choice: ");
    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1": CreateAccount(); break;
        case "2": Deposit(); break;
        case "3": Withdraw(); break;
        case "4": Transfer(); break;
        case "5": ViewAccountDetails(); break;
        case "6": ViewTransactionHistory(); break;
        case "7": ChangeAccountStatus(); break;
        case "8": Console.WriteLine("Exiting..."); return;
        default: Console.WriteLine("Invalid choice! Please try again."); break;
    }

    Enter();
}

void LoadAccountsFromFile()
{
    try
    {
        if (!Directory.Exists(pathToFile))
        {
            Directory.CreateDirectory(pathToFile);
        }
        if (!File.Exists(accountsFileName))
        {
            Console.WriteLine("No existing accounts found. Starting with an empty account list.");
            File.WriteAllText(accountsFileName, "[]");
            accounts = [];
            return;
        }

        using FileStream inputAccountStream = File.OpenRead(accountsFileName);
        accounts = JsonSerializer.Deserialize<List<BankAccount>>(inputAccountStream) ?? [];
        Console.WriteLine($"Loaded {accounts.Count} accounts from {accountsFileName}!");
    }
    catch (JsonException)
    {
        accounts = [];
        SaveAccountsToFile();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Unexpected error: " + ex.Message);
    }
}

// void LoadTransactionsFromFile()
// {
    
// }

void SaveAccountsToFile()
{
    try
    {
        using FileStream outputStream = File.Create(accountsFileName);
        JsonSerializer.Serialize(outputStream, accountsFileName);
        Console.WriteLine($"Accounts saved!");
    }

    catch (Exception ex)
    {
        Console.WriteLine("Error: Save failed: " + ex.Message);
    }
}

// void SaveTransactionsToFile()
// {
    
// }

void ShowMenu()
{
    Console.Clear();
    Console.WriteLine("=== BANK ACCOUNT SYSTEM ===");
    Console.WriteLine("1. Create a new account");
    Console.WriteLine("2. Deposit money");
    Console.WriteLine("3. Withdraw money");
    Console.WriteLine("4. Transfer money");
    Console.WriteLine("5. View account details");
    Console.WriteLine("6. View transaction history");
    Console.WriteLine("7. Freeze/Unfreeze account");
    Console.WriteLine("8. Exit");
    Console.WriteLine("--------------------------------");
}

void Enter()
{
    Console.WriteLine("\nPress Enter to continue...");
    Console.ReadLine();
}

bool CheckAccountExistByNumber(string accountNumber)
{
    if (FindAccountByNumber(accountNumber) != null)
    {
        Console.WriteLine("Error: Account already exists!");
        return false;
    }
    return true;
}

bool CheckAccountStatus(BankAccount account)
{
    if (account.Status == AccountStatus.FROZEN)
    {
        Console.WriteLine("Error: Account is frozen!");
        return false;
    }
    return true;
}

void CreateAccount()
{
    Console.Clear();
    Console.WriteLine("--- Create Account ---");
    Console.Write("Enter account number: ");
    string? accountNumber = Console.ReadLine();
    Console.Write("Enter owner name: ");
    string? ownerName = Console.ReadLine();
    Console.Write("Enter initial balance: ");
    string? balanceStr = Console.ReadLine();

    // Check account exists
    if (string.IsNullOrWhiteSpace(accountNumber))
    {
        Console.WriteLine("Error: Account number is required!");
        return;
    }
    if (!CheckAccountExistByNumber(accountNumber)) return;

    if (string.IsNullOrWhiteSpace(ownerName))
    {
        Console.WriteLine("Error: Owner name is required!");
        return;
    }

    if (string.IsNullOrWhiteSpace(balanceStr))
    {
        Console.WriteLine("Error: Initial balance is required!");
        return;
    }
    if (!decimal.TryParse(balanceStr, out decimal initialBalance) || initialBalance < 0) {
        Console.WriteLine("Error: Invalid initial balance.");
        return;
    }

    BankAccount newAccount = new BankAccount(accountNumber, ownerName, initialBalance);
    accounts.Add(newAccount);
    SaveAccountsToFile();
    Console.WriteLine("Account created successfully!");
}

void Deposit()
{
    Console.Clear();
    Console.WriteLine("--- Deposit Money ---");
    Console.Write("Enter account number: ");
    string? accountNumber = Console.ReadLine();

    // Check account exists
    if (string.IsNullOrWhiteSpace(accountNumber))
    {
        Console.WriteLine("Error: Account number is required!");
        return;
    }
    if (FindAccountByNumber(accountNumber) == null)
    {
        Console.WriteLine("Error: Account not found!");
        return;
    }

    // Check account status
    BankAccount account = FindAccountByNumber(accountNumber);
    if (!CheckAccountStatus(account)) return;

    // Enter amount
    Console.Write("Enter deposit amount: ");
    string? amountStr = Console.ReadLine();
    Console.Write("Description (optional): ");
    string? description = Console.ReadLine() ?? "";
    
    if (string.IsNullOrWhiteSpace(amountStr))
    {
        Console.WriteLine("Error: Amount is required!");
        return;
    }
    if (!decimal.TryParse(amountStr, out decimal amount) || amount <= 0)
    {
        Console.WriteLine("Error: Invalid amount.");
        return;
    }

    account.Deposit(amount);
    Transaction newTransaction = new Transaction
    {
        Id = transactions.Count + 1,
        AccountNumber = account.AccountNumber,
        Type = TransactionType.DEPOSIT,
        Amount = amount,
        Description = description,
    };
    transactions.Add(newTransaction);

    Console.WriteLine("Deposit successful!");
    newTransaction.ToString();
    account.ToString();
    SaveAccountsToFile();
}

void Withdraw()
{
    Console.Clear();
    Console.WriteLine("--- Withdraw Money ---");
    Console.Write("Enter account number: ");
    string? accountNumber = Console.ReadLine();

    // Check account exists
    if (string.IsNullOrWhiteSpace(accountNumber))
    {
        Console.WriteLine("Error: Account number is required!");
        return;
    }
    if (FindAccountByNumber(accountNumber) == null)
    {
        Console.WriteLine("Error: Account not found!");
        return;
    }   

    // Check account status
    BankAccount account = FindAccountByNumber(accountNumber);
    if (account.Status == AccountStatus.FROZEN)
    {
        Console.WriteLine("Error: Account is frozen!");
        return;
    }
    
    // Enter amount
    Console.Write("Enter withdraw amount: ");
    string? amountStr = Console.ReadLine();
    Console.Write("Description (optional): ");
    string? description = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(amountStr))
    {
        Console.WriteLine("Error: Amount is required!");
        return;
    }
    if (!decimal.TryParse(amountStr, out decimal amount) || amount <= 0)
    {
        Console.WriteLine("Error: Invalid amount.");
        return;
    }
    if (amount > account.Balance)
    {
        Console.WriteLine("Error: Insufficient balance!");
        return;
    }

    account.Withdraw(amount);
    Transaction newTransaction = new Transaction
    {
        Id = transactions.Count + 1,
        AccountNumber = account.AccountNumber,
        Type = TransactionType.WITHDRAW,
        Amount = amount,
        Description = description,
    };
    transactions.Add(newTransaction);

    Console.WriteLine("Withdraw successful!");
    newTransaction.ToString();
    account.ToString();
    SaveAccountsToFile();
}

void Transfer()
{
    Console.Clear();
    Console.WriteLine("--- Transfer Money ---");
    Console.Write("Enter sender's account number: ");
    string? senderNumber = Console.ReadLine();
    Console.Write("Enter recipient's account number: ");
    string? recipientNumber = Console.ReadLine();

    // Check accounts exist
    

    if (string.IsNullOrWhiteSpace(senderNumber))
    {
        Console.WriteLine("Error: Sender account number is required!");
        return;
    }
    if (!CheckAccountExistByNumber(senderNumber)) return;

    if (string.IsNullOrWhiteSpace(recipientNumber))
    {
        Console.WriteLine("Error: Recipient account number is required!");
        return;
    }
    if (!CheckAccountExistByNumber(recipientNumber)) return;

    // Check account statuses
    BankAccount sender = FindAccountByNumber(senderNumber);
    if (!CheckAccountStatus(sender)) return;
    if (sender.Balance <= 0)
    {
        Console.WriteLine("Error: Sender account has no balance!");
        return;
    }

    BankAccount recipient = FindAccountByNumber(recipientNumber);
    if (!CheckAccountStatus(recipient)) return;

    // Enter amount
    Console.Write("Enter transfer amount: ");
    string? amountStr = Console.ReadLine();
    Console.Write("Description (optional): ");
    string? description = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(amountStr))
    {
        Console.WriteLine("Error: Amount is required!");
        return;
    }
    if (!decimal.TryParse(amountStr, out decimal amount) || amount <= 0)
    {
        Console.WriteLine("Error: Invalid amount.");
        return;
    }
    if (amount > sender.Balance)
    {
        Console.WriteLine("Error: Insufficient balance!");
        return;
    }

    sender.Withdraw(amount);
    recipient.Deposit(amount);
    Transaction newTransaction = new Transaction
    {
        Id = transactions.Count + 1,
        AccountNumber = sender.AccountNumber,
        Type = TransactionType.TRANSFER,
        Amount = amount,
        Description = description + $" to {recipient.AccountNumber}",
    };
    transactions.Add(newTransaction);

    SaveAccountsToFile();
    Console.WriteLine("Withdraw successful!");
    newTransaction.ToString();
    Console.Write(">> Sender: ");
    sender.ToString();
    Console.Write(">> Recipient: ");
    recipient.ToString();
    SaveAccountsToFile();
}

void ViewAccountDetails()
{
    Console.Write("Enter account number: ");
    string? accountNumber = Console.ReadLine();

    // Check account exists
    if (string.IsNullOrWhiteSpace(accountNumber))
    {
        Console.WriteLine("Error: Account number is required!");
        return;
    }
    if (FindAccountByNumber(accountNumber) == null)
    {
        Console.WriteLine("Error: Account not found!");
        return;
    }

    Console.WriteLine($"Account details:\n{FindAccountByNumber(accountNumber).ToString()}");
}

void ViewTransactionHistory()
{
    Console.Write("Enter account number: ");
    string? accountNumber = Console.ReadLine();

    // Check account exists
    if (string.IsNullOrWhiteSpace(accountNumber))
    {
        Console.WriteLine("Error: Account number is required!");
        return;
    }
    if (FindAccountByNumber(accountNumber) == null)
    {
        Console.WriteLine("Error: Account not found!");
        return;
    }

    bool invalidChoice = false;
    do
    {
        Console.WriteLine($"--- View Transaction of {accountNumber}---");
        Console.WriteLine("1. View all transactions.");
        Console.WriteLine("2. View only DEPOSITs.");
        Console.WriteLine("3. View only WITHDRAWs.");
        Console.WriteLine("4. View only TRANSFERs.");
        Console.WriteLine("--------------------------------");
        Console.Write("Enter your choice: ");
        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1": ViewTransactionHistoryByType(null); break;
            case "2": ViewTransactionHistoryByType(TransactionType.DEPOSIT); break;
            case "3": ViewTransactionHistoryByType(TransactionType.WITHDRAW); break;
            case "4": ViewTransactionHistoryByType(TransactionType.TRANSFER); break;
            default: Console.WriteLine("Invalid choice!"); invalidChoice = true; break;
        }
    } while(invalidChoice);
    
}

void ViewTransactionHistoryByType(TransactionType? type)
{
    var filteredTypes = type == null
        ? transactions
        : transactions.Where(n => n.Type == type).ToList();

    if (!filteredTypes.Any())
    {
        Console.WriteLine(type == null
            ? "No notes found."
            : $"No notes found with Type: '{type.ToString()}'"
        );
        return;
    }

    foreach (var t in filteredTypes)
    {
        if (t.Type == TransactionType.DEPOSIT) Console.ForegroundColor = ConsoleColor.Green;
        else if (t.Type == TransactionType.WITHDRAW) Console.ForegroundColor = ConsoleColor.Red;
        else if (t.Type == TransactionType.TRANSFER) Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(t.ToString());
        Console.ResetColor();
    }
}

void ChangeAccountStatus()
{
    Console.Write("Enter account number: ");
    string? accountNumber = Console.ReadLine();

    // Check account exists
    if (string.IsNullOrWhiteSpace(accountNumber))
    {
        Console.WriteLine("Error: Account number is required!");
        return;
    }
    if (FindAccountByNumber(accountNumber) == null)
    {
        Console.WriteLine("Error: Account not found!");
        return;
    }
    
    // Check account status
    BankAccount account = FindAccountByNumber(accountNumber);
    account.ToString();

    bool invalidChoice = false;
    do
    {
        Console.WriteLine("--------------------------------");
        Console.WriteLine("1. Freeze");
        Console.WriteLine("2. Unfreeze");
        Console.Write("Enter action (1 or 2): ");
        string? choice = Console.ReadLine();
        switch (choice)
        {
            case "1": 
                account.Freeze(); 
                Console.WriteLine("Account frozen!"); 
                break;
            case "2": 
                account.Unfreeze(); 
                Console.WriteLine("Account active!"); 
                break;
            default: Console.WriteLine("Invalid choice!"); invalidChoice = true; break;
        }
    } while (invalidChoice);
    
    SaveAccountsToFile();
}

BankAccount FindAccountByNumber(string accountNumber)
{
#pragma warning disable CS8603 // Possible null reference return.
    return accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
#pragma warning restore CS8603 // Possible null reference return.
}