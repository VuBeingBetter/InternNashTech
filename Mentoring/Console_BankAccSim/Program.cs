using Console_BankAccSim;

List<BankAccount> accounts = [];
List<Transaction> transactions = [];


while (true)
{
    Console.Clear();
    ShowMenu();
    Console.Write("Enter your choice: ");
    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1": CreateAccount(); break;
        case "2": DepositMoney(); break;
        case "3": WithdrawMoney(); break;
        case "4": TransferMoney(); break;
        case "5": ViewAccountDetails(); break;
        case "6": ViewTransactionHistory(); break;
        case "7": FreezeUnfreezeAccount(); break;
        case "8": Console.WriteLine("Exiting..."); return;
        default: Console.WriteLine("Invalid choice! Please try again."); break;
    }

    Enter();
}

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

    
}

bool CheckAccountExists(string accountNumber)
{
    return accounts.Any(a => a.AccountNumber == accountNumber);
}


void DepositMoney()
{
    
}

void WithdrawMoney()
{
    
}

void TransferMoney()
{
    
}

void ViewAccountDetails()
{
    
}

void ViewTransactionHistory()
{
    
}

void FreezeUnfreezeAccount()
{
    
}